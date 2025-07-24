using UnityEngine;

public class ActiveObjectChecker : MonoBehaviour
{
    [SerializeField] GameObject cream;
    [SerializeField] GameObject great;
    [SerializeField] GameObject[] groupA;
    [SerializeField] GameObject[] groupB;
    [SerializeField] GameObject[] groupC;
    [SerializeField] AudioSource greatSound;
    [SerializeField] bool allConditionsMet;

    private bool hasPlayed = false;

    void Update()
    {
        allConditionsMet = IsAnyActive(groupA) &&IsAnyActive(groupB) &&IsAnyActive(groupC) &&cream != null && !cream.activeSelf;
        if (allConditionsMet)
        {
            if (great != null) great.SetActive(true);
            if (!hasPlayed && greatSound != null)
            {
                greatSound.Play();
                hasPlayed = true;
            }
        }
        else
        {
            if (great != null) great.SetActive(false);
            hasPlayed = false; 
        }
    }

    private bool IsAnyActive(GameObject[] group)
    {
        foreach (GameObject obj in group)
        {
            if (obj != null && obj.activeInHierarchy)
                return true;
        }
        return false;
    }
}