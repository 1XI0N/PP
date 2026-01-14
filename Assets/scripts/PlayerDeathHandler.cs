using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Health health;
    [SerializeField] private PlayerDeathPopup deathPopup;

    private bool shown;

    void Awake()
    {
        if (!health) health = GetComponent<Health>();

        Debug.Log($"[PlayerDeathHandler] Awake. health={(health ? "OK" : "NULL")} popup={(deathPopup ? "OK" : "NULL")}", this);
    }

    void OnEnable()
    {
        Debug.Log("[PlayerDeathHandler] OnEnable -> subscribe", this);

        if (health != null)
            health.Died += OnDied;
        else
            Debug.LogError("[PlayerDeathHandler] Health is NULL. Повесь PlayerDeathHandler на объект где есть Health!", this);
    }

    void OnDisable()
    {
        Debug.Log("[PlayerDeathHandler] OnDisable -> unsubscribe", this);

        if (health != null)
            health.Died -= OnDied;
    }

    void Update()
    {
        // ТЕСТ: если нажать K — панель должна появиться
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("[PlayerDeathHandler] TEST KEY K -> Show popup", this);
            if (deathPopup != null) deathPopup.Show();
            else Debug.LogError("[PlayerDeathHandler] deathPopup is NULL (не назначен в инспекторе).", this);
        }
    }

    void OnDied()
    {
        Debug.Log("[PlayerDeathHandler] OnDied fired!", this);

        if (shown) return;
        shown = true;

        if (deathPopup != null) deathPopup.Show();
        else Debug.LogError("[PlayerDeathHandler] deathPopup is NULL (назначь ссылку в инспекторе).", this);

        StartCoroutine(AutoReturn());

        System.Collections.IEnumerator AutoReturn()
        {
            yield return new WaitForSecondsRealtime(2f); // важно: Realtime, т.к. Time.timeScale = 0
            if (deathPopup != null) deathPopup.ReturnToBase();
        }

    }

}
