using UnityEngine;
using TMPro;
using System; // For DateTime

public class ShowDateTimeTMP : MonoBehaviour
{
    public TMP_Text dateTimeText; // Drag your TextMeshPro text here in Inspector

    void Start()
    {
        // Get current system date & time
        DateTime now = DateTime.Now;

        // Set TMP text
        dateTimeText.text = now.ToString("yyyy-MM-dd");
    }
}