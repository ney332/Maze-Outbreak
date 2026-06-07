using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int levelNumber = 1;
    public string levelName = "Fase";

    private void Start()
    {
        UIManager ui = FindAnyObjectByType<UIManager>();
        if (ui != null)
        {
            ui.SetHud(levelName, "Encontre a saida sem ser pego pelos zumbis.");
        }
    }

    public void CompleteLevel()
    {
        if (levelNumber >= 4)
        {
            GameManager.Instance.Victory();
            return;
        }

        SceneManager.LoadScene("Level" + (levelNumber + 1));
    }
}
