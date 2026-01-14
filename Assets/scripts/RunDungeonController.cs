using UnityEngine;
using UnityEngine.SceneManagement;

public class RunDungeonController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public RoomBoundsBuilder bounds;
    public EnemySpawnerProgression spawner;
    public SoulsWallet souls;

    [Header("UI")]
    public GameObject roomClearedPanel;
    public bool pauseOnPanel = true;

    [Header("Floor")]
    public int floor = 1;

    [Header("Scenes")]
    public string lobbySceneName = "Lobby";

    private bool floorCleared;

    void Awake()
    {
        if (!souls) souls = GetComponent<SoulsWallet>();
    }

    void Start()
    {
        if (roomClearedPanel) roomClearedPanel.SetActive(false);

        if (RunCarryOver.Instance != null)
            floor = Mathf.Max(1, RunCarryOver.Instance.NextFloor); // стартуем с сохранённого

        floorCleared = false;

        if (bounds) bounds.Build();

        if (spawner)
        {
            spawner.AllEnemiesDead += OnRoomCleared;
            SpawnCurrentFloor();
        }
    }


    void SpawnCurrentFloor()
    {
        if (!player || !spawner) return;
        spawner.SpawnForFloor(floor, player);
    }

    void OnRoomCleared()
    {
        floorCleared = true;
        ShowRoomClearedPanel();
    }

    void ShowRoomClearedPanel()
    {
        if (roomClearedPanel) roomClearedPanel.SetActive(true);
        if (pauseOnPanel) Time.timeScale = 0f;
    }

    void HideRoomClearedPanel()
    {
        if (pauseOnPanel) Time.timeScale = 1f;
        if (roomClearedPanel) roomClearedPanel.SetActive(false);
    }

    // Button: Continue
    public void OnContinuePressed()
    {
        HideRoomClearedPanel();
        NextFloor();
    }

    // Button: Return to Base
    public void OnReturnToBasePressed()
    {
        HideRoomClearedPanel();

        // сохраняем HP + души
        var hpComp = player ? player.GetComponent<Health>() : null;
        float hp = hpComp != null ? hpComp.CurrentHp : 1f;

        var stats = player ? player.GetComponent<CharacterStats>() : null;
        float maxHp = stats != null ? stats.MaxHp : 1f;

        int soulsAmount = souls != null ? souls.Souls : 0;

        if (RunCarryOver.Instance != null)
        {
            RunCarryOver.Instance.SetFromRun(soulsAmount, hp, maxHp);

            //  если этаж уже пройден (меню “комната очищена”), то следующий старт = floor+1
            //  если игрок ушёл посреди боя — стартуем с текущего floor
            int next = floorCleared ? (floor + 1) : floor;
            RunCarryOver.Instance.SetNextFloor(next);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbySceneName);
    }


    public void ReturnToLobby()
    {
        var hpComp = player ? player.GetComponent<Health>() : null;
        var stats = player ? player.GetComponent<CharacterStats>() : null;

        float hp = hpComp != null ? hpComp.CurrentHp : 1f;
        float maxHp = stats != null ? stats.MaxHp : 1f;
        int soulsAmount = souls != null ? souls.Souls : 0;

        if (RunCarryOver.Instance != null)
            RunCarryOver.Instance.SetFromRun(soulsAmount, hp, maxHp);

        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbySceneName);
    }


    public void NextFloor()
    {
        floor++;
        floorCleared = false;    // новый этаж ещё не пройден
        if (bounds) bounds.Build();
        SpawnCurrentFloor();
    }

}
