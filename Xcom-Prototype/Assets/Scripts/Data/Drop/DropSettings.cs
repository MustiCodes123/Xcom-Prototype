using System;
using UnityEngine;

#region Enumerations
public enum DropItemType
{
    Axe,
    Bow,
    Dagger,
    Mace,
    Maul,
    Shield,
    Spear,
    Sword,
    Wand,
    THAxe,
    THMace,
    THMaul,
    THSword,
    ChestArmor,
    Gloves,
    Helmet,
    Legs,
    Craft
};
#endregion

#region Chance Classes
[Serializable]
public class DropItemChance
{
    [field: SerializeField] public DropItemType ItemType { get; private set; }
    [field: SerializeField] [field: Range(1, 15)] public int MinLevel { get; private set; }
    [field: SerializeField] [field: Range(1, 15)] public int MaxLevel { get; private set; }
    [field: SerializeField] [field: Range(0f, 100f)] public float CommonChance { get; set; }
    [field: SerializeField] [field: Range(0f, 100f)] public float RareChance { get; set; }
    [field: SerializeField] [field: Range(0f, 100f)] public float EpicChance { get; set; }
    [field: SerializeField] [field: Range(0f, 100f)] public float LegendaryChance { get; set; }
}

[Serializable]
public class DropCristalChance
{
    [field: SerializeField][field: Range(0f, 100f)] public float CommonChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float RareChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float EpicChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float LegendaryChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float MythicalChance { get; set; }
}

[Serializable]
public class DropBrokenChestChance
{
    [field: SerializeField][field: Range(0f, 100f)] public float CommonChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float RareChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float EpicChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float LegendaryChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float MythicalChance { get; set; }
}

[Serializable]
public class DropChestChance
{
    [field: SerializeField][field: Range(0f, 100f)] public float CommonChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float RareChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float EpicChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float LegendaryChance { get; set; }
    [field: SerializeField][field: Range(0f, 100f)] public float MythicalChance { get; set; }
}
#endregion

[Serializable]
[CreateAssetMenu(fileName = "DropSettings", menuName = "Data/Drop/DropSettings")]
public class DropSettings : ScriptableObject
{
    public DropItemChance[] DropItemChances;
    public DropCristalChance[] DropCristalChances;
    public DropChestChance[] DropChestChances;
    public DropBrokenChestChance[] DropBrokenChestChances;

    public CristalData CommonCristal;
    public CristalData RareCristal;
    public CristalData EpicCristal;
    public CristalData LegendaryCristal;
    public CristalData MythicalCristal;

    [SerializeField] private ChestViewData[] ChestsViewData;

    #region MonoBehaviour Methods
    private void OnValidate()
    {
        AdjustItemChancesValues();
        AdjustCristalChancesValues();
        AdjustBrokenChestChancesValues();
        AdjustChestChancesValues();
    }
    #endregion

    #region AdjustChances

    public float CalcDropItemTotalChance(DropItemChance chance)
    {
        return chance.CommonChance + chance.RareChance + chance.EpicChance + chance.LegendaryChance;
    }

    private void AdjustItemChancesValues()
    {
        for (int i = 0; i < DropItemChances.Length; i++)
        {
            DropItemChance chance = DropItemChances[i];
            float totalChance = CalcDropItemTotalChance(chance);

            if (totalChance > 100f)
            {
                float adjustedValue = 100f / totalChance;

                chance.CommonChance *= adjustedValue;
                chance.RareChance *= adjustedValue;
                chance.EpicChance *= adjustedValue;
                chance.LegendaryChance *= adjustedValue;
            }

            DropItemChances[i] = chance;
        }
    }

    public float CalcDropCristalTotalChance(DropCristalChance chance)
    {
        return chance.CommonChance + chance.RareChance + chance.EpicChance + chance.LegendaryChance + chance.MythicalChance;
    }

    private void AdjustCristalChancesValues()
    {
        for (int i = 0; i < DropCristalChances.Length; i++)
        {
            DropCristalChance chance = DropCristalChances[i];
            float totalChance = CalcDropCristalTotalChance(chance);

            if (totalChance > 100f)
            {
                float adjustedValue = 100f / totalChance;

                chance.CommonChance *= adjustedValue;
                chance.RareChance *= adjustedValue;
                chance.EpicChance *= adjustedValue;
                chance.LegendaryChance *= adjustedValue;
                chance.MythicalChance *= adjustedValue;
            }

            DropCristalChances[i] = chance;
        }
    }

    public float CalcDropBrokenChestTotalChance(DropBrokenChestChance chance)
    {
        return chance.CommonChance + chance.RareChance + chance.EpicChance + chance.LegendaryChance + chance.MythicalChance;
    }

    private void AdjustBrokenChestChancesValues()
    {
        for (int i = 0; i < DropBrokenChestChances.Length; i++)
        {
            DropBrokenChestChance chance = DropBrokenChestChances[i];
            float totalChance = CalcDropBrokenChestTotalChance(chance);

            if (totalChance > 100f)
            {
                float adjustedValue = 100f / totalChance;

                chance.CommonChance *= adjustedValue;
                chance.RareChance *= adjustedValue;
                chance.EpicChance *= adjustedValue;
                chance.LegendaryChance *= adjustedValue;
                chance.MythicalChance *= adjustedValue;
            }

            DropBrokenChestChances[i] = chance;
        }
    }

    public float CalcDropChestTotalChance(DropChestChance chance)
    {
        return chance.CommonChance + chance.RareChance + chance.EpicChance + chance.LegendaryChance + chance.MythicalChance;
    }

    private void AdjustChestChancesValues()
    {
        for (int i = 0; i < DropChestChances.Length; i++)
        {
            DropChestChance chance = DropChestChances[i];
            float totalChance = CalcDropChestTotalChance(chance);
            
            if (totalChance > 100f)
            {
                float adjustedValue = 100f / totalChance;

                chance.CommonChance *= adjustedValue;
                chance.RareChance *= adjustedValue;
                chance.EpicChance *= adjustedValue;
                chance.LegendaryChance *= adjustedValue;
                chance.MythicalChance *= adjustedValue;
            }

            DropChestChances[i] = chance;
        }
    }
    #endregion

    #region Utility Methods
    public ChestViewData GetChestViewDataByRarity(ChestType rarity)
    {
        foreach(ChestViewData chestViewData in ChestsViewData)
        {
            if (chestViewData.Rarity == rarity)
                return chestViewData;

            else continue;
        }

        Debug.Log($"No ChestViewData found for rarity: {rarity}");
        return null;
    }

    public ChestViewData[] GetChesViewData()
    {
        return ChestsViewData;
    }
    #endregion
}