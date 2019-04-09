using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Utilities;


public class WatsonTTS : WatsonCommon
{
    private TextToSpeech service;

    [SerializeField]
    private GameObject AvatarModel;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateService());
        MessengerBehaviour.onTTSCompleteEvent += ConvertTextToSpeech;
    }

    // Update is called once per frame
    void Update()
    {
//        if (Input.GetKeyDown(KeyCode.A))
//        {           
//            ConvertTextToSpeech();
//        }
    }
    
    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamAPIKey))
        {
            throw new WatsonException("Plesae provide IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        Credentials credentials = null;

        //  Authenticate using iamApikey
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = iamAPIKey
        };

        credentials = new Credentials(tokenOptions, serviceUrl);

        //  Wait for tokendata
        yield return new WaitUntil(credentials.HasIamTokenData);

        service = new TextToSpeech(credentials);

    }


    private void ConvertTextToSpeech()
    {
        service.Voice = VoiceType.en_US_Lisa;
        service.ToSpeech(OnSucessTextToSpeech, OnFail, MessengerBehaviour.Instance.FinalMassageOutput, true);
    }

    private void OnSucessTextToSpeech(AudioClip response, Dictionary<string, object> customdata)
    {
        PlayClip(response);
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customdata)
    {
        Debug.Log("WATSONTTS.ONFAIL"+error.ErrorMessage);
    }

    private void PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            GameObject audioObject = new GameObject("AudioObject");
            AudioSource source = AvatarModel.GetComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.loop = false;
            source.clip = clip;
            source.Play();
            MessengerBehaviour.Instance.isAvatartalking = true;

            StartCoroutine(DestroyClipAndResumeListening(audioObject, clip));
        }
    }

    IEnumerator DestroyClipAndResumeListening(GameObject audioObject, AudioClip clip)
    {
        Destroy(audioObject, clip.length);
        
        yield return new WaitForSeconds(clip.length);

        Debug.Log("Came here to make it false");
        MessengerBehaviour.Instance.isAvatartalking = false;
    }
}
