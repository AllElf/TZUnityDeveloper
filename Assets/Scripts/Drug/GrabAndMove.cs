using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GrabAndMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Основной объект")]
    [SerializeField] private GameObject mainObject;
    [SerializeField] private RectTransform handPositionAfterTaking;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Манипуляции с дочерним объектом")]
    [SerializeField] private Transform locationOfTheChildObject;
    [SerializeField] private float radiusStartPos = 50f;

    [Header("UI настройки")]
    [SerializeField] private string tagUI;

    private Camera uiCamera;
    private RectTransform canvasRect;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    private RectTransform mainRect;

    private RectTransform uiTarget;
    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Vector3 initialMainPosition;
    private Vector3 pickupLocalPos;
    private Vector3 handVelocity = Vector3.zero;
    [SerializeField] private Vector3 postReleaseTargetPosition;

    public bool hasTakenObject = false;
    private bool canFollowPointer = false;
    private bool isHolding = false;
    private bool returnZoneActive = false;
    private bool waitForMousePress = false;
    private bool isBusy = false;

    [System.Obsolete]
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        canvasRect = canvas.GetComponent<RectTransform>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        mainRect = mainObject.GetComponent<RectTransform>();

        initialMainPosition = mainRect.localPosition;
        mainObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBusy || hasTakenObject || isHolding) return;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(new PointerEventData(eventSystem) { position = eventData.position }, results);

        foreach (var rr in results)
        {
            GameObject go = rr.gameObject;
            if (!go.CompareTag(tagUI)) continue;

            uiTarget = go.GetComponent<RectTransform>();
            originalParent = uiTarget.parent;
            originalLocalPos = uiTarget.localPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, uiCamera, out Vector2 localPoint);
            pickupLocalPos = localPoint;
            returnZoneActive = true;

            mainObject.SetActive(true);
            StopAllCoroutines();
            isBusy = true;

            StartCoroutine(MoveTo(mainRect.localPosition, pickupLocalPos, () =>
            {
                uiTarget.SetParent(locationOfTheChildObject);
                uiTarget.localPosition = Vector3.zero;

                StartCoroutine(MoveTo(pickupLocalPos, handPositionAfterTaking.localPosition, () =>
                {
                    waitForMousePress = true;
                    hasTakenObject = true;
                    isBusy = false;
                }));
            }));

            break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        if (canFollowPointer && hasTakenObject)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, uiCamera, out Vector2 releasePoint))
            {
                postReleaseTargetPosition = new Vector3(releasePoint.x, releasePoint.y, 0f);
                float dist = Vector3.Distance(postReleaseTargetPosition, pickupLocalPos);

                if (returnZoneActive && dist < radiusStartPos)
                {
                    returnZoneActive = false;
                    uiTarget.SetParent(originalParent);
                    uiTarget.localPosition = originalLocalPos;

                    canFollowPointer = false;
                    hasTakenObject = false;
                    isHolding = false;
                    StopAllCoroutines();
                    StartCoroutine(ReturnHandToStart());
                }
            }
        }
    }

    void Update()
    {
        if (hasTakenObject)
        {
            if (waitForMousePress && Input.GetMouseButtonDown(0))
            {
                canFollowPointer = true;
                waitForMousePress = false;
            }

            if (canFollowPointer)
            {
                if (Input.GetMouseButton(0))
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, uiCamera, out Vector2 dragPoint))
                    {
                        Vector3 targetLocalPos = new Vector3(dragPoint.x, dragPoint.y, 0f);
                        mainRect.localPosition = Vector3.SmoothDamp(mainRect.localPosition, targetLocalPos, ref handVelocity, smoothTime);
                        postReleaseTargetPosition = targetLocalPos;
                        isHolding = true;
                        CheckReturnZone();
                    }
                }
                else
                {
                    mainRect.localPosition = Vector3.SmoothDamp(mainRect.localPosition, postReleaseTargetPosition, ref handVelocity, smoothTime);
                }
            }
            else if (waitForMousePress)
            {
                
                if (Vector3.Distance(mainRect.localPosition, handPositionAfterTaking.localPosition) > 0.05f)
                {
                    mainRect.localPosition = Vector3.SmoothDamp(mainRect.localPosition, handPositionAfterTaking.localPosition, ref handVelocity, smoothTime);
                }
                else
                {
                    mainRect.localPosition = handPositionAfterTaking.localPosition;
                    handVelocity = Vector3.zero;
                }
            }
        }
    }

    private void CheckReturnZone()
    {
        if (!returnZoneActive || uiTarget == null) return;

        float dist = Vector3.Distance(mainRect.localPosition, pickupLocalPos);
        if (dist < radiusStartPos)
        {
            returnZoneActive = false;
            uiTarget.SetParent(originalParent);
            uiTarget.localPosition = originalLocalPos;
            canFollowPointer = false;
            hasTakenObject = false;
            isHolding = false;
            waitForMousePress = false;
            StopAllCoroutines();
            StartCoroutine(ReturnHandToStart());
        }
    }

    private IEnumerator ReturnHandToStart()
    {
        yield return StartCoroutine(MoveTo(mainRect.localPosition, initialMainPosition, null));
        mainObject.SetActive(false);
        isBusy = false;
    }

    private IEnumerator MoveTo(Vector3 from, Vector3 to, System.Action onComplete)
    {
        while (Vector3.Distance(mainRect.localPosition, to) > 0.1f)
        {
            mainRect.localPosition = Vector3.Lerp(mainRect.localPosition, to, moveSpeed * Time.deltaTime);
            yield return null;
        }
        mainRect.localPosition = to;
        handVelocity = Vector3.zero; 
        onComplete?.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        if (canvasRect != null && returnZoneActive)
        {
            Gizmos.color = Color.cyan;
            Vector3 world = canvasRect.TransformPoint(pickupLocalPos);
            Gizmos.DrawWireSphere(world, radiusStartPos);
        }
    }
}
