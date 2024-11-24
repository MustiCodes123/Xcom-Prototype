using UnityEngine;

public class WeaponItem : BaseItem
{
    public WeaponTypeEnum weaponType;
    public AttackType DamageType;
    public int minDamage;
    public int maxDamage;
    public float BlockChance;
    public float PhysicalResistance;
    public float MagicalResistance;
    public float CriticalChance;
    public float CriticalMultiplayer;
    public float attackSpeed;
    public float Health;
    public float HealthIncreasePercent;
    public int Armor;
    public float ArmorIncreasePercent;
    public int NextMinDamage;
    public int NextMaxDamage;
    public float NextAttackSpeed;
    public float ItemWeight;
    public int Mana;
    public int ManaInPercents;

    public bool IsEquipedInOffHand { get; set; }

    public void SetupLevel(WeaponUpgradeStats currentStats, WeaponUpgradeStats nextLevelStats)
    {
        minDamage = currentStats.minDamage;
        maxDamage = currentStats.maxDamage;
        BlockChance = currentStats.BlockChance;
        PhysicalResistance = currentStats.PhysicalResistance;
        MagicalResistance = currentStats.MagicalResistance;
        CriticalChance = currentStats.CriticalChance;
        CriticalMultiplayer = currentStats.CriticalMultiplayer;
        attackSpeed = currentStats.attackSpeed;
        Health = currentStats.health;
        HealthIncreasePercent = currentStats.HealthIncreasePercent;
        Armor = currentStats.armor;
        ArmorIncreasePercent = currentStats.ArmorIncreasePercent;
        ItemWeight = currentStats.ItemWeight;
        Mana = currentStats.mana;
        ManaInPercents = currentStats.ManaInPercents;

        if (nextLevelStats != null)
        {
            NextMinDamage = nextLevelStats.minDamage;
            NextMaxDamage = nextLevelStats.maxDamage;
            NextAttackSpeed = nextLevelStats.attackSpeed;
        }
    }
}


