using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Services.Assistant.v2;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.WatsonDeveloperCloud.Assistant.v2;
using UnityEngine;
using UnityEngine.UI;

public class WatsonAssistant : MonoBehaviour
{
    [SerializeField]
    private string serviceURL;

    [SerializeField]
    private string assistantId;

    [SerializeField]
    private string versionDate;

    [SerializeField]
    private string iamAPIkey;

    [SerializeField] 
    private Text outputText;
    
    [SerializeField] 
    private Text chatHistory;
    
    private Assistant service;
    private string sessionId;
    private bool canRequestMessage;
    private string messageRequest;


    public Animator anim;
    private void Start()
    {
        StartCoroutine(CreateService());
        MessengerBehaviour.onSTTCompleteEvent += takeInput;
    }

    public void takeInput()
    {
        if (!MessengerBehaviour.Instance.isAvatartalking)
        {
            messageRequest = MessengerBehaviour.Instance.FinalMassageInput;
            //inputField.text = "";
            chatHistory.text +=  messageRequest+"\n" ;

            MessageRequest messageRequest1 = new MessageRequest()
            {
                Input = new MessageInput()
                {
                    Text = messageRequest
                }
            };
            service.Message(OnMassageSuccess, OnMassageFail, assistantId, sessionId, messageRequest1);
        }
        
    }
    
    
    IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamAPIkey))
        {
            throw new WatsonException("Please Provide a valid IAM API KEY");
        }

        Debug.Log("Applying Credentials...");
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = iamAPIkey
        };
        
        Credentials credentials = new Credentials(tokenOptions,serviceURL);
        
        yield return new WaitUntil(credentials.HasIamTokenData);
        
        service = new Assistant(credentials);
        service.VersionDate = versionDate;

        Debug.Log("Creating a New Session...");
        service.CreateSession(OnCreateSessionSuccess, OnCreateSessionFailed, assistantId);

    }

    private void OnCreateSessionSuccess(SessionResponse response, Dictionary<string, object> customdata)
    {
        
        sessionId = response.SessionId;
        canRequestMessage = true;
        service.Message(OnMassageSuccess, OnMassageFail, assistantId, sessionId);
        Debug.Log("Session Created");
        
    }
    
    private void OnCreateSessionFailed(RESTConnector.Error error, Dictionary<string, object> customdata)
    {
        Debug.Log("Session could not be Created... FAILED" + error.ErrorMessage);

    }

    private void OnMassageSuccess(MessageResponse response, Dictionary<string, object> customdata)
    {
        Debug.Log("Response: "+response.Output.Generic[0].Text);
        MessengerBehaviour.Instance.FinalMassageOutput = response.Output.Generic[0].Text;
        MessengerBehaviour.Instance.TTSCompleted();
        outputText.text = response.Output.Generic[0].Text;
        anim.SetTrigger("doTalk");
    }

    private void OnMassageFail(RESTConnector.Error error, Dictionary<string, object> customdata)
    {
        string err = "Watson Assistant OnFail Call failed: " + error.ErrorCode + " " + error.ErrorMessage;
        Debug.LogError(err);

    }
}
