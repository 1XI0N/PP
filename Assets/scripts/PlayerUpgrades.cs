using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CharacterStats stats;
    [SerializeField] private Health health;

    [Header("Upgrade Levels")]
    public int damageLevel = 0;
    public int hpLevel = 0;

    [Header("Upgrade Step")]
    public float damagePerLevel = 1f;
    public float hpPerLevel = 2f;

    [Header("Essence Costs")]
    public int damageBaseEssenceCost = 10;
    public int hpBaseEssenceCost = 10;
    public float costMultiplier = 1.35f;

    [Header("Material Costs")]
    public MaterialType damageMaterial = MaterialType.Metal;
    public MaterialType hpMaterial = MaterialType.Cloth;
    public int damageBaseMatCost = 2;
    public int hpBaseMatCost = 2;

    const string KEY_DMG = "UPG_DMG_LVL";
    const string KEY_HP = "UPG_HP_LVL";

    float _baseDamage = -1;
    float _baseMaxHp = -1;

    void Awake()
    {
        if (!stats) stats = GetComponent<CharacterStats>();
        if (!health) health = GetComponent<Health>();

        damageLevel = PlayerPrefs.GetInt(KEY_DMG, 0);
        hpLevel = PlayerPrefs.GetInt(KEY_HP, 0);

        CacheBaseStats();
        ApplyAllUpgrades();
    }

    void CacheBaseStats()
    {
        if (!stats) return;
        if (_baseDamage < 0) _baseDamage = stats.Damage;
        if (_baseMaxHp < 0) _baseMaxHp = stats.MaxHp;
    }

    public int GetDamageEssenceCost() => Mathf.RoundToInt(damageBaseEssenceCost * Mathf.Pow(costMultiplier, damageLevel));
    public int GetHpEssenceCost() => Mathf.RoundToInt(hpBaseEssenceCost * Mathf.Pow(costMultiplier, hpLevel));

    public int GetDamageMatCost() => Mathf.RoundToInt(damageBaseMatCost * Mathf.Pow(costMultiplier, damageLevel));
    public int GetHpMatCost() => Mathf.RoundToInt(hpBaseMatCost * Mathf.Pow(costMultiplier, hpLevel));

    public bool CanUpgradeDamage()
    {
        if (EssenceManager.Instance == null || MaterialsWallet.Instance == null) return false;
        return EssenceManager.Instance.Essence >= GetDamageEssenceCost()
            && MaterialsWallet.Instance.Has(damageMaterial, GetDamageMatCost());
    }

    public bool CanUpgradeHp()
    {
        if (EssenceManager.Instance == null || MaterialsWallet.Instance == null) return false;
        return EssenceManager.Instance.Essence >= GetHpEssenceCost()
            && MaterialsWallet.Instance.Has(hpMaterial, GetHpMatCost());
    }

    public bool UpgradeDamage()
    {
        if (!CanUpgradeDamage()) return false;

        int eCost = GetDamageEssenceCost();
        int mCost = GetDamageMatCost();

        if (!EssenceManager.Instance.Spend(eCost)) return false;
        if (!MaterialsWallet.Instance.Spend(damageMaterial, mCost))
        {
            // если вдруг материал не списалс€ Ч вернЄм эссенцию назад (страховка)
            EssenceManager.Instance.Add(eCost);
            return false;
        }

        damageLevel++;
        PlayerPrefs.SetInt(KEY_DMG, damageLevel);

        ApplyDamageUpgrade();
        return true;
    }

    public bool UpgradeMaxHp()
    {
        if (!CanUpgradeHp()) return false;

        int eCost = GetHpEssenceCost();
        int mCost = GetHpMatCost();

        if (!EssenceManager.Instance.Spend(eCost)) return false;
        if (!MaterialsWallet.Instance.Spend(hpMaterial, mCost))
        {
            EssenceManager.Instance.Add(eCost);
            return false;
        }

        hpLevel++;
        PlayerPrefs.SetInt(KEY_HP, hpLevel);

        ApplyHpUpgrade();
        return true;
    }

    void ApplyAllUpgrades()
    {
        ApplyDamageUpgrade();
        ApplyHpUpgrade();
    }

    void ApplyDamageUpgrade()
    {
        if (!stats) return;
        CacheBaseStats();
        stats.SetDamage(_baseDamage + damageLevel * damagePerLevel);
    }

    void ApplyHpUpgrade()
    {
        if (!stats) return;
        CacheBaseStats();
        stats.SetMaxHp(_baseMaxHp + hpLevel * hpPerLevel);

        if (health != null)
            health.ResetToMax();
    }
}
