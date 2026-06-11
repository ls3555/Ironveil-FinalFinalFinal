using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameState currentState;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject confirmMenu;
    private GameObject menuScreen;
    public GameObject controlsScreen;
    private GameObject previousMenu;
    public AudioClip onClickClip;
    public AudioSource audioSource;

    public static PlayerInput Input;
    private InputAction menuAction;
    private InputAction pauseAction;
    public FadeImg fader;

    public enum GameState
    {
        Playing,
        Paused,
        Win,
        Lose,
    }

    private void Awake()
    {
        Input = new PlayerInput();
        pauseAction = Input.UI.Pause;
        menuAction = Input.UI.Menu;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        Input.UI.Enable();
        Input.Player.Enable();
    }

    void OnDisable()
    {
        Input.UI.Enable();
        Input.Player.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(fader.FadeIn());
        confirmMenu.SetActive(false);
        controlsScreen.SetActive(false);
        SetState(GameState.Playing);
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            Input.Player.Disable();
            SetState(GameState.Paused);
        }
        else if (currentState == GameState.Paused)
        {
            confirmMenu.SetActive(false);
            Input.Player.Enable();
            SetState(GameState.Playing);
        }
    }

    public void PlayerDied()
    {
        if (currentState != GameState.Playing)
            return;

        LoseGame();
    }

    public void WinGame()
    {
        SetState(GameState.Win);
    }

    void LoseGame()
    {
        SetState(GameState.Lose);
    }

    void SetState(GameState newState)
    {
        currentState = newState;
        bool isPlaying = newState == GameState.Playing;
        Time.timeScale = isPlaying ? 1f : 0f;

        gameScreen.SetActive(newState == GameState.Playing);
        pauseScreen.SetActive(newState == GameState.Paused);
        winScreen.SetActive(newState == GameState.Win);
        loseScreen.SetActive(newState == GameState.Lose);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Playing:
                if (pauseAction.WasPressedThisFrame())
                {
                    TogglePause();
                }
                break;

            case GameState.Paused:
                if (pauseAction.WasPressedThisFrame())
                {
                    TogglePause();
                }
                break;

            case GameState.Win:
            case GameState.Lose:
                if (menuAction.WasPressedThisFrame())
                {
                    RestartGame();
                }
                break;
        }
    }

    public void ClickAndLoadMainMenu()
    {
        audioSource.PlayOneShot(onClickClip);
        Time.timeScale = 1;
        Invoke("LoadMainMenu", 0.3f);
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMenuRoutine());
    }

    IEnumerator LoadMenuRoutine()
    {
        yield return StartCoroutine(fader.FadeOut());
        SceneManager.LoadScene("MainMenu");
    }

    public void GotoControlsScreen()
    {
        // remember where we came from
        if (pauseScreen.activeSelf) previousMenu = pauseScreen;
        else if (winScreen.activeSelf) previousMenu = winScreen;
        else if (loseScreen.activeSelf) previousMenu = loseScreen;
        else previousMenu = pauseScreen; // fallback

        pauseScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        confirmMenu.SetActive(false);

        controlsScreen.SetActive(true);
    }

    public void BackFromControls()
    {
        controlsScreen.SetActive(false);

        if (previousMenu != null)
        {
            previousMenu.SetActive(true);
        }
        else
        {
            pauseScreen.SetActive(true);
        }
    }

    public void BackToPauseMenu()
    {
        switch (currentState)
        {
            case GameState.Paused:
                confirmMenu.SetActive(false);
                pauseScreen.SetActive(true);
                break;
            case GameState.Lose:
                confirmMenu.SetActive(false);
                loseScreen.SetActive(true);
                break;
            case GameState.Win:
                confirmMenu.SetActive(false);
                winScreen.SetActive(true);
                break;
        }
    }
    public void GoToConfirmMenu()
    {
        switch (currentState)
        {
            case GameState.Paused:
                pauseScreen.SetActive(false);
                confirmMenu.SetActive(true);
                break;
            case GameState.Lose:
                loseScreen.SetActive(false);
                confirmMenu.SetActive(true);
                break;
            case GameState.Win:
                winScreen.SetActive(false);
                confirmMenu.SetActive(true);
                break;
        }
    }

    public void playClickSound()
    {
        audioSource.PlayOneShot(onClickClip);
    }

}
