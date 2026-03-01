using Meta.XR.BuildingBlocks.AIBlocks;
using TMPro;
using UnityEngine;

public class SpeechToTextController : MonoBehaviour
{
    public SpeechToTextAgent speechAgent;  // Assign your SpeechToTextAgent prefab component
    public TMP_Text transcriptText;        // Assign your TMP text to show transcript
    public TMP_Text statusText;            // Assign TMP for status display

    private bool isListening = false;

    private void OnEnable()
    {
        if (speechAgent != null)
        {
            // Subscribe to the OnTranscript event (check exact event name!)
            speechAgent.onTranscript.AddListener(OnTranscriptReceived);
        }
        UpdateStatus();
    }

    private void OnDisable()
    {
        if (speechAgent != null)
        {
            speechAgent.onTranscript.RemoveListener(OnTranscriptReceived);
        }
    }

    private void OnTranscriptReceived(string transcript)
    {
        transcriptText.text = transcript;
    }

    private void UpdateStatus()
    {
        statusText.text = isListening ? "Listening..." : "Stopped";
    }

    public void ToggleListening()
    {
        if (speechAgent == null) return;

        if (!isListening)
        {
            speechAgent.StartListening();
            isListening = true;
        }
        else
        {
            speechAgent.StopNow();
            isListening = false;
        }
        UpdateStatus();
    }
}