using UnityEngine;

public class Sponge : MonoBehaviour
{
    public string nameMakeup;
    public GameObject[] allMakeup;
    [SerializeField] GameObject blush;
    private void Start()
    {
        Clear();
    }
    public void Clear()
    {
        if (allMakeup != null && blush != null)
        {
            for (int i = 0; i < allMakeup.Length; i++)
            {
                allMakeup[i].SetActive(false);
            }
            blush.SetActive(true);
        }
    }
    private void LateUpdate()
    {
        if (allMakeup != null && blush != null)
        {
            if (nameMakeup != "" && nameMakeup != "Cream")
            {
                for (int i = 0; i < allMakeup.Length; i++)
                {
                    if (allMakeup[i].name == nameMakeup)
                    {
                        allMakeup[i].SetActive(true);
                    }
                }
            }
            else if (nameMakeup == "Cream")
            {
                blush.SetActive(false);
            }
        }      
    }
}
