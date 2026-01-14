using UnityEngine;

public class EnemySoulDrop : MonoBehaviour
{
    [SerializeField] private int soulsReward = 1;

    private bool rewarded;

    public void RewardSouls()
    {
        if (rewarded) return;
        rewarded = true;

        // »щем кошелЄк душ в сцене (обычно он в RunManagers)
        SoulsWallet wallet = FindFirstObjectByType<SoulsWallet>();
        if (wallet == null)
        {
            Debug.LogWarning("[EnemySoulDrop] SoulsWallet not found in scene!");
            return;
        }

        wallet.Add(soulsReward);
        Debug.Log($"[EnemySoulDrop] +{soulsReward} souls");
    }
}
