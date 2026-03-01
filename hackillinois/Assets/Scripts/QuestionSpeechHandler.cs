using UnityEngine;
using TMPro;
using Oculus.Voice;

public class QuestionSpeechHandler : MonoBehaviour
{
    [Header("Internal References (auto-assigned or drag in)")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] private TMP_Text transcriptionText;

    private bool _isThisInstanceListening = false;

    private void Start()
    {
        // Find sibling TMP if not assigned
        if (transcriptionText == null)
            transcriptionText = GetComponentInChildren<TMP_Text>(); // be specific in Inspector
    }

    public void StartListening()
    {
        _isThisInstanceListening = true;

        // Subscribe only when this instance initiates
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartial);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFull);

        appVoiceExperience.Activate();
    }

    private void OnPartial(string partial)
    {
        if (_isThisInstanceListening)
            transcriptionText.text = partial;
    }

    private void OnFull(string full)
    {
        if (_isThisInstanceListening)
        {
            transcriptionText.text = full;
            Cleanup();
        }
    }

    private void Cleanup()
    {
        _isThisInstanceListening = false;
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartial);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFull);
    }

    private void OnDestroy() => Cleanup();
}