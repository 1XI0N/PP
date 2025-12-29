using UnityEngine;
using UnityEngine.SceneManagement;

public class StartRunButton : MonoBehaviour
{
    public string runSceneName = "Run";

    public void StartRun()
    {
        if (ScoreManager.I != null)
            ScoreManager.I.ResetRun();

        Time.timeScale = 1f;
        SceneManager.LoadScene(runSceneName);
    }
}
