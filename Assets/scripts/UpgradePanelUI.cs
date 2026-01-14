using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelRoot;
    public Button damageButton;
    public Button hpButton;
    public TMP_Text damageCostText;
    public TMP_Text hpCostText;
    public TMP_Text essenceText;
    public TMP_Text damageMatText;
    public TMP_Text hpMatText;

    [Header("Refs")]
    public PlayerUpgrades upgrades;

    EssenceManager em;

    void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;
    }

    void OnEnable()
    {
        em = EssenceManager.Instance;

        if (upgrades == null)
            upgrades = FindFirstObjectByType<PlayerUpgrades>();

        if (em != null) em.EssenceChanged += OnEssenceChanged;
        if (MaterialsWallet.Instance != null) MaterialsWallet.Instance.Changed += OnMaterialsChanged;

        Debug.Log("[UpgradePanelUI] Enabled. Subscribed to Essence/Materials changes.");
        Refresh();
    }

    void OnDisable()
    {
        if (em != null) em.EssenceChanged -= OnEssenceChanged;
        if (MaterialsWallet.Instance != null) MaterialsWallet.Instance.Changed -= OnMaterialsChanged;

        Debug.Log("[UpgradePanelUI] Disabled. Unsubscribed.");
    }

    void OnEssenceChanged(int newValue)
    {
        Debug.Log($"[UpgradePanelUI] EssenceChanged -> {newValue}");
        Refresh();
    }

    void OnMaterialsChanged()
    {
        Debug.Log("[UpgradePanelUI] MaterialsChanged");
        Refresh();
    }

    public void Open()
    {
        Debug.Log("[UpgradePanelUI] Open()");
        panelRoot.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        Debug.Log("[UpgradePanelUI] Close()");
        panelRoot.SetActive(false);
    }

    // ✅ Вешай на кнопку Damage
    public void OnDamagePressed()
    {
        Debug.Log("[UpgradePanelUI] Damage button pressed");

        if (upgrades == null)
        {
            Debug.LogError("[UpgradePanelUI] upgrades == null");
            return;
        }
        if (EssenceManager.Instance == null)
        {
            Debug.LogError("[UpgradePanelUI] EssenceManager.Instance == null");
            return;
        }
        if (MaterialsWallet.Instance == null)
        {
            Debug.LogError("[UpgradePanelUI] MaterialsWallet.Instance == null");
            return;
        }

        int eCost = upgrades.GetDamageEssenceCost();
        int mCost = upgrades.GetDamageMatCost();
        var mat = upgrades.damageMaterial;

        Debug.Log($"[UpgradePanelUI] Trying UpgradeDamage: lvl={upgrades.damageLevel} -> lvl+1, cost={eCost} essence + {mCost} {mat} | haveEssence={EssenceManager.Instance.Essence} | haveMat={MaterialsWallet.Instance.Get(mat)}");

        bool ok = upgrades.UpgradeDamage();
        Debug.Log($"[UpgradePanelUI] UpgradeDamage result = {ok} | newLvl={upgrades.damageLevel} | essenceNow={EssenceManager.Instance.Essence} | {mat}Now={MaterialsWallet.Instance.Get(mat)}");

        Refresh();
    }

    // ✅ Вешай на кнопку HP
    public void OnHpPressed()
    {
        Debug.Log("[UpgradePanelUI] HP button pressed");

        if (upgrades == null)
        {
            Debug.LogError("[UpgradePanelUI] upgrades == null");
            return;
        }
        if (EssenceManager.Instance == null)
        {
            Debug.LogError("[UpgradePanelUI] EssenceManager.Instance == null");
            return;
        }
        if (MaterialsWallet.Instance == null)
        {
            Debug.LogError("[UpgradePanelUI] MaterialsWallet.Instance == null");
            return;
        }

        int eCost = upgrades.GetHpEssenceCost();
        int mCost = upgrades.GetHpMatCost();
        var mat = upgrades.hpMaterial;

        Debug.Log($"[UpgradePanelUI] Trying UpgradeHP: lvl={upgrades.hpLevel} -> lvl+1, cost={eCost} essence + {mCost} {mat} | haveEssence={EssenceManager.Instance.Essence} | haveMat={MaterialsWallet.Instance.Get(mat)}");

        bool ok = upgrades.UpgradeMaxHp();
        Debug.Log($"[UpgradePanelUI] UpgradeHP result = {ok} | newLvl={upgrades.hpLevel} | essenceNow={EssenceManager.Instance.Essence} | {mat}Now={MaterialsWallet.Instance.Get(mat)}");

        Refresh();
    }

    void Refresh()
    {
        int essence = (EssenceManager.Instance != null) ? EssenceManager.Instance.Essence : 0;
        if (essenceText) essenceText.text = essence.ToString();

        if (upgrades == null)
        {
            if (damageButton) damageButton.interactable = false;
            if (hpButton) hpButton.interactable = false;
            Debug.LogError("[UpgradePanelUI] Refresh: PlayerUpgrades not found -> buttons disabled");
            return;
        }

        int dmgE = upgrades.GetDamageEssenceCost();
        int dmgM = upgrades.GetDamageMatCost();
        var dmgMat = upgrades.damageMaterial;

        int hpE = upgrades.GetHpEssenceCost();
        int hpM = upgrades.GetHpMatCost();
        var hpMat = upgrades.hpMaterial;

        if (damageCostText) damageCostText.text = $"{dmgE} + {dmgM} {dmgMat}";
        if (hpCostText) hpCostText.text = $"{hpE} + {hpM} {hpMat}";

        bool canDmg = upgrades.CanUpgradeDamage();
        bool canHp = upgrades.CanUpgradeHp();

        if (damageButton) damageButton.interactable = canDmg;
        if (hpButton) hpButton.interactable = canHp;

        int haveDmgMat = (MaterialsWallet.Instance != null) ? MaterialsWallet.Instance.Get(dmgMat) : 0;
        int haveHpMat = (MaterialsWallet.Instance != null) ? MaterialsWallet.Instance.Get(hpMat) : 0;

        if (damageMatText) damageMatText.text = $"{dmgMat} {haveDmgMat}/{dmgM}";
        if (hpMatText) hpMatText.text = $"{hpMat} {haveHpMat}/{hpM}";

        Debug.Log($"[UpgradePanelUI] Refresh: essence={essence} | DMG cost={dmgE}+{dmgM}{dmgMat} haveMat={haveDmgMat} can={canDmg} | HP cost={hpE}+{hpM}{hpMat} haveMat={haveHpMat} can={canHp}");
    }
}
