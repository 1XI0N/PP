using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;

    [Header("Scene")]
    [SerializeField] private string lobbySceneName = "Lobby";

    void Awake()
    {
        if (!panelRoot) panelRoot = gameObject;
        panelRoot.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("[DeathPopup] Show()");
        panelRoot.SetActive(true);
        Time.timeScale = 0f; // пауза
    }

    public void Hide()
    {
        Debug.Log("[DeathPopup] Hide()");
        Time.timeScale = 1f;
        panelRoot.SetActive(false);
    }

    //  ЭТОТ МЕТОД вешай на кнопку "Return to Base"
    public void ReturnToBase()
    {
        Debug.Log("[DeathPopup] ReturnToBase() CLICKED");
        Time.timeScale = 1f; // ВАЖНО: вернуть время перед сменой сцены
        SceneManager.LoadScene(lobbySceneName);
    }
}
