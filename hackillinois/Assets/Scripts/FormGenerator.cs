using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach to a GameObject. Assign the scrollRect and prefabs in Inspector,
/// then call GenerateForm(FormData) at runtime — or fill in the [SerializeField]
/// testFormData and enable generateOnStart for quick prototyping.
/// </summary>
public class FormGenerator : MonoBehaviour
{
    [Header("Scroll View")]
    [Tooltip("The Scroll Rect that contains the content panel.")]
    public ScrollRect scrollRect;

    [Header("Prefabs")]
    public GameObject sectionHeaderPrefab;   // has TextMeshProUGUI
    public GameObject questionItemPrefab;    // has QuestionItem component

    [Header("Spacing")]
    public float sectionSpacing = 24f;
    public float questionSpacing = 12f;

    [Header("Prototype / Test")]
    public bool generateOnStart = false;
    [SerializeField] private FormData testFormData;

    // All spawned question items, accessible after generation
    public List<QuestionItem> SpawnedQuestions { get; private set; } = new List<QuestionItem>();

    // ------------------------------------------------------------------ //

    void Start()
    {
        if (generateOnStart && testFormData != null)
            GenerateForm(testFormData);
    }

    // ------------------------------------------------------------------ //
    //  PUBLIC API
    // ------------------------------------------------------------------ //

    /// <summary>Clears any existing form and builds a new one from data.</summary>
    public void GenerateForm(FormData data)
    {
        ClearForm();

        RectTransform content = scrollRect.content;

        // Vertical layout on content (add if not present)
        VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.spacing = questionSpacing;
        vlg.padding = new RectOffset(16, 16, 16, 32);

        ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
        if (csf == null) csf = content.gameObject.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Canvas.ForceUpdateCanvases();

        foreach (FormSection section in data.sections)
        {
            // --- Section Header ---
            GameObject headerGO = Instantiate(sectionHeaderPrefab, content);
            TextMeshProUGUI headerText = headerGO.GetComponentInChildren<TextMeshProUGUI>();
            if (headerText != null) headerText.text = section.sectionHeader;

            // Extra spacing above section (after first)
            LayoutElement le = headerGO.GetComponent<LayoutElement>();
            if (le == null) le = headerGO.AddComponent<LayoutElement>();
            le.minHeight = 40f;

            // --- Questions ---
            foreach (FormQuestion q in section.questions)
            {
                GameObject qGO = Instantiate(questionItemPrefab, content);
                QuestionItem item = qGO.GetComponent<QuestionItem>();
                if (item != null)
                {
                    item.Setup(q.questionText, q.commentText);
                    item.OnAnswerChanged += HandleAnswerChanged;
                    SpawnedQuestions.Add(item);
                }
            }
        }
        StartCoroutine(ResetScrollPositions());
    }

    private System.Collections.IEnumerator ResetScrollPositions()
    {
        // Wait two frames for Unity to finish building and measuring the layout
        yield return null;
        yield return null;

        // Outer: start at first section (left)
        scrollRect.horizontalNormalizedPosition = 0f;

        // Each inner vertical scroll: start at top
        foreach (ScrollRect inner in scrollRect.content.GetComponentsInChildren<ScrollRect>())
            inner.verticalNormalizedPosition = 1f;
    }

    /// <summary>Destroy all spawned children and reset state.</summary>
    public void ClearForm()
    {
        SpawnedQuestions.Clear();
        RectTransform content = scrollRect.content;
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);
    }

    /// <summary>Returns a summary of all answers.</summary>
    public List<(string question, AnswerState answer, string transcription)> GetResults()
    {
        var results = new List<(string, AnswerState, string)>();
        foreach (var item in SpawnedQuestions)
            results.Add((item.questionLabel.text, item.CurrentAnswer, item.transcription.text));
        return results;
    }

    // ------------------------------------------------------------------ //

    private void HandleAnswerChanged(QuestionItem item)
    {
        Debug.Log($"[FormGenerator] '{item.questionLabel.text}' -> {item.CurrentAnswer}");
    }
}