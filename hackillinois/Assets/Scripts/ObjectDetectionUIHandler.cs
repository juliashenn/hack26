using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.BuildingBlocks.AIBlocks;

public class ObjectDetectionUIHandler : MonoBehaviour
{
    public Transform contentParent;
    public GameObject listItemPrefab;

    public void UpdateUIList(List<BoxData> detections)
    {
        // Clear previous UI
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (detections == null || detections.Count == 0)
            return;

        // Sort by probability (assumes label format: "ObjectName 0.87")
        var sortedDetections = detections
            .OrderByDescending(d =>
            {
                var parts = d.label.Split(' ');
                if (parts.Length < 2) return 0f;
                if (float.TryParse(parts[^1], out float conf))
                    return conf;
                return 0f;
            })
            .ToList();

        foreach (var detection in sortedDetections)
        {
            GameObject item = Instantiate(listItemPrefab, contentParent);

            // Display label as-is (includes confidence)
            var text = item.GetComponent<TextMeshProUGUI>();
            text.text = detection.label;
        }
    }
}