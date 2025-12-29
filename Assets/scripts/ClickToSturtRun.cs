using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToStartRun : MonoBehaviour
{
    public string runSceneName = "Run";

    void OnMouseDown()
    {
        if (ScoreManager.I != null)
            ScoreManager.I.ResetRun();

        Time.timeScale = 1f;
        SceneManager.LoadScene(runSceneName);
    }
}
