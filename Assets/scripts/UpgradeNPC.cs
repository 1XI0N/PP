using UnityEngine;

public class UpgradeNPC : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject upgradePanel; // сам Panel (GameObject)

    void Start()
    {
        if (upgradePanel) upgradePanel.SetActive(false);
    }

    void OnMouseDown()
    {
        Debug.Log("[UpgradeNPC] clicked");

        if (upgradePanel == null)
        {
            Debug.LogError("[UpgradeNPC] upgradePanel is NULL");
            return;
        }

        upgradePanel.SetActive(true);
        Time.timeScale = 1f; // на всякий случай, если где-то оставался пауза-режим
    }
}
