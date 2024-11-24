using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PlayFab.EconomyModels;
using UnityEngine.Networking;
using System.Text;
using Cysharp.Threading.Tasks;
using System;
using PlayFab;
using Zenject;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    public Action<ChestType, int> OnChestAmountChanged;
    public Action<SummonCristalsEnum, int> OnCristalAmountChanged;
    public Action<ScrollsRarity, int> OnScrollAmountChanged;

    private ChestsInventoryStorage _chestsInventoryStorage;
    private CristalsInventoryStorage _cristalsInventoryStorage;
    private ScrollsInventoryStorage _scrollsInventoryStorage;

    public List<InventoryItem> CachedInventory = new List<InventoryItem>();

    private float _inventoryUpdateInterval = 60f;
    private float _lastInventoryUpdateTime = 0f;

    public static PlayerInventory Instance { get; private set; }

    #region MonoBehaviour Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _chestsInventoryStorage = new ChestsInventoryStorage();
        _cristalsInventoryStorage = new CristalsInventoryStorage();
        _scrollsInventoryStorage = new ScrollsInventoryStorage();
    }

    private async void OnApplicationQuit()
    {
        await Wallet.Instance.SyncCurrencies();
    }
    #endregion

    #region Inventory Operations
    public async UniTask AuthnUpdateInventory()
    {
        if (Time.time - _lastInventoryUpdateTime >= _inventoryUpdateInterval || CachedInventory.Count == 0)
        {
            List<InventoryItem> inventoryItems = await GetUserInventoryAsync();

            if (inventoryItems != null)
            {
                CachedInventory = inventoryItems;
                _chestsInventoryStorage.UpdateChestInventory(CachedInventory);
                _cristalsInventoryStorage.UpdateCristalInventory(CachedInventory);
                _scrollsInventoryStorage.UpdateScrollInventory(CachedInventory);
                _lastInventoryUpdateTime = Time.time;
            }
            else
            {
                Debug.LogError("Failed to retrieve user inventory.");
            }
        }
        else
        {
            _chestsInventoryStorage.UpdateChestInventory(CachedInventory);
            _cristalsInventoryStorage.UpdateCristalInventory(CachedInventory);
            _scrollsInventoryStorage.UpdateScrollInventory(CachedInventory);
        }
    }
    
    public async Task UpdateInventory(bool forceUpdate = false)
    {
        if (forceUpdate || Time.time - _lastInventoryUpdateTime >= _inventoryUpdateInterval || CachedInventory.Count == 0)
        {
            List<InventoryItem> inventoryItems = await GetUserInventoryAsync();

            if (inventoryItems != null)
            {
                CachedInventory = inventoryItems;
                _chestsInventoryStorage.UpdateChestInventory(CachedInventory);
                _cristalsInventoryStorage.UpdateCristalInventory(CachedInventory);
                _scrollsInventoryStorage.UpdateScrollInventory(CachedInventory);
                _lastInventoryUpdateTime = Time.time;
            }
            else
            {
                Debug.LogError("Failed to retrieve user inventory.");
            }
        }
        else
        {
            _chestsInventoryStorage.UpdateChestInventory(CachedInventory);
            _cristalsInventoryStorage.UpdateCristalInventory(CachedInventory);
            _scrollsInventoryStorage.UpdateScrollInventory(CachedInventory);
        }
    }

    public async Task<List<InventoryItem>> GetUserInventoryAsync()
    {
        List<InventoryItem> inventoryItems = new List<InventoryItem>();
        string continuationToken = null;

        do
        {
            string getUrl = $"https://{GameEconomy.s_TitleId}.playfabapi.com/Inventory/GetInventoryItems";
            UnityWebRequest www = new UnityWebRequest(getUrl, "POST");
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("X-EntityToken", GameEconomy.s_EntityToken);

            string requestBody = continuationToken != null ? $"{{\"ContinuationToken\":\"{continuationToken}\"}}" : "{}";
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
            www.downloadHandler = new DownloadHandlerBuffer();

            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching user inventory: {www.error}");
                Debug.LogError($"Response: {www.downloadHandler.text}");
                return null;
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log($"Inventory response from PlayFab: {jsonResponse}");
                StringBuilder CrystalsDataForInventory = new();
                foreach (KeyValuePair<SummonCristalsEnum, string> crystalTypeData in CristalsIds.CristalsIdMap)
                {
                    if (!jsonResponse.Contains(crystalTypeData.Value))
                    {
                        CrystalsDataForInventory.Append($",{'{'}\"Id\":\"{crystalTypeData.Value}\",\"StackId\":\"default\",\"DisplayProperties\":{'{'}{'}'},\"Amount\":0,\"Type\":\"catalogItem\"{'}'}");
                    }
                }
                if (CrystalsDataForInventory.Length != 0)
                {
                    jsonResponse = jsonResponse.Insert(jsonResponse.LastIndexOf(']'), CrystalsDataForInventory.ToString());
                    jsonResponse = jsonResponse.Replace("[,", "[");
                }
                InventoryResponseWrapper responseWrapper = JsonConvert.DeserializeObject<InventoryResponseWrapper>(jsonResponse);
                GetInventoryItemsResponse response = responseWrapper.Data;

                if (response.Items != null)
                {
                    inventoryItems.AddRange(response.Items);
                }

                continuationToken = response.ContinuationToken;
            }
        }
        while (!string.IsNullOrEmpty(continuationToken));

        return inventoryItems;
    }

    public async Task UpdateSingleInventoryItem(string itemId, int addAmount = 0)
    {
        try
        {
            GetInventoryItemsResponse response = await GetSingleInventoryItemFromServer(itemId);

            if (response != null && response.Items != null && response.Items.Count > 0)
            {
                InventoryItem updatedItem = response.Items[0];
                Debug.Log($"Updated item: {updatedItem.Id}, Amount: {updatedItem.Amount}");

                int index = CachedInventory.FindIndex(item => item.Id == itemId);
                if (index != -1)
                {
                    CachedInventory[index] = updatedItem;
                    CachedInventory[index].Amount += addAmount;
                }
                else
                {
                    updatedItem.Amount += addAmount;
                    CachedInventory.Add(updatedItem);
                }

                if (ChestIds.ChestIdMap.ContainsValue(itemId))
                {
                    Debug.Log($"Updating ChestIdMap");
                    _chestsInventoryStorage.UpdateSingleChest(updatedItem, addAmount);
                }
                else if (CristalsIds.CristalsIdMap.ContainsValue(itemId))
                {
                    Debug.Log($"Updating CristalsIdMap");
                    _cristalsInventoryStorage.UpdateSingleCristal(updatedItem);
                }
                else if (ScrollsIds.ScrollsIdMap.ContainsValue(itemId))
                {
                    Debug.Log($"Updating ScrollsIdMap");
                    _scrollsInventoryStorage.UpdateSingleScroll(updatedItem);
                }

                InvokeItemUpdateEvent(itemId, (int)updatedItem.Amount);
            }
            else
            {
                Debug.LogWarning($"No item found with ID: {itemId}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating single inventory item {itemId}: {ex.Message}");
        }
    }

    private async Task<GetInventoryItemsResponse> GetSingleInventoryItemFromServer(string itemId)
    {
        string getUrl = $"https://{GameEconomy.s_TitleId}.playfabapi.com/Inventory/GetInventoryItems";
        UnityWebRequest www = new UnityWebRequest(getUrl, "POST");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("X-EntityToken", GameEconomy.s_EntityToken);

        string requestBody = "{}";

        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
        www.downloadHandler = new DownloadHandlerBuffer();

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching inventory items: {www.error}");
            return null;
        }

        string jsonResponse = www.downloadHandler.text;
        InventoryResponseWrapper responseWrapper = JsonConvert.DeserializeObject<InventoryResponseWrapper>(jsonResponse);

        if (responseWrapper.Data != null && responseWrapper.Data.Items != null)
        {
            responseWrapper.Data.Items = responseWrapper.Data.Items.Where(item => item.Id == itemId).ToList();
        }

        return responseWrapper.Data;
    }

    private void InvokeItemUpdateEvent(string itemId, int newAmount)
    {
        if (ChestIds.ChestIdMap.FirstOrDefault(x => x.Value == itemId).Key is ChestType chestType)
        {
            OnChestAmountChanged?.Invoke(chestType, newAmount);
        }
        else if (CristalsIds.CristalsIdMap.FirstOrDefault(x => x.Value == itemId).Key is SummonCristalsEnum cristalType)
        {
            OnCristalAmountChanged?.Invoke(cristalType, newAmount);
        }
        else if (ScrollsIds.ScrollsIdMap.FirstOrDefault(x => x.Value == itemId).Key is ScrollsRarity scrollType)
        {
            OnScrollAmountChanged?.Invoke(scrollType, newAmount);
        }
    }
    #endregion

    #region Chests Operations
    public Chests GetChestsData(ChestType weaponChestRarity)
    {
        return _chestsInventoryStorage.GetChestsData(weaponChestRarity);
    }

    public async Task<bool> TryRemoveChests(ChestType weaponChestRarity, int amount = 1)
    {
        bool success = await _chestsInventoryStorage.TryRemoveChests(weaponChestRarity, amount);

        if (success)
        {
            Chests chests = _chestsInventoryStorage.GetChestsData(weaponChestRarity);
            OnChestAmountChanged?.Invoke(weaponChestRarity, chests.Amount);
            await UpdateInventory();
        }

        return success;
    }

    public async Task<bool> AddChestToInventory(ChestType weaponChestRarity, int amount = 1)
    {
        var tcs = new TaskCompletionSource<bool>();
        string chestId = ChestIds.GetChestId(weaponChestRarity);

        InventoryItem chestItem = CachedInventory.Find(item => item.Id == chestId);
        chestItem.Amount += amount;

        if (!string.IsNullOrEmpty(chestId))
        {
            var getItemRequest = new GetItemRequest { Id = chestId };

            PlayFabEconomyAPI.GetItem(getItemRequest,
                (result) => OnGetItemSuccess(result, weaponChestRarity, amount, tcs),
                (error) => OnGetItemError(error, weaponChestRarity, tcs));

            _chestsInventoryStorage.UpdateChestInventory(CachedInventory);
            OnChestAmountChanged?.Invoke(weaponChestRarity, (int)chestItem.Amount);

            Chests chests = _chestsInventoryStorage.GetChestsData(weaponChestRarity);
            OnChestAmountChanged?.Invoke(weaponChestRarity, chests.Amount + amount);
            return await tcs.Task;
        }

        return false;
    }
    #endregion

    #region Cristals Operations
    public Cristals GetCristalsData(SummonCristalsEnum summonCristalsEnum)
    {
        return _cristalsInventoryStorage.GetCristalsData(summonCristalsEnum);
    }

    public async Task<bool> TryRemoveCristals(SummonCristalsEnum summonCristalsEnum, int amount = 1)
    {
        bool success = await _cristalsInventoryStorage.TryRemoveCristals(summonCristalsEnum, amount);

        if (success)
        {
            Cristals cristals = _cristalsInventoryStorage.GetCristalsData(summonCristalsEnum);
            OnCristalAmountChanged?.Invoke(summonCristalsEnum, cristals.Amount);
            await UpdateInventory(true);
        }

        return success;
    }

    public async Task<bool> AddCristalToInventory(SummonCristalsEnum summonCristalsEnum, int amount = 1)
    {
        var tcs = new TaskCompletionSource<bool>();
        string cristalId = CristalsIds.GetCristalId(summonCristalsEnum);

        InventoryItem cristalItem = CachedInventory.Find(item => item.Id == cristalId);
        cristalItem.Amount += amount;

        if (!string.IsNullOrEmpty(cristalId))
        {
            var getItemRequest = new GetItemRequest { Id = cristalId };

            PlayFabEconomyAPI.GetItem(getItemRequest,
                (result) => OnGetItemSuccess(result, summonCristalsEnum, amount, tcs),
                (error) => OnGetItemError(error, summonCristalsEnum, tcs));

            _cristalsInventoryStorage.UpdateCristalInventory(CachedInventory);

            return await tcs.Task;
        }

        return false;
    }
    #endregion

    #region Scrolls Operations
    public Scrolls GetScrollsData(ScrollsRarity summonScrollsEnum)
    {
        return _scrollsInventoryStorage.GetScrollsData(summonScrollsEnum);
    }

    public async Task<bool> TryRemoveScrolls(ScrollsRarity summonScrollsEnum, int amount = 1)
    {
        bool success = await _scrollsInventoryStorage.TryRemoveScrolls(summonScrollsEnum, amount);

        if (success)
        {
            Scrolls scrolls = _scrollsInventoryStorage.GetScrollsData(summonScrollsEnum);
            OnScrollAmountChanged?.Invoke(summonScrollsEnum, scrolls.Amount);
            await UpdateInventory();
        }

        return success;
    }

    public async Task<bool> AddScrollToInventory(ScrollsRarity summonScrollsEnum, int amount = 1)
    {
        var tcs = new TaskCompletionSource<bool>();
        string scrollId = ScrollsIds.GetScrollId(summonScrollsEnum);

        if (!string.IsNullOrEmpty(scrollId))
        {
            var getItemRequest = new GetItemRequest { Id = scrollId };

            PlayFabEconomyAPI.GetItem(getItemRequest,
                (result) => OnGetItemSuccess(result, summonScrollsEnum, amount, tcs),
                (error) => OnGetItemError(error, summonScrollsEnum, tcs));

            await UpdateInventory();

            return await tcs.Task;
        }

        return false;
    }
    #endregion

    #region Sets Operations
    public void AddSetToInventory(string itemReferenceID)
    {
        BaseItemsSet set = SetItemsContainer.Instance.GetSet(itemReferenceID);

        if (set != null)
        {
            AddItemToInventory(set.Helmet);
            AddItemToInventory(set.Chest);
            AddItemToInventory(set.Boots);
            AddItemToInventory(set.Gloves);

            switch (set)
            {
                case ShieldSet shieldSet:
                    AddItemToInventory(shieldSet.WeaponOH);
                    AddItemToInventory(shieldSet.Shield);
                    break;

                case PairWeaponsSet pairWeaponSet:
                    AddItemToInventory(pairWeaponSet.FirstWeaponOH);
                    AddItemToInventory(pairWeaponSet.SecondWeaponOH);
                    break;

                case TwoHandsWeaponsSet twoHandsSet:
                    AddItemToInventory(twoHandsSet.WeaponTH);
                    break;
            }
        }
    }

    private void AddItemToInventory(ItemTemplate item)
    {
        BaseItem itemInstance = ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item);
        _playerData.AddItemToInventory(itemInstance);
    }
    #endregion

    #region PlayFab Callbacks
    private void OnGetItemSuccess(GetItemResponse itemResult, ChestType weaponChestRarity, int amount, TaskCompletionSource<bool> tcs)
    {
        GameEconomy.AddCatalogItemToInventory(itemResult.Item, amount);
    }

    private void OnGetItemSuccess(GetItemResponse itemResult, SummonCristalsEnum summonCristalsEnum, int amount, TaskCompletionSource<bool> tcs)
    {
        GameEconomy.AddCatalogItemToInventory(itemResult.Item, amount);
    }

    private void OnGetItemSuccess(GetItemResponse itemResult, ScrollsRarity summonScrollsEnum, int amount, TaskCompletionSource<bool> tcs)
    {
        GameEconomy.AddCatalogItemToInventory(itemResult.Item, amount);
        UpdateInventory().ContinueWith(_ => tcs.SetResult(true));
    }

    private void OnGetItemError(PlayFabError error, ChestType weaponChestRarity, TaskCompletionSource<bool> tcs)
    {
        Debug.LogError($"Failed to load {weaponChestRarity}: {error.GenerateErrorReport()}");
        tcs.SetResult(false);
    }

    private void OnGetItemError(PlayFabError error, SummonCristalsEnum summonCristalsEnum, TaskCompletionSource<bool> tcs)
    {
        Debug.LogError($"Failed to load {summonCristalsEnum}: {error.GenerateErrorReport()}");
        tcs.SetResult(false);
    }

    private void OnGetItemError(PlayFabError error, ScrollsRarity summonScrollsEnum, TaskCompletionSource<bool> tcs)
    {
        Debug.LogError($"Failed to load {summonScrollsEnum}: {error.GenerateErrorReport()}");
        tcs.SetResult(false);
    }
    #endregion
}

[Serializable]
public class InventoryResponseWrapper
{
    public int Code;
    public string Status;
    public GetInventoryItemsResponse Data;
}