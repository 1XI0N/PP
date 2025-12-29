using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        Time.timeScale = 1f; // на случай, если игра была на паузе
        SceneManager.LoadScene("Lobby");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // видно в Editor
    }
}
