using UnityEngine;
using UnityEngine.UI;

public class BrushColorPicker : MonoBehaviour
{
    [System.Obsolete]
    void Update()
    {
        GameObject[] colorObjects = GameObject.FindGameObjectsWithTag("Color");

        foreach (GameObject colorObject in colorObjects)
        {
            RectTransform brushRT = GetComponent<RectTransform>();
            RectTransform colorRT = colorObject.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(colorRT, brushRT.position, GetUICamera()))
            {
                Image colorImg = colorObject.GetComponent<Image>();
                Image brushImg = GetComponent<Image>();

                if (colorImg != null && brushImg != null)
                {
                    brushImg.color = colorImg.color;
                }
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