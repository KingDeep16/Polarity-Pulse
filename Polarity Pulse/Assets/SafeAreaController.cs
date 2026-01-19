using UnityEngine;

public class SafeAreaController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    void Update()
    {
        // Continuously check for changes (like screen rotation)
        Refresh();
    }

    void Refresh()
    {
        Rect safeArea = Screen.safeArea;

        if (safeArea != lastSafeArea)
        {
            ApplySafeArea(safeArea);
        }
    }

    void ApplySafeArea(Rect r)
    {
        lastSafeArea = r;

        // Convert safe area rectangle from pixels to normalized anchors
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
