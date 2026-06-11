using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public FadeImg fader;
    public void ClickAndLoad()
    {
        StartCoroutine(fader.FadeOut());
        Invoke("LoadGame", 0.5f);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("SC Demo");
    }
}