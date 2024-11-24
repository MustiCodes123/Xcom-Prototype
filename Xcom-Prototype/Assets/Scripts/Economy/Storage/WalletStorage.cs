using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using UnityEngine;
using UnityEngine.Networking;

public enum GameCurrencies
{
    Gold,
    Gem,
    Energy,
    Key
}

public static class WalletStorage
{
    private static readonly int s_initialGoldAmount = 1000;
    private static readonly int s_initialGemsAmount = 100;
    private static readonly int s_initialEnergyAmount = 60;
    private static readonly int s_initialKeysAmount = 2;

    private static Dictionary<string, long> _goldBalances = new Dictionary<string, long>();
    private static Dictionary<string, long> _gemsBalances = new Dictionary<string, long>();
    private static Dictionary<string, long> _energyBalances = new Dictionary<string, long>();
    private static Dictionary<string, long> _keysBalances = new Dictionary<string, long>();

    #region Initialization
    public static async UniTask InitializeCurrencyData()
    {
        _goldBalances = new();
        _gemsBalances = new();
        _energyBalances = new();
        _keysBalances = new();

        List<InventoryItem> inventoryItems = await PlayerInventory.Instance.GetUserInventoryAsync();

        foreach (GameCurrencies currency in Enum.GetValues(typeof(GameCurrencies)))
        {
            string id = ShopHelper.ConvertCurrencyCodeToItemID(currency);
            InventoryItem currencyItem = inventoryItems.Find(item => item.Id == id);

            if (currencyItem != null)
            {
                SetCachedCurrency(currency, (long)currencyItem.Amount);
            }
            else
            {
                int initialCurrencyAmount = GetInitialAmount(currency);
                SetCachedCurrency(currency, initialCurrencyAmount);
                await AddCurrency(currency, initialCurrencyAmount);
            }
        }
    }
    #endregion

    #region Public Methods
    public static async Task<int> LoadCurrency(GameCurrencies currencyType)
    {
        string playerId = GameEconomy.s_PlayFabId;
        string currencyItemId = ShopHelper.ConvertCurrencyCodeToItemID(currencyType);
        int balance = 0;

        try
        {
            Dictionary<string, long> balances = GetBalanceDictionary(currencyType);

            List<InventoryItem> inventoryItems = await PlayerInventory.Instance.GetUserInventoryAsync();
            Debug.Log($"Inventory items count: {inventoryItems.Count}");

            InventoryItem currencyItem = inventoryItems.Find(item => item.Id == currencyItemId);

            if (currencyItem != null)
            {
                balance = (int)currencyItem.Amount;
                Debug.Log($"Currency item found. Balance: {balance}");
            }
            else
            {
                Debug.Log("Currency item not found in inventory");
                int initialCurrencyAmount = GetInitialAmount(currencyType);
                await AddCurrency(currencyType, initialCurrencyAmount);
                balance = initialCurrencyAmount;
                balances[playerId] = balance;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading currency: {ex.Message}");
        }

        return balance;
    }

    public static async Task SubtractCurrency(GameCurrencies currencyType, uint amount)
    {
        string currencyID = ShopHelper.ConvertCurrencyCodeToItemID(currencyType);
        string postUrl = $"https://{GameEconomy.s_TitleId}.playfabapi.com/Inventory/SubtractInventoryItems";

        UnityWebRequest www = new UnityWebRequest(postUrl, "POST");

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("X-EntityToken", GameEconomy.s_EntityToken);

        var requestBody = new
        {
            Entity = new { Type = "title_player_account", Id = GameEconomy.s_PlayerTitleId },
            Item = new { Id = currencyID, StackId = "default" },
            Amount = amount
        };

        string jsonBody = JsonConvert.SerializeObject(requestBody);
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        www.downloadHandler = new DownloadHandlerBuffer();

        try
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (www.error.Contains("InsufficientFunds"))
                {
                    Debug.LogWarning($"Insufficient funds to subtract {amount} {currencyType}.");
                }
                else
                {
                    Debug.LogError($"Error subtracting {currencyType}: {www.error}");
                    Debug.LogError($"Response: {www.downloadHandler.text}");
                }
            }
            else
            {
                Dictionary<string, long> balances = GetBalanceDictionary(currencyType);
                string playerId = GameEconomy.s_PlayFabId;
                balances[playerId] -= amount;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error subtracting {currencyType}: {ex.Message}");
        }
    }

    public static async Task AddCurrency(GameCurrencies currencyType, int amount)
    {
        string currencyID = ShopHelper.ConvertCurrencyCodeToItemID(currencyType);
        string postUrl = $"https://{GameEconomy.s_TitleId}.playfabapi.com/Inventory/AddInventoryItems";

        UnityWebRequest www = new UnityWebRequest(postUrl, "POST");

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("X-EntityToken", GameEconomy.s_EntityToken);

        var requestBody = new
        {
            Entity = new { Type = "title_player_account", Id = GameEconomy.s_PlayerTitleId },
            Item = new { Id = currencyID, StackId = "default" },
            Amount = amount
        };

        string jsonBody = JsonConvert.SerializeObject(requestBody);
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        www.downloadHandler = new DownloadHandlerBuffer();

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error adding Gold to inventory: {www.error}");
            Debug.LogError($"Response: {www.downloadHandler.text}");
        }
        else
        {
            Dictionary<string, long> balances = GetBalanceDictionary(currencyType);
            string playerId = GameEconomy.s_PlayFabId;

            if (!balances.ContainsKey(playerId))
            {
                balances[playerId] = 0;
            }

            balances[playerId] += amount;
        }
    }
    #endregion

    #region Cached Currency Methods
    private static int GetInitialAmount(GameCurrencies currency)
    {
        int initialCurrencyAmount;
        switch (currency)
        {
            case (GameCurrencies.Gold):
                initialCurrencyAmount = s_initialGoldAmount;
                break;
            case (GameCurrencies.Gem):
                initialCurrencyAmount = s_initialGemsAmount;
                break;
            case (GameCurrencies.Energy):
                initialCurrencyAmount = s_initialEnergyAmount;
                break;
            case (GameCurrencies.Key):
                initialCurrencyAmount = s_initialKeysAmount;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return initialCurrencyAmount;
    }

    private static Dictionary<string, long> GetBalanceDictionary(GameCurrencies currencyType)
    {
        switch (currencyType)
        {
            case GameCurrencies.Gold:
                return _goldBalances;
            case GameCurrencies.Gem:
                return _gemsBalances;
            case GameCurrencies.Energy:
                return _energyBalances;
            case GameCurrencies.Key:
                return _keysBalances;
            default:
                throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
        }
    }

    public static long GetCachedCurrency(GameCurrencies currencyType)
    {
        Dictionary<string, long> balances = GetBalanceDictionary(currencyType);
        string playerId = GameEconomy.s_PlayFabId;

        if (balances.ContainsKey(playerId))
        {
            return balances[playerId];
        }
        else
        {
            int initialCurrencyAmount = GetInitialAmount(currencyType);
            balances[playerId] = initialCurrencyAmount;
            return initialCurrencyAmount;
        }
    }

    public static void SetCachedCurrency(GameCurrencies currencyType, long amount)
    {
        Dictionary<string, long> balances = GetBalanceDictionary(currencyType);
        string playerId = GameEconomy.s_PlayFabId;
        balances[playerId] = amount;
    }

    public static async Task SyncCurrencyWithServer(GameCurrencies currencyType)
    {
        long localAmount = GetCachedCurrency(currencyType);
        long serverAmount = await LoadCurrency(currencyType);

        long difference = localAmount - serverAmount;

        if (difference > 0)
        {
            await AddCurrency(currencyType, (int)difference);
        }
        else if (difference < 0)
        {
            await SubtractCurrency(currencyType, (uint)Math.Abs(difference));
        }
    }

    public static async Task SyncCurrencies()
    {
        foreach(GameCurrencies currency in Enum.GetValues(typeof(GameCurrencies)))
        {
            await SyncCurrencyWithServer(currency);
        }
    }
    #endregion
}
