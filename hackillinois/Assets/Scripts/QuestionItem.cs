using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public enum AnswerState { None, Good, Monitor, Fail }
public class QuestionItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionLabel;
    public TextMeshProUGUI commentLabel;
    public Button goodButton;
    public Button monitorButton;
    public Button failButton;
    public Button activateButton;
    public TextMeshProUGUI transcription;
    private AnswerState _currentAnswer = AnswerState.None;
    public AnswerState CurrentAnswer => _currentAnswer;
    public Action<QuestionItem> OnAnswerChanged;
    public Action<QuestionItem> OnSelected;
    private static readonly Color ColorGood = new Color(0.2f, 0.75f, 0.2f);
    private static readonly Color ColorMonitor = new Color(0.95f, 0.75f, 0.1f);
    private static readonly Color ColorFail = new Color(0.85f, 0.2f, 0.2f);
    private static readonly Color ColorUnselected = new Color(0.35f, 0.35f, 0.35f);
    void Awake()
    {
        // Debug.Log($"[QuestionItem] GO: {gameObject.name} ...");
        goodButton.onClick.AddListener(() => SetAnswer(AnswerState.Good));
        monitorButton.onClick.AddListener(() => SetAnswer(AnswerState.Monitor));
        failButton.onClick.AddListener(() => SetAnswer(AnswerState.Fail));
        transcription.fontMaterial = new Material(transcription.fontMaterial);
        activateButton.onClick.AddListener(() => startListening());
        ResetButtonColors();
    }
    public void startListening()
    {
        Debug.Log($"StartListening called with: {transcription.name} | instanceID: {transcription.GetInstanceID()}");
        VoiceCommentManager.Instance.StartListening(transcription);
    }
    public void Setup(string question, string comment)
    {
        questionLabel.text = question;
        commentLabel.text = comment;
    }
    private void SetAnswer(AnswerState state)
    {
        _currentAnswer = state;
        ResetButtonColors();
        switch (state)
        {
            case AnswerState.Good: SetButtonColor(goodButton, ColorGood); break;
            case AnswerState.Monitor: SetButtonColor(monitorButton, ColorMonitor); break;
            case AnswerState.Fail: SetButtonColor(failButton, ColorFail); break;
        }
        OnAnswerChanged?.Invoke(this);
    }
    public void Select()
    {
        OnSelected?.Invoke(this);
    }
    private void ResetButtonColors()
    {
        SetButtonColor(goodButton, ColorUnselected);
        SetButtonColor(monitorButton, ColorUnselected);
        SetButtonColor(failButton, ColorUnselected);
    }
    private void SetButtonColor(Button btn, Color color)
    {
        var colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.15f;
        colors.pressedColor = color * 0.85f;
        colors.selectedColor = color;
        btn.colors = colors;
    }
}