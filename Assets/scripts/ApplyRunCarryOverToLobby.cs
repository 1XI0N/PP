using System.Collections;
using UnityEngine;

public class ApplyRunCarryOverToLobby : MonoBehaviour
{
    public SoulsWallet lobbySouls;
    public Health playerHealth;
    public CharacterStats playerStats;

    IEnumerator Start()
    {
        // ждём кадр, чтобы все Start() у Player/Health/Upgrades успели выполниться
        yield return null;

        if (RunCarryOver.Instance == null) yield break;

        var data = RunCarryOver.Instance;

        // 1) души
        if (lobbySouls != null)
        {
            lobbySouls.ResetToZero();
            lobbySouls.Add(data.Souls);
        }

        // 2) maxHP (через stats)
        if (playerStats != null)
            playerStats.SetMaxHp(data.MaxHp);

        // 3) currentHP (через Health)
        if (playerHealth != null)
            playerHealth.ForceSetHp(data.Hp);

        // по желанию — очистить, чтобы второй раз не применялось
        // RunCarryOver.Instance.Clear();
    }
}
