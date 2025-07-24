using UnityEngine;

public class RadiusObjects : MonoBehaviour
{
    [SerializeField] RectTransform[] objectUI;
    [SerializeField] float radiusObject;
    [SerializeField] bool internalRadius;
    [SerializeField] bool externalRadius;

    void InternalRadius()
    {
        if (objectUI == null) return;
        for (int i = 0; i < objectUI.Length; i++)
        {
            float radiusX = (objectUI[i].rect.width * objectUI[i].lossyScale.x) / 2f;
            float radiusY = (objectUI[i].rect.height * objectUI[i].lossyScale.y) / 2f;
            radiusObject = Mathf.Max(radiusX, radiusY);
        }
    }
    void ExternalRadius()
    {
        if (objectUI == null) return;
        for (int i = 0; i < objectUI.Length; i++)
        {
            Vector3[] cornersOne = new Vector3[4];
            objectUI[i].GetWorldCorners(cornersOne);
            foreach (var cornerOne in cornersOne)
            {
                radiusObject = Vector3.Distance(cornerOne, objectUI[i].position);
                break;
            }
        }
    }
    private void Start()
    {
        if (internalRadius && !externalRadius)
        {
            InternalRadius();
        }
        else if (externalRadius && !internalRadius)
        {
            ExternalRadius();
        }
        else
        {
            bool pick = Random.Range(0, 2) == 0;
            internalRadius = pick;
            externalRadius = !pick;
            Debug.Log("Не выбран каой радиус объекта использовать, сработал рандом");
        }
    }
    private void OnDrawGizmos()
    {
        if(objectUI != null)
        {
            foreach (var obj in objectUI){if (obj == null) { return; }}
            Gizmos.color = Color.green;
            for (int i = 0; i < objectUI.Length; i++)
            {
                Gizmos.DrawWireSphere(objectUI[i].position, radiusObject);
            }
        }
        else { return; }
    }
}
