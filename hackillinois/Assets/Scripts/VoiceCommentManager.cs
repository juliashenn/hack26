using UnityEngine;
using TMPro;
using Oculus.Voice.Dictation;


public class VoiceCommentManager : MonoBehaviour
{
    public static VoiceCommentManager Instance;

    public AppDictationExperience dictation;

    private TMP_Text currentTargetText;

    void Awake()
    {
        Instance = this;
        dictation.DictationEvents.OnFullTranscription
            .AddListener(OnTranscriptionReceived);
    }

    public void StartListening(TMP_Text targetText)
    {
        // Stop if already active
        if (dictation.Active)
        {
            dictation.Deactivate();
            return;
        }
            

        currentTargetText = targetText;
        dictation.Activate();
    }

    public void StopListening()
    {
        if (dictation.Active)
            dictation.Deactivate();
    }

    private void OnTranscriptionReceived(string text)
    {
        Debug.Log($"[VoiceManager] Transcription received, writing to: {currentTargetText?.GetInstanceID()}");
        if (currentTargetText != null)
        {
            currentTargetText.text += text + "\n";
        }
    }
}