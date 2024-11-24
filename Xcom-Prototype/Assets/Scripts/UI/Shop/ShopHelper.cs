using Newtonsoft.Json;
using PlayFab;
using PlayFab.EconomyModels;
using System;
using System.Threading.Tasks;
using UnityEngine;

public static class ShopHelper
{
    public static string GetStoreIdByWindowType(WindowType windowType)
    {
        switch (windowType)
        {
            case WindowType.GamePass:
                return "a89915b2-1b44-4618-bd58-0fb555e2bbe6";
            case WindowType.Pack:
                return "e4f97fa6-7114-46f0-bace-5cbbc466ac10";
            case WindowType.Bank:
                return "ccdecca8-b69d-46b4-97ef-0db7293c0a48";
            case WindowType.GemShop:
                return "600663c0-ac55-46f9-aca9-e5d7bce4f442";
            case WindowType.Limited:
                return "c86f43e9-d871-46f8-9317-e137e75b8399";
            case WindowType.Daily:
                return "79d020b8-1f5d-4c6b-a026-7448f88e7f9d";

            default:
                throw new ArgumentOutOfRangeException(nameof(windowType), $"Unknown window windowType: {windowType}");
        }
    }

    public static string ConvertCurrencyCodeToItemID(GameCurrencies currencyCode)
    {
        switch (currencyCode)
        {
            case GameCurrencies.Gold:
                return "500af815-effb-40a0-8f0a-070d7ff0de3a";
            case GameCurrencies.Gem:
                return "c9771b59-8c64-47d4-89d9-58903d15397d";
            case GameCurrencies.Energy:
                return "b1b7f061-a95a-40be-8107-8fa24ded447c";
            case GameCurrencies.Key:
                return "3c84ff6e-335e-4ed5-80a3-d29a46f39b27";

            default:
                throw new ArgumentException($"Unrecognized currency code: {currencyCode}", nameof(currencyCode));
        }
    }

    public static GameCurrencies ConvertItemIDToCurrencyCode(string currencyID)
    {
        switch (currencyID)
        {
            case "500af815-effb-40a0-8f0a-070d7ff0de3a":
                return GameCurrencies.Gold;
            case "c9771b59-8c64-47d4-89d9-58903d15397d":
                return GameCurrencies.Gem;
            case "b1b7f061-a95a-40be-8107-8fa24ded447c":
                return GameCurrencies.Energy;

            default:
                throw new ArgumentException($"Unrecognized currencyID: {currencyID}", nameof(currencyID));
        }
    }

    public static T DeserializeCustomData<T>(CatalogItem item) where T : class
    {
        if (item.DisplayProperties != null)
        {
            var customDataJson = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer)
                                .SerializeObject(item.DisplayProperties);

            return JsonConvert.DeserializeObject<T>(customDataJson);
        }

        Debug.LogError("Error: DisplayProperties is null");

        return null;
    }

    public static int GetPriceFromCatalogItem(CatalogItem item)
    {
        int price = 0;

        foreach (var priceOption in item.PriceOptions.Prices)
        {
            foreach (var amount in priceOption.Amounts)
            {
                price = amount.Amount;

                break;
            }

            break;
        }

        return price;
    }

    public static async void AddRewardToInventory(CatalogItem item)
    {
        if (item.DisplayProperties == null)
        {
            Debug.LogError("Error: DisplayProperties is null");
            return;
        }
        
        try
        {
            var customDataJson = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer)
                .SerializeObject(item.DisplayProperties);

            GamePassCardRewardData gamePassRewardData = JsonConvert.DeserializeObject<GamePassCardRewardData>(customDataJson);

            if (gamePassRewardData != null && !string.IsNullOrEmpty(gamePassRewardData.ItemReferenceID))
            {
                await ProcessReward(gamePassRewardData.ItemReferenceID, gamePassRewardData.Amount);
            }
            else
            {
                await ProcessReward(item.Id, 1);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

    private static async Task ProcessReward(string itemReferenceID, int amount)
    {
        if (IsChestReward(itemReferenceID))
        {
            ChestType chestRarity = GetChestRarityFromItemReferenceID(itemReferenceID);
            await PlayerInventory.Instance.AddChestToInventory(chestRarity, amount);
        }
        else if (IsCristalReward(itemReferenceID))
        {
            SummonCristalsEnum cristalType = GetCristalRarityFromReferenceID(itemReferenceID);
            await PlayerInventory.Instance.AddCristalToInventory(cristalType, amount);
        }
        else if (IsItemsSetReward(itemReferenceID))
        {
            PlayerInventory.Instance.AddSetToInventory(itemReferenceID);
        }
        else
        {
            Debug.LogError($"Unknown reward type: {itemReferenceID}");
        }
    }

    private static bool IsCristalReward(string itemReferenceID)
    {
        return CristalsIds.CristalsIdMap.ContainsValue(itemReferenceID);
    }

    private static bool IsChestReward(string itemReferenceID)
    {
        return ChestIds.ChestIdMap.ContainsValue(itemReferenceID);
    }

    private static bool IsItemsSetReward(string itemReferenceID)
    {
        return SetsIds.SetsIdMap.ContainsValue(itemReferenceID);
    }

    private static ChestType GetChestRarityFromItemReferenceID(string itemReferenceID)
    {
        foreach (var kvp in ChestIds.ChestIdMap)
        {
            if (kvp.Value == itemReferenceID)
            {
                return kvp.Key;
            }
        }

        Debug.LogError($"No chest rarity found for item reference ID: {itemReferenceID}");
        return ChestType.Wooden;
    }

    private static SummonCristalsEnum GetCristalRarityFromReferenceID(string itemReferenceID)
    {
        foreach (var kvp in CristalsIds.CristalsIdMap)
        {
            if (kvp.Value == itemReferenceID)
            {
                return kvp.Key;
            }
        }

        Debug.LogError($"No cristal rarity found for item reference ID: {itemReferenceID}");
        return SummonCristalsEnum.Common;
    }
}