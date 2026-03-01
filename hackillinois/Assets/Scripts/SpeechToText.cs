using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Meta.XR.BuildingBlocks.AIBlocks;

public class SpeechToText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button listenButton;
    [SerializeField] private TextMeshProUGUI outputText;

    [Header("Settings")]
    [SerializeField] private string listeningLabel = "Listening...";
    [SerializeField] private string idleLabel = "Press to Speak";

    private SpeechToText _speechToText;
    private bool _isListening = false;

    private void Start()
    {
        // Find the SpeechToTextBuildingBlock in the scene
        _speechToText = FindObjectOfType<SpeechToText>();

        if (_speechToText == null)
        {
            Debug.LogError("SpeechToTextBuildingBlock not found in scene!");
            return;
        }

        // Subscribe to transcription events
        //_speechToText.OnTranscriptionResult += OnTranscriptionResult;
        //_speechToText.OnTranscriptionStarted += OnTranscriptionStarted;
        //_speechToText.OnTranscriptionStopped += OnTranscriptionStopped;

        //listenButton.onClick.AddListener(ToggleListening);
    }

    //private void ToggleListening()
    //{
    //    if (_isListening)
    //    {
    //        _speechToText.StopTranscription();
    //    }
    //    else
    //    {
    //        _speechToText.StartTranscription();
    //    }
    //}

    private void OnTranscriptionStarted()
    {
        _isListening = true;
        outputText.text = listeningLabel;
        // Optionally update button label if you have one
    }

    private void OnTranscriptionStopped()
    {
        _isListening = false;
    }

    private void OnTranscriptionResult(string transcription, bool isFinal)
    {
        outputText.text = transcription;

        if (isFinal)
        {
            _isListening = false;
        }
    }

    //private void OnDestroy()
    //{
    //    if (_speechToText != null)
    //    {
    //        _speechToText.OnTranscriptionResult -= OnTranscriptionResult;
    //        _speechToText.OnTranscriptionStarted -= OnTranscriptionStarted;
    //        _speechToText.OnTranscriptionStopped -= OnTranscriptionStopped;
    //    }

    //    listenButton.onClick.RemoveListener(ToggleListening);
    //}
}