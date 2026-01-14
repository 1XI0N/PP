using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CrystalClickReward : MonoBehaviour
{
    [SerializeField] private int essenceReward = 10;

    public void Click()
    {
        if (EssenceManager.Instance == null)
        {
            Debug.LogError("EssenceManager не найден в сцене!");
            return;
        }

        EssenceManager.Instance.Add(essenceReward);
        Debug.Log($"+{essenceReward} essence");
    }
}

