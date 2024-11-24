using Newtonsoft.Json;
using PlayFab.EconomyModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum SetsNames
{
    Test,
    Test1
}

public class SetsInventoryStorage
{
    private Dictionary<SetsNames, Sets> _setsMap = new Dictionary<SetsNames, Sets>();

    public SetsInventoryStorage()
    {
        InitializeSets();
    }

    #region Initialization
    private void InitializeSets()
    {
        _setsMap[SetsNames.Test] = new Sets();
        _setsMap[SetsNames.Test1] = new Sets();
    }
    #endregion

    #region Public Methods
    public Sets GetSetsData(SetsNames setRarity)
    {
        if (_setsMap.TryGetValue(setRarity, out Sets sets))
        {
            return sets;
        }
        else
        {
            throw new ArgumentOutOfRangeException($"No sets data found for setName: {setRarity}");
        }
    }

    public async Task<bool> TryRemoveSets(SetsNames setRarity, int amount = 1)
    {
        Sets sets = GetSetsData(setRarity);

        if (sets.Amount >= amount)
        {
            string setID = SetsIds.GetSetId(setRarity);

            if (!string.IsNullOrEmpty(setID))
            {
                bool success = await GameEconomy.RemoveItem(setID, amount);

                if (success)
                {
                    sets.Amount -= amount;
                    return true;
                }
            }
        }

        Debug.Log($"Not enough sets in inventory");
        return false;
    }

    public void UpdateSetsInventory(List<InventoryItem> inventoryItems)
    {
        foreach (var kvp in _setsMap)
        {
            SetsNames setName = kvp.Key;
            Sets setInstance = kvp.Value;

            string setID = SetsIds.GetSetId(setName);

            if (!string.IsNullOrEmpty(setID))
            {
                InventoryItem setItem = inventoryItems.Find(item => item.Id == setID);

                if (setItem != null)
                {
                    setInstance.ItemInstanceId = setItem.Id;
                    setInstance.Amount = (int)setItem.Amount;
                }
                else
                {
                    setInstance.ItemInstanceId = string.Empty;
                    setInstance.Amount = 0;
                }
            }

            Debug.Log($"{setName} sets amount: {setInstance.Amount}");
        }
    }
    #endregion

}

public class Sets
{
    public string ItemInstanceId { get; set; }
    public int Amount { get; set; }
}

public static class SetsIds
{
    public static readonly Dictionary<SetsNames, string> SetsIdMap = new Dictionary<SetsNames, string>
    {
        { SetsNames.Test, "854a20ab-0d05-48ff-83eb-de257d34efa7" },
        { SetsNames.Test1, "e8be1c1b-f8c5-460e-b7bd-f477d0e42635" },
    };

    public static string GetSetId(SetsNames rarity)
    {
        if (SetsIdMap.TryGetValue(rarity, out string chestId))
        {
            return chestId;
        }
        else
        {
            Debug.LogError($"No chest ID for setName: {rarity}");

            return null;
        }
    }
}
