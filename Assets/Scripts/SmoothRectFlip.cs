using UnityEngine;
using System.Collections;

public class SmoothRectFlip : MonoBehaviour
{
    public RectTransform targetRect;          
    public float rotationDuration = 0.5f;    
    public float childSwapDelay = 0.5f;       

    private Coroutine currentCoroutine;
    private float lastYAngle = 0f;

    public void FlipToBack() => TryStartRotation(0f);
    public void FlipToNext() => TryStartRotation(180f);

    private void TryStartRotation(float targetYAngle)
    {
        float currentY = targetRect.localRotation.eulerAngles.y;

        if (!Mathf.Approximately(currentY, targetYAngle))
        {
            lastYAngle = targetYAngle;
            StartSmoothRotation(targetYAngle);
            StartCoroutine(DelayedChildSwap());

            if (targetRect.transform.parent != null)
            {
                int lastIndex = targetRect.transform.parent.childCount - 1;
                targetRect.SetSiblingIndex(lastIndex);
            }
        }
    }

    private void StartSmoothRotation(float targetYAngle)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(RotateY(targetYAngle));
    }

    private IEnumerator RotateY(float targetAngle)
    {
        Quaternion startRotation = targetRect.localRotation;
        Quaternion endRotation = Quaternion.Euler(0, targetAngle, 0);
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);
            targetRect.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        targetRect.localRotation = endRotation;
        currentCoroutine = null;
    }

    private IEnumerator DelayedChildSwap()
    {
        yield return new WaitForSeconds(childSwapDelay);

        if (targetRect.childCount < 2)
            yield break;

        Transform first = targetRect.GetChild(0);
        Transform second = targetRect.GetChild(1);

        int firstIndex = first.GetSiblingIndex();
        int secondIndex = second.GetSiblingIndex();

        first.SetSiblingIndex(secondIndex);
        second.SetSiblingIndex(firstIndex);
    }
}