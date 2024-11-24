using PlayFab.EconomyModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ScrollsInventoryStorage : MonoBehaviour
{
    private Dictionary<ScrollsRarity, Scrolls> _scrollsMap = new Dictionary<ScrollsRarity, Scrolls>();

    public ScrollsInventoryStorage()
    {
        InitializeScrolls();
    }

    #region Initialization
    private void InitializeScrolls()
    {
        _scrollsMap[ScrollsRarity.Common] = new CommonScrolls();
        _scrollsMap[ScrollsRarity.Rare] = new RareScrolls();
        _scrollsMap[ScrollsRarity.Epic] = new EpicScrolls();
        _scrollsMap[ScrollsRarity.Legendary] = new LegendaryScrolls();
        _scrollsMap[ScrollsRarity.Mythical] = new MythicalScrolls();
    }
    #endregion

    #region Public Methods
    public void UpdateSingleScroll(InventoryItem updatedItem)
    {
        foreach (var kvp in _scrollsMap)
        {
            ScrollsRarity rarity = kvp.Key;
            Scrolls scrollInstance = kvp.Value;

            string scrollId = ScrollsIds.GetScrollId(rarity);

            if (scrollId == updatedItem.Id)
            {
                scrollInstance.ItemInstanceId = updatedItem.Id;
                scrollInstance.Amount = (int)updatedItem.Amount;
                break;
            }
        }
    }

    public Scrolls GetScrollsData(ScrollsRarity scrollsRarity)
    {
        if (_scrollsMap.TryGetValue(scrollsRarity, out Scrolls scrolls))
        {
            return scrolls;
        }
        else
        {
            throw new ArgumentOutOfRangeException($"No scrolls data found for rarity: {scrollsRarity}");
        }
    }

    public async Task<bool> TryRemoveScrolls(ScrollsRarity scrollsRarity, int amount = 1)
    {
        Scrolls scrolls = GetScrollsData(scrollsRarity);

        if (scrolls.Amount >= amount)
        {
            string scrollId = ScrollsIds.GetScrollId(scrollsRarity);

            if (!string.IsNullOrEmpty(scrollId))
            {
                bool success = await GameEconomy.RemoveItem(scrollId, amount);

                if (success)
                {
                    scrolls.Amount -= amount;
                    return true;
                }
            }
        }

        Debug.Log($"Not enough scrolls in inventory");
        return false;
    }

    public void UpdateScrollInventory(List<InventoryItem> inventoryItems)
    {
        foreach (var kvp in _scrollsMap)
        {
            ScrollsRarity rarity = kvp.Key;
            Scrolls scrollInstance = kvp.Value;

            string scrollId = ScrollsIds.GetScrollId(rarity);

            if (!string.IsNullOrEmpty(scrollId))
            {
                InventoryItem scrollItem = inventoryItems.Find(item => item.Id == scrollId);

                if (scrollItem != null)
                {
                    scrollInstance.ItemInstanceId = scrollItem.Id;
                    scrollInstance.Amount = (int)scrollItem.Amount;
                }
                else
                {
                    scrollInstance.ItemInstanceId = string.Empty;
                    scrollInstance.Amount = 0;
                }
            }

            Debug.Log($"{rarity} scrolls amount: {scrollInstance.Amount}");
        }
    }
    #endregion
}

public enum ScrollsRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythical
}

public static class ScrollsIds
{
    public static readonly Dictionary<ScrollsRarity, string> ScrollsIdMap = new Dictionary<ScrollsRarity, string>
    {
        { ScrollsRarity.Common, "48626961-553e-4087-b040-3586f6a0cc22" },
        { ScrollsRarity.Rare, "a9d3ad2e-d42b-434f-8408-7770de911310" },
        { ScrollsRarity.Epic, "39b247b9-cd65-4efd-a769-ab9b02ff2b6f" },
        { ScrollsRarity.Legendary, "18427fdb-5b1c-4280-9207-7b7e96338404" },
        { ScrollsRarity.Mythical, "60dfe7c3-90f8-4772-9fd7-1b2624d49aff" }
    };

    public static string GetScrollId(ScrollsRarity rarity)
    {
        if (ScrollsIdMap.TryGetValue(rarity, out string scrollId))
        {
            return scrollId;
        }
        else
        {
            Debug.LogError($"No scroll ID for rarity: {rarity}");
            return null;
        }
    }
}

public abstract class Scrolls
{
    public string ItemInstanceId { get; set; }
    public int Amount { get; set; }
}

public class CommonScrolls : Scrolls { }
public class RareScrolls : Scrolls { }
public class EpicScrolls : Scrolls { }
public class LegendaryScrolls : Scrolls { }
public class MythicalScrolls : Scrolls { }