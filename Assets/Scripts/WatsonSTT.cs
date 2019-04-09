using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class WatsonSTT : WatsonCommon
{
    
    private SpeechToText service;
    private AudioClip recording = null;

    private string microphoneId;
    private int recordingBufferSize = 1;
    private int recordingHZ = 22050;

    private int recordingRoutine;

    private string _recognizeModel;

    public Text resultText;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateService());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool Active
    {
        get { return service.IsListening; }
        set
        {
            if (value && !service.IsListening)
            {
                service.RecognizeModel = (string.IsNullOrEmpty(_recognizeModel) ? "en-US_BroadbandModel" : _recognizeModel);
                service.DetectSilence = true;
                service.EnableWordConfidence = true;
                service.EnableTimestamps = true;
                service.SilenceThreshold = 0.01f;
                service.MaxAlternatives = 1;
                service.EnableInterimResults = true;
                service.OnError = OnError;
                service.InactivityTimeout = -1;
                service.ProfanityFilter = false;
                service.SmartFormatting = true;
                service.SpeakerLabels = false;
                service.WordAlternativesThreshold = null;
                service.StartListening(OnRecognize, OnRecognizeSpeaker);
            }
            else if (!value && service.IsListening)
            {
                service.StopListening();
            }
        }
    }
    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamAPIKey))
        {
            throw new WatsonException("Plesae provide IAM ApiKey for the service.");
        }

        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = iamAPIKey
        };
        
        Credentials credentials = new Credentials(tokenOptions,serviceUrl);
        
        yield return new WaitUntil(credentials.HasIamTokenData);
        
        service = new SpeechToText(credentials);
        service.StreamMultipart = true;
        
        Active = true;
        StartRecording();
        
    }

    private void StartRecording()
    {       
        if (recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (recordingRoutine != 0)
        {
            Microphone.End(microphoneId);
            Runnable.Stop(recordingRoutine);
            recordingRoutine = 0;
        }
    }

    private IEnumerator RecordingHandler()
    {
        
        recording = Microphone.Start(microphoneId, true, recordingBufferSize, recordingHZ);
        yield return null;      // let recordingRoutine get set..

        if (recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = recording.samples / 2;
        float[] samples = null;

        while (recordingRoutine != 0 && recording != null)
        {
            if (MessengerBehaviour.Instance.isAvatartalking)
            {
                Active = false;
            }
            else
            {
                Active = true;
            }
            

            int writePos = Microphone.GetPosition(microphoneId);
            if (writePos > recording.samples || !Microphone.IsRecording(microphoneId))
            {
                Debug.LogError("WatsonSTT.RecordingHandler()"+"Microphone disconnected.");

                StopRecording();
                yield break;
            }

            if ((bFirstBlock && writePos >= midPoint)
              || (!bFirstBlock && writePos < midPoint))
            {
                // front block is recorded, make a RecordClip and pass it onto our callback.
                samples = new float[midPoint];
                recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, recording.channels, recordingHZ, false);
                record.Clip.SetData(samples, 0);

                service.OnListen(record);

                bFirstBlock = !bFirstBlock;
                
                Debug.Log("Started Recording...");
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio, 
                // and wait that amount of time it will take to record.
                int remaining = bFirstBlock ? (midPoint - writePos) : (recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }

        }

        yield break;
    }
    
     private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
                    if (res.final)
                    {
                        MessengerBehaviour.Instance.FinalMassageInput = alt.transcript;
                        MessengerBehaviour.Instance.STTCompleted();
                    }
                    resultText.text = text;

                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Debug.Log(string.Format("WatsonSTT.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time));
                    }
                }

                if (res.word_alternatives != null)
                {
                    foreach (var wordAlternative in res.word_alternatives)
                    {
                        foreach (var alternative in wordAlternative.alternatives)
                        {
                            Debug.Log(string.Format("word: {0} | confidence: {1}", alternative.word, alternative.confidence));

                        }
                    }
                }
            }
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Debug.Log(string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
            }
        }
    }
    
    private void OnError(string error)
    {
        Active = false;

        Debug.Log("Error!" + error);
    }
    
}
