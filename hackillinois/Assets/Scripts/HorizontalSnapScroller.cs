using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to the outer horizontal ScrollRect.
/// Snaps between sections on release and updates section dots.
///
/// Assign dotContainer and dotPrefab, then call InitDots() after GenerateForm().
/// FormGenerator handles this automatically.
/// </summary>
public class HorizontalSnapScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector] public ScrollRect    scrollRect;
    [HideInInspector] public int           pageCount;
    [HideInInspector] public RectTransform dotContainer;
    [HideInInspector] public GameObject    dotPrefab;

    public float snapSpeed = 10f;

    public Color dotActive   = Color.white;
    public Color dotInactive = new Color(1f, 1f, 1f, 0.35f);

    private int         _currentPage = 0;
    private bool        _dragging    = false;
    private bool        _snapping    = false;
    private float       _targetNorm  = 0f;
    private List<Image> _dots        = new List<Image>();

    // ------------------------------------------------------------------ //

    public void InitDots()
    {
        if (dotContainer == null || dotPrefab == null) return;

        foreach (Transform child in dotContainer) Destroy(child.gameObject);
        _dots.Clear();

        for (int i = 0; i < pageCount; i++)
        {
            GameObject d = Instantiate(dotPrefab, dotContainer);
            Image img = d.GetComponent<Image>();
            if (img != null)
            {
                img.color = i == 0 ? dotActive : dotInactive;
                _dots.Add(img);
            }
        }
    }

    // ------------------------------------------------------------------ //

    void Update()
    {
        if (_snapping && !_dragging)
        {
            float current = scrollRect.horizontalNormalizedPosition;
            float next    = Mathf.Lerp(current, _targetNorm, Time.deltaTime * snapSpeed);
            scrollRect.horizontalNormalizedPosition = next;

            if (Mathf.Abs(next - _targetNorm) < 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = _targetNorm;
                _snapping = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData _) => _dragging = true;

    public void OnEndDrag(PointerEventData _)
    {
        _dragging = false;
        SnapToNearest();
    }

    private void SnapToNearest()
    {
        if (pageCount <= 1) return;

        float norm     = scrollRect.horizontalNormalizedPosition;
        float pageNorm = 1f / (pageCount - 1);

        _currentPage = Mathf.RoundToInt(norm / pageNorm);
        _currentPage = Mathf.Clamp(_currentPage, 0, pageCount - 1);

        _targetNorm = _currentPage * pageNorm;
        _snapping   = true;

        UpdateDots(_currentPage);
    }

    private void UpdateDots(int activePage)
    {
        for (int i = 0; i < _dots.Count; i++)
            _dots[i].color = i == activePage ? dotActive : dotInactive;
    }
}
