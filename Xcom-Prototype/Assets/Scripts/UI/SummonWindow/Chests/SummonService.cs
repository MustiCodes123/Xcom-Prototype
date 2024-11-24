using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SummonService
{
    private List<ItemTemplate> _allItems;
    private ChestRarityDropChances _dropChances;

    public SummonService(List<ItemTemplate> allItems, ChestRarityDropChances dropChances)
    {
        _allItems = allItems;
        _dropChances = dropChances;
    }

    #region Public Methods
    public object Summon(ChestType chestType)
    {
        if (IsBrokenChest(chestType))
        {
            return SummonSingleItem(chestType);
        }
        else
        {
            return SummonSetItems(chestType);
        }
    }
    #endregion

    #region Private Methods
    private ItemTemplate SummonSingleItem(ChestType chestType)
    {
        ChestRarityDropChances.RarityDropChances rarityDropChances = GetBrokenRarityDropChances(chestType);
        ChestRarityDropChances.ItemTypeDropChances[] itemTypeDropChances = GetBrokenDropChances(chestType);

        RareEnum droppedRarity = GetRandomRarity(rarityDropChances);
        SlotEnum droppedItemType = GetRandomItemType(itemTypeDropChances);

        ItemTemplate droppedItem = GetRandomItem(droppedRarity, droppedItemType);
        return droppedItem;
    }

    private BaseItemsSet SummonSetItems(ChestType chestType)
    {
        if (IsExceptionalChest(chestType))
        {
            Chests chestData = PlayerInventory.Instance.GetChestsData(chestType);

            if (chestData is ExceptionalChests exceptionalChest && !string.IsNullOrEmpty(exceptionalChest.SetId))
            {
                return SetItemsContainer.Instance.GetSet(exceptionalChest.SetId);
            }
            else
            {
                Debug.LogError($"No SetId found for Exceptional chest type: {chestType}");
                return null;
            }
        }
        else
        {
            ChestRarityDropChances.ItemSetDropChances[] setDropChances = GetItemSetDropChances(chestType);
            ItemsSetRarity droppedSetPlayFabID = GetRandomSetRarity(setDropChances);

            return SetItemsContainer.Instance.GetRandomSetByRarity(droppedSetPlayFabID);
        }
    }

    private RareEnum GetRandomRarity(ChestRarityDropChances.RarityDropChances rarityDropChances)
    {
        float random = UnityEngine.Random.Range(0f, 100f);

        if (random < rarityDropChances.CommonDropChance)
        {
            return RareEnum.Common;
        }
        else if (random < rarityDropChances.CommonDropChance + rarityDropChances.RareDropChance)
        {
            return RareEnum.Rare;
        }
        else if (random < rarityDropChances.CommonDropChance + rarityDropChances.RareDropChance + rarityDropChances.EpicDropChance)
        {
            return RareEnum.Epic;
        }
        else if (random < rarityDropChances.CommonDropChance + rarityDropChances.RareDropChance + rarityDropChances.EpicDropChance + rarityDropChances.LegendaryDropChance)
        {
            return RareEnum.Legendary;
        }
        else
        {
            return RareEnum.Mythical;
        }
    }

    private SlotEnum GetRandomItemType(ChestRarityDropChances.ItemTypeDropChances[] itemTypeDropChances)
    {
        float random = UnityEngine.Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (var dropChance in itemTypeDropChances)
        {
            cumulativeChance += dropChance.DropChance;
            if (random < cumulativeChance)
            {
                return dropChance.ItemType;
            }
        }

        Debug.LogError("No item type was selected (which shouldn't happen if the drop chance sums to 100%), return the last item type in the array");
        return itemTypeDropChances[itemTypeDropChances.Length - 1].ItemType;
    }

    private ItemTemplate GetRandomItem(RareEnum rarity, SlotEnum itemType)
    {
        IEnumerable<ItemTemplate> possibleItems = _allItems.Where(item => item.Rare == rarity && item.Slot == itemType);
        return possibleItems.ElementAt(UnityEngine.Random.Range(0, possibleItems.Count()));
    }

    private ChestRarityDropChances.RarityDropChances GetBrokenRarityDropChances(ChestType chestRarity)
    {
        switch (chestRarity)
        {
            case ChestType.BrokenWooden:
                return _dropChances.BrokenWoodenChestDropChances;
            case ChestType.BrokenAncient:
                return _dropChances.BrokenAncientChestDropChances;
            case ChestType.BrokenKing:
                return _dropChances.BrokenKingChestDropChances;
            case ChestType.BrokenHero:
                return _dropChances.BrokenHeroChestDropChances;
            case ChestType.BrokenGod:
                return _dropChances.BrokenGodChestDropChances;
            default:
                throw new ArgumentOutOfRangeException(nameof(chestRarity), chestRarity, null);
        }
    }

    private ChestRarityDropChances.ItemTypeDropChances[] GetBrokenDropChances(ChestType chestRarity)
    {
        switch (chestRarity)
        {
            case ChestType.BrokenWooden:
                return _dropChances.BrokenWoodenChestItemsDropChances;
            case ChestType.BrokenAncient:
                return _dropChances.BrokenAncientChestItemsDropChances;
            case ChestType.BrokenKing:
                return _dropChances.BrokenKingChestItemsDropChances;
            case ChestType.BrokenHero:
                return _dropChances.BrokenHeroChestItemsDropChances;
            case ChestType.BrokenGod:
                return _dropChances.BrokenGodChestItemsDropChances;
            default:
                throw new ArgumentOutOfRangeException(nameof(chestRarity), chestRarity, null);
        }
    }

    private string GetRandomExceptionalChestSetPlayFabID()
    {
        int randomIndex = UnityEngine.Random.Range(0, _dropChances.ExceptionalChestSetPlayFabIDs.Length);
        return _dropChances.ExceptionalChestSetPlayFabIDs[randomIndex];
    }

    private bool IsExceptionalChest(ChestType chestRarity) => chestRarity == ChestType.Exceptional;

    private ChestRarityDropChances.ItemSetDropChances[] GetItemSetDropChances(ChestType chestRarity)
    {
        switch (chestRarity)
        {
            case ChestType.Wooden:
                return _dropChances.WoodenChestSetDropChances;
            case ChestType.Ancient:
                return _dropChances.AncientChestSetDropChances;
            case ChestType.King:
                return _dropChances.KingChestSetDropChances;
            case ChestType.Hero:
                return _dropChances.HeroChestSetDropChances;
            case ChestType.God:
                return _dropChances.GodChestSetDropChances;
            default:
                throw new ArgumentOutOfRangeException(nameof(chestRarity), chestRarity, null);
        }
    }

    private ItemsSetRarity GetRandomSetRarity(ChestRarityDropChances.ItemSetDropChances[] setDropChances)
    {
        float random = UnityEngine.Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (var dropChance in setDropChances)
        {
            cumulativeChance += dropChance.DropChance;
            if (random < cumulativeChance)
            {
                return dropChance.SetRarity;
            }
        }

        Debug.LogError("No set rarity was selected (which shouldn't happen if the drop chance sums to 100%), return the last set rarity in the array");
        return setDropChances[setDropChances.Length - 1].SetRarity;
    }

    private bool IsBrokenChest(ChestType chestType)
    {
        return chestType == ChestType.BrokenWooden ||
               chestType == ChestType.BrokenAncient ||
               chestType == ChestType.BrokenKing ||
               chestType == ChestType.BrokenHero ||
               chestType == ChestType.BrokenGod;
    }
    #endregion
}