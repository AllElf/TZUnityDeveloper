using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] string nameScene = "SampleScene";
    [SerializeField] GameObject panelMenu;
    [SerializeField] AudioSource music;
    [SerializeField] bool soundActive;
    [SerializeField] GameObject soundImage;

    private void Start()
    {
        if (panelMenu != null)
        {
            panelMenu.SetActive(true);
        }

    }
    void Update()
    {
        Menu();
    }
    void Menu()
    {
        if (panelMenu != null && panelMenu.activeSelf)
        {
            Time.timeScale = 0.0f;
        }
        else if (panelMenu != null && !panelMenu.activeSelf)
        {
            Time.timeScale = 1.0f;
        }
    }
    public void Pause()
    {
        panelMenu.SetActive(true);
    }
    public void Play()
    {
        panelMenu.SetActive(false);
    }
    public void Sound()
    {
        soundActive = !soundActive;
        if (soundActive)
        {
            music.Pause();
            if (soundImage != null) { soundImage.SetActive(false); }
        }
        else if (!soundActive)
        {
            music.Play();
            if (soundImage != null) { soundImage.SetActive(true); }
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(nameScene);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
