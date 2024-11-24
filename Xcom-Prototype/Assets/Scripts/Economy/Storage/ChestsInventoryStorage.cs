using Newtonsoft.Json;
using PlayFab;
using PlayFab.EconomyModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChestsInventoryStorage
{
    private Dictionary<ChestType, Chests> _chestsMap;
    private List<CatalogItem> _storedCatalogItems;

    public ChestsInventoryStorage()
    {
        InitializeChests();
    }

    #region Initialization
    private void InitializeChests()
    {
        _chestsMap = new Dictionary<ChestType, Chests>();
        _storedCatalogItems = new List<CatalogItem>();

        _chestsMap[ChestType.Wooden] = new WoodenChests();
        _chestsMap[ChestType.Ancient] = new AncientChests();
        _chestsMap[ChestType.King] = new KingChests();
        _chestsMap[ChestType.Hero] = new HeroChests();
        _chestsMap[ChestType.God] = new GodChests();
        _chestsMap[ChestType.BrokenWooden] = new BrokenWoodenChests();
        _chestsMap[ChestType.BrokenAncient] = new BrokenAncientChests();
        _chestsMap[ChestType.BrokenKing] = new BrokenKingChests();
        _chestsMap[ChestType.BrokenHero] = new BrokenHeroChests();
        _chestsMap[ChestType.BrokenGod] = new BrokenGodChests();
        _chestsMap[ChestType.Exceptional] = new ExceptionalChests();
    }
    #endregion

    #region Public Methods
    public void UpdateSingleChest(InventoryItem updatedItem, int addAmount = 0)
    {
        foreach (var kvp in _chestsMap)
        {
            ChestType rarity = kvp.Key;
            Chests chestInstance = kvp.Value;

            string chestId = ChestIds.GetChestId(rarity);

            if (chestId == updatedItem.Id)
            {
                chestInstance.ItemInstanceId = updatedItem.Id;
                chestInstance.Amount = (int)updatedItem.Amount + addAmount;

                if (updatedItem.DisplayProperties != null)
                {
                    string json = JsonConvert.SerializeObject(updatedItem.DisplayProperties);
                    ChestDisplayProperties displayProperties = JsonConvert.DeserializeObject<ChestDisplayProperties>(json);

                    if (displayProperties != null)
                    {
                        chestInstance.IsBroken = displayProperties.IsBroken;

                        if (chestInstance is ExceptionalChests setChest)
                        {
                            setChest.SetId = displayProperties.SetId;
                        }
                    }
                }

                break;
            }
        }
    }

    public Chests GetChestsData(ChestType weaponChestRarity)
    {
        if (_chestsMap.TryGetValue(weaponChestRarity, out Chests chests))
        {
            return chests;
        }
        else
        {
            throw new ArgumentOutOfRangeException($"No chests data found for rarity: {weaponChestRarity}");
        }
    }

    public async Task<bool> TryRemoveChests(ChestType weaponChestRarity, int amount = 1)
    {
        Chests chests = GetChestsData(weaponChestRarity);

        if (chests.Amount >= amount)
        {
            string chestId = ChestIds.GetChestId(weaponChestRarity);

            if (!string.IsNullOrEmpty(chestId))
            {
                bool success = await GameEconomy.RemoveItem(chestId, amount);

                if (success)
                {
                    chests.Amount -= amount;
                    return true;
                }
            }
        }

        Debug.Log($"Not enough chests in inventory");
        return false;
    }

    public async void UpdateChestInventory(List<InventoryItem> inventoryItems)
    {
        foreach (var kvp in _chestsMap)
        {
            ChestType rarity = kvp.Key;
            Chests chestInstance = kvp.Value;

            string chestId = ChestIds.GetChestId(rarity);

            if (!string.IsNullOrEmpty(chestId))
            {
                InventoryItem chestItem = inventoryItems.Find(item => item.Id == chestId);

                if (chestItem != null)
                {
                    CatalogItem catalogItem = await GetCatalogItem(chestItem.Id);
                    _storedCatalogItems.Add(catalogItem);

                    if (catalogItem != null && catalogItem.DisplayProperties != null)
                    {
                        string json = JsonConvert.SerializeObject(catalogItem.DisplayProperties);

                        ChestDisplayProperties displayProperties = JsonConvert.DeserializeObject<ChestDisplayProperties>(json);

                        if (displayProperties != null)
                        {
                            chestInstance.IsBroken = displayProperties.IsBroken;

                            if (chestInstance is ExceptionalChests setChest)
                            {
                                setChest.SetId = displayProperties.SetId;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to deserialize DisplayProperties for {rarity} chest");
                        }
                    }

                    chestInstance.ItemInstanceId = chestItem.Id;
                    chestInstance.Amount = (int)chestItem.Amount;
                }
                else
                {
                    chestInstance.ItemInstanceId = string.Empty;
                    chestInstance.Amount = 0;
                }
            }
        }
    }

    private async Task<CatalogItem> GetCatalogItem(string itemId, int maxRetries = 3)
    {
        if(_storedCatalogItems != null)
        {
            foreach (CatalogItem item in _storedCatalogItems)
            {
                if(item.Id == itemId)
                {
                    Debug.Log($"Returning: {item.Title["NEUTRAL"]} from local storage");
                    return item;
                }
            }
        }
        else
        {
            Debug.LogError($"List is null");
        }

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                CatalogItem result = new CatalogItem();
                var getItemRequest = new GetItemRequest { Id = itemId };
                var tcs = new TaskCompletionSource<CatalogItem>();

                PlayFabEconomyAPI.GetItem(getItemRequest,
                    result => tcs.SetResult(result.Item),
                    error =>
                    {
                        if (error.Error == PlayFabErrorCode.APIConcurrentRequestLimitExceeded)
                        {
                            Debug.LogWarning($"Rate limit exceeded, retrying... Attempt {attempt + 1} of {maxRetries}");
                        }
                        else
                        {
                            Debug.LogError($"Failed to get catalog item {itemId}: {error.GenerateErrorReport()}");
                            tcs.SetResult(null);
                        }
                    }
                );

                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
                var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                if (completedTask == tcs.Task)
                {
                    return await tcs.Task;
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error in GetCatalogItem: {ex}");
            }
        }

        Debug.LogError($"Failed to get catalog item {itemId} after {maxRetries} attempts");
        return null;
    }
    #endregion
}

