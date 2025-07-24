using UnityEngine;
using System.Collections.Generic;

public class TheObjectsUIArea : MonoBehaviour
{
    [SerializeField] string findTag = "Brush";
    [SerializeField] string nameTriggerObj;
    [SerializeField] GameObject mainObj;
    public bool entered;
    [SerializeField] GameObject[] taggedObjects;
    [SerializeField] Sponge sponge;
    [SerializeField] GameObject effect;

    HashSet<GameObject> enteredObjects = new HashSet<GameObject>();

    private void Start()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag(findTag);
        sponge = GameObject.FindAnyObjectByType<Sponge>();
        effect.SetActive(false);
    }
    void Update()
    {
        Rect myRect = GetWorldRect(mainObj.GetComponent<RectTransform>());
        foreach (GameObject obj in taggedObjects)
        {
            if (obj == mainObj || obj.GetComponent<RectTransform>() == null) continue;
            Rect otherRect = GetWorldRect(obj.GetComponent<RectTransform>());

            bool Got = myRect.Overlaps(otherRect);

            if (Got && !enteredObjects.Contains(obj))
            {
                enteredObjects.Add(obj);
                entered = true;
                if(obj.GetComponent<BrushPixelColorAutoSampler>() != null && sponge != null)
                {
                    nameTriggerObj = obj.GetComponent<BrushPixelColorAutoSampler>().currentColorName;
                    sponge.nameMakeup = nameTriggerObj;
                    if(effect != null) { effect.SetActive(true); }
                }
                //Debug.Log($"{obj.name} ס עודמל '{findTag}' ÂÏÅÐÂÛÅ ןמןאכ ג מבכאסעü {mainObj.name}");
            }
            else if (!Got && enteredObjects.Contains(obj))
            {
                if (sponge != null)
                {
                    nameTriggerObj = "";
                    sponge.nameMakeup = "";
                    if (effect != null) { effect.SetActive(false); }
                }
                entered = false;
                enteredObjects.Remove(obj);
            }
        }
    }

    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }
}