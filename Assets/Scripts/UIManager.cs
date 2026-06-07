using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text levelNameText;
    public Text objectiveText;
    public GameObject pausePanel;

    public void SetHud(string levelName, string objective)
    {
        if (levelNameText != null)
        {
            levelNameText.text = levelName;
        }

        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }
    }

    public void SetPauseVisible(bool visible)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(visible);
        }
    }

    public void Resume()
    {
        GameManager.Instance.SetPaused(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
