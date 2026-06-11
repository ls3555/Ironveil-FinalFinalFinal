using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public FadeImg fader;
    public void ClickAndLoad()
    {
        StartCoroutine(fader.FadeOut());
        Invoke("LoadGame", 1f);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("SC Demo");
    }
}