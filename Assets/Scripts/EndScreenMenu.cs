using UnityEngine;

public class EndScreenMenu : MonoBehaviour
{
    public void PlayAgain()
    {
        EnsureGameManager().StartGame();
    }

    public void MainMenu()
    {
        EnsureGameManager().LoadMainMenu();
    }

    public void Quit()
    {
        EnsureGameManager().QuitGame();
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
