using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Level") && Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!IsPaused);
        }
    }

    public void StartGame()
    {
        SetPaused(false);
        SceneManager.LoadScene("Level1");
    }

    public void LoadMainMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene("MainMenu");
        UnlockCursorForMenus();
    }

    public void GameOver()
    {
        SetPaused(false);
        SceneManager.LoadScene("GameOver");
        UnlockCursorForMenus();
    }

    public void Victory()
    {
        SetPaused(false);
        SceneManager.LoadScene("Victory");
        UnlockCursorForMenus();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        bool gameplayScene = SceneManager.GetActiveScene().name.StartsWith("Level");
        Cursor.lockState = paused || !gameplayScene ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused || !gameplayScene;
        UIManager ui = FindAnyObjectByType<UIManager>();
        if (ui != null)
        {
            ui.SetPauseVisible(paused);
        }
    }

    private void UnlockCursorForMenus()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
