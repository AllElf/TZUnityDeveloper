using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector3 originalPosition;
    [SerializeField] Canvas canvas;
    [SerializeField] BrushPixelColorAutoSampler brushPixelColorAutoSampler;

    [System.Obsolete]
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        brushPixelColorAutoSampler = FindObjectOfType<BrushPixelColorAutoSampler>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null,
            out localPoint);

        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.localPosition = originalPosition;
        if (brushPixelColorAutoSampler != null) { brushPixelColorAutoSampler.DefaultColor(); }
    }
}