using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragManager : MonoBehaviour
{
    private RectTransform grabbedObject;
    private Vector3 originalPosition;
    private bool dragging = false;

    [System.Obsolete]
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabObject();
        }

        if (Input.GetMouseButton(0) && grabbedObject != null && dragging)
        {
            Vector2 localPoint;
            RectTransform parentRT = grabbedObject.parent as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRT,
                Input.mousePosition,
                GetUICamera(),
                out localPoint))
            {
                grabbedObject.localPosition = localPoint;
            }
        }

        if (Input.GetMouseButtonUp(0) && grabbedObject != null)
        {
            dragging = false;
            grabbedObject.localPosition = originalPosition;
            grabbedObject = null;
        }
    }

    private void TryGrabObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            GameObject go = result.gameObject;

            if (!go.CompareTag("Drag"))
                continue;

            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                grabbedObject = rt;
                originalPosition = grabbedObject.localPosition;
                dragging = true;
                break;
            }
        }
    }

    [System.Obsolete]
    private Camera GetUICamera()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            return canvas.worldCamera;

        return null;
    }
}