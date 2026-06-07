using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject levelSelectPanel;
    public GameObject creditsPanel;

    public void StartGame()
    {
        EnsureGameManager().StartGame();
    }

    public void OpenLevelSelect()
    {
        ShowOnly(levelSelectPanel);
    }

    public void OpenCredits()
    {
        ShowOnly(creditsPanel);
    }

    public void BackToMain()
    {
        ShowOnly(mainPanel);
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }

    public void QuitGame()
    {
        EnsureGameManager().QuitGame();
    }

    private void ShowOnly(GameObject panel)
    {
        if (mainPanel != null) mainPanel.SetActive(panel == mainPanel);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(panel == levelSelectPanel);
        if (creditsPanel != null) creditsPanel.SetActive(panel == creditsPanel);
    }

    private GameManager EnsureGameManager()
    {
        if (GameManager.Instance != null)
        {
            return GameManager.Instance;
        }

        return new GameObject("GameManager").AddComponent<GameManager>();
    }
}
