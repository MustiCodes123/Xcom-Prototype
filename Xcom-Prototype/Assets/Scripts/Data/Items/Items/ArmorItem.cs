using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : BaseItem
{
    public float DodgeChance;
    public float BlockChance;
    public int Armor;
    public float ArmorIncreasePercent;
    public int Health;
    public float HealthIncreasePercent;
    public int Mana;
    public int ManaInPercents;
    public float PhysicalResistance;
    public float MagicalResistance;
    public int NextArmor;
    public int NextHealth;
    public int NextMana;
    public float ItemWeight;

    public void SetupLevel(ArmorUpgradeStats currentStats, ArmorUpgradeStats nextLevelStats)
    {
        DodgeChance = currentStats.DodgeChance;
        BlockChance = currentStats.BlockChance;
        Armor = currentStats.Armor;
        ArmorIncreasePercent = currentStats.ArmorIncreasePercent;
        Health = currentStats.Health;
        HealthIncreasePercent = currentStats.HealthIncreasePercent;
        Mana = currentStats.Mana;
        ManaInPercents = currentStats.ManaInPercents;
        PhysicalResistance = currentStats.PhysicalResistance;
        MagicalResistance = currentStats.MagicalResistance;

        if (nextLevelStats != null)
        {
            NextArmor = nextLevelStats.Armor;
            NextHealth = nextLevelStats.Health;
            NextMana = nextLevelStats.Mana;
        }

    }
}
