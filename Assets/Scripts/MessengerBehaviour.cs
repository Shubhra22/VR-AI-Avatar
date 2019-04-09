using UnityEngine;
using UnityEngine.UI;

public class MessengerBehaviour : Manager<MessengerBehaviour>
{
    string finalMassageInput;
    string finalMassageOutput;

    public bool isAvatartalking { get; set; }
    public InputField messageBox;

    public delegate void OnSpeechToTextComplete ();
    public static event OnSpeechToTextComplete onSTTCompleteEvent;

    public delegate void OnTextToSpeechComplete();
    public static event OnTextToSpeechComplete onTTSCompleteEvent;
    
    public string FinalMassageInput
    {
        get { return finalMassageInput; }
        set
        {
            finalMassageInput = value;
            messageBox.text = finalMassageInput;
        }

    }

    public string FinalMassageOutput
    {
        get { return finalMassageOutput; }
        set
        {
            finalMassageOutput = value;
        }

    }
    public void STTCompleted()
    {
        onSTTCompleteEvent();
    }
    
    public void TTSCompleted()
    {
        onTTSCompleteEvent();
    }
}

