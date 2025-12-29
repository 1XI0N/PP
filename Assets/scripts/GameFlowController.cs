using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowController : MonoBehaviour
{
    [Header("Refs")]
    public RoomFlowController roomFlow;
    public RoguelikeLevelGenerator generator;
    public RoomContentSpawner spawner;

    public Transform player;
    public PlayerHealth playerHealth;

    public RunChoicePopup popup;

    [Header("Scenes")]
    public string baseSceneName = "Lobby";

    [Header("Score")]
    public bool resetScoreOnGoToBase = true;

    bool isTransitioning;

    void Start()
    {
        if (roomFlow != null)
        {
            roomFlow.RoomCleared += OnRoomCleared;
            roomFlow.PlayerDied += OnPlayerDied;
        }
    }

    void OnDestroy()
    {
        if (roomFlow != null)
        {
            roomFlow.RoomCleared -= OnRoomCleared;
            roomFlow.PlayerDied -= OnPlayerDied;
        }
        Time.timeScale = 1f;
    }

    void OnRoomCleared(int roomId)
    {
        int score = (ScoreManager.I != null) ? ScoreManager.I.RunScore : 0;

        if (popup != null)
        {
            popup.Show(
                title: "Комната очищена! Нажми Continue и встань на свет.",
                score: score,
                continueAction: ContinueAfterClear,
                baseAction: GoToBase
            );
        }
    }

    void OnPlayerDied()
    {
        int score = (ScoreManager.I != null) ? ScoreManager.I.RunScore : 0;

        if (popup != null)
        {
            popup.Show(
                title: "Ты погиб. Continue = новый уровень.",
                score: score,
                continueAction: ContinueAfterDeath,
                baseAction: GoToBase
            );
        }
    }

    void ContinueAfterClear()
    {
        // Ничего не делаем — табличка сама закрылась, игрок идет на световой тайл
    }

    void ContinueAfterDeath()
    {
        // По умолчанию: новый уровень + восстановить HP
        StartNextLevel();
        if (playerHealth != null) playerHealth.RestoreFull();
    }

    public void StartNextLevel()
    {
        if (isTransitioning) return;
        if (generator == null) return;

        isTransitioning = true;

        // очистка старых врагов/объектов
        if (spawner != null) spawner.ResetForNewLevel();
        if (roomFlow != null) roomFlow.ResetForNewLevel();

        // новый seed и генерация
        generator.seed = 0;
        generator.Generate();

        // поставить игрока в центр первой/крупной комнаты — у нас генератор сам не ставит, поэтому сделаем так:
        if (player != null && generator.rooms != null && generator.rooms.Count > 0 && generator.ground != null)
        {
            // старт = самая большая комната
            int startId = 0;
            int bestArea = -1;
            for (int i = 0; i < generator.rooms.Count; i++)
            {
                int area = generator.rooms[i].Area;
                if (area > bestArea)
                {
                    bestArea = area;
                    startId = i;
                }
            }

            var c = generator.rooms[startId].center;
            player.position = generator.ground.GetCellCenterWorld(new Vector3Int(c.x, c.y, 0));
        }

        isTransitioning = false;
    }

    void GoToBase()
    {
        Time.timeScale = 1f;

        if (ScoreManager.I != null && resetScoreOnGoToBase)
            ScoreManager.I.ResetRun();

        SceneManager.LoadScene(baseSceneName);
    }

    public bool IsPopupOpen()
    {
        return popup != null && popup.root != null && popup.root.activeSelf;
    }
}