#region Chests Data Classes
public abstract class Chests
{
    public string ItemInstanceId { get; set; }
    public int Amount { get; set; }
    public bool IsBroken { get; set; }
}

[System.Serializable]
public class ChestDisplayProperties
{
    public bool IsBroken { get; set; }
    public string SetId { get; set; }
}

public class WoodenChests : Chests { }
public class AncientChests : Chests { }
public class KingChests : Chests { }
public class HeroChests : Chests { }
public class GodChests : Chests { }
public class BrokenWoodenChests : Chests { }
public class BrokenAncientChests : Chests { }
public class BrokenKingChests : Chests { }
public class BrokenHeroChests : Chests { }
public class BrokenGodChests : Chests { }
public class ExceptionalChests : Chests { public string SetId { get; set; } }

public static class ChestIds
{
    public static readonly Dictionary<ChestType, string> ChestIdMap = new Dictionary<ChestType, string>
    {
        { ChestType.Wooden, "3d804228-1b50-40fd-8170-90afe3fa2e4b" },
        { ChestType.Ancient, "ce236fca-4a53-4772-bce7-8e2e0cfd6063" },
        { ChestType.King, "b5c5573f-d0fd-4db3-9d2b-58745143bd7d" },
        { ChestType.Hero, "32a47906-5e6c-4a8a-8142-5c7eda04eaba" },
        { ChestType.God, "f7125f79-701c-45da-8f06-92c1cdef381d" },
        { ChestType.BrokenWooden, "2e8cf045-0380-466c-8664-e33769542e9a" },
        { ChestType.BrokenAncient, "d36c5bc6-3eb6-4ea9-bff5-7edef302f990" },
        { ChestType.BrokenKing, "b23da2d7-cc84-4f9d-b413-1bd2eef22954" },
        { ChestType.BrokenHero, "29a82ef0-d215-4952-bac7-41c9c086ee0f" },
        { ChestType.BrokenGod, "0c785184-795a-4bd2-a496-d72b6e306b55" },
        { ChestType.Exceptional, "8d544383-e21c-4dc2-993f-0da9da8f9331" },
    };

    public static string GetChestId(ChestType rarity)
    {
        if (ChestIdMap.TryGetValue(rarity, out string chestId))
        {
            return chestId;
        }
        else
        {
            Debug.LogError($"No chest ID for rarity: {rarity}");

            return null;
        }
    }
}
#endregion