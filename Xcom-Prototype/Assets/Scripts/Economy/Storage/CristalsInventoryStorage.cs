using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CristalsInventoryStorage
{
    private Dictionary<SummonCristalsEnum, Cristals> _cristalsMap = new Dictionary<SummonCristalsEnum, Cristals>();
    private PlayFab.EconomyModels.CatalogItem _cristalCatalogItem = new PlayFab.EconomyModels.CatalogItem();

    public CristalsInventoryStorage()
    {
        InitializeCristals();
    }

    #region Initialization
    private void InitializeCristals()
    {
        _cristalsMap[SummonCristalsEnum.Common] = new CommonCristals();
        _cristalsMap[SummonCristalsEnum.Rare] = new RareCristals();
        _cristalsMap[SummonCristalsEnum.Epic] = new EpicCristals();
        _cristalsMap[SummonCristalsEnum.Legendary] = new LegendaryCristals();
        _cristalsMap[SummonCristalsEnum.Mythical] = new MythicalCristals();
    }
    #endregion

    #region Public Methods
    public void UpdateSingleCristal(InventoryItem updatedItem)
    {
        foreach (var kvp in _cristalsMap)
        {
            SummonCristalsEnum rarity = kvp.Key;
            Cristals cristalInstance = kvp.Value;

            string cristalId = CristalsIds.GetCristalId(rarity);

            if (cristalId == updatedItem.Id)
            {
                cristalInstance.ItemInstanceId = updatedItem.Id;
                cristalInstance.Amount = (int)updatedItem.Amount;
                break;
            }
        }
    }

    public Cristals GetCristalsData(SummonCristalsEnum summonCristalsEnum)
    {
        if (_cristalsMap.TryGetValue(summonCristalsEnum, out Cristals cristals))
        {
            return cristals;
        }
        else
        {
            throw new ArgumentOutOfRangeException($"No cristals data found for rarity: {summonCristalsEnum}");
        }
    }

    public async Task<bool> TryRemoveCristals(SummonCristalsEnum summonCristalsEnum, int amount = 1)
    {
        Cristals cristals = GetCristalsData(summonCristalsEnum);

        if (cristals.Amount >= amount)
        {
            string cristalId = CristalsIds.GetCristalId(summonCristalsEnum);

            if (!string.IsNullOrEmpty(cristalId))
            {
                bool success = await GameEconomy.RemoveItem(cristalId, amount);

                if (success)
                {
                    cristals.Amount -= amount;
                    _cristalsMap[summonCristalsEnum] = cristals;
                    return true;
                }
            }
        }

        Debug.Log($"Not enough cristals in inventory");
        return false;
    }

    public void UpdateCristalInventory(List<InventoryItem> inventoryItems)
    {
        Dictionary<SummonCristalsEnum, Cristals> updatedCristalsMap = new Dictionary<SummonCristalsEnum, Cristals>();

        foreach (var kvp in _cristalsMap)
        {
            SummonCristalsEnum rarity = kvp.Key;
            Cristals cristalInstance = kvp.Value;

            string cristalId = CristalsIds.GetCristalId(rarity);

            if (!string.IsNullOrEmpty(cristalId))
            {
                InventoryItem cristalItem = inventoryItems.Find(item => item.Id == cristalId);

                if (cristalItem != null)
                {
                    cristalInstance.ItemInstanceId = cristalItem.Id;
                    cristalInstance.Amount = (int)cristalItem.Amount;
                }
                else
                {
                    cristalInstance.ItemInstanceId = string.Empty;
                    cristalInstance.Amount = 0;
                }
            }

            updatedCristalsMap[rarity] = cristalInstance;

            Debug.Log($"{rarity} cristals amount: {cristalInstance.Amount}");
        }

        _cristalsMap = updatedCristalsMap;
    }
    #endregion
}

public static class CristalsIds
{
    public static readonly Dictionary<SummonCristalsEnum, string> CristalsIdMap = new Dictionary<SummonCristalsEnum, string>
    {
        { SummonCristalsEnum.Common, "db320794-f747-450f-a5fd-28b63572d8ce" },      //Misty
        { SummonCristalsEnum.Rare, "1dc82e1e-1ce0-4ad5-84e4-b48f681a4f50" },        //Heaven
        { SummonCristalsEnum.Epic, "7012efb3-eeaa-48a6-add3-d3936ebfbf2f" },        //Lost
        { SummonCristalsEnum.Legendary, "bf32e443-69b5-4294-94c7-21fcd7a36430" },   //Eternal
        { SummonCristalsEnum.Mythical, "d1cabdf1-ea0f-4357-84cb-6f39ab3f69bd" }     //Divine
    };

    public static string GetCristalId(SummonCristalsEnum rarity)
    {
        if (CristalsIdMap.TryGetValue(rarity, out string cristalId))
        {
            return cristalId;
        }
        else
        {
            Debug.LogError($"No cristal ID for rarity: {rarity}");
            return null;
        }
    }
}

public abstract class Cristals
{
    public string ItemInstanceId { get; set; }
    public int Amount { get; set; }
}

public class CommonCristals : Cristals { }
public class RareCristals : Cristals { }
public class EpicCristals : Cristals { }
public class LegendaryCristals : Cristals { }
public class MythicalCristals : Cristals { }