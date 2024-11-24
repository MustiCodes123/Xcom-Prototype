using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PlayFab.EconomyModels;
using System;
using PlayFab;
using System.Linq;
using Cysharp.Threading.Tasks;

public class ShopRepository
{
    public Dictionary<WindowType, List<CatalogItem>> Bundles { get; private set; }
    public Dictionary<string, List<CatalogItem>> ItemsFromBundles { get; private set; }
    public Dictionary<string, Sprite> DownloadedImages { get; private set; }

    public async UniTask LoadData()
    {
        Bundles = new Dictionary<WindowType, List<CatalogItem>>();
        ItemsFromBundles = new Dictionary<string, List<CatalogItem>>();
        DownloadedImages = new Dictionary<string, Sprite>();

        foreach (WindowType windowType in Enum.GetValues(typeof(WindowType)))
        {
            Bundles.Add(windowType, new List<CatalogItem>());
        }

        await InitializeStoreDataForAllCategories();
        InitializeIAPPurchasing();
    }

    private async Task InitializeStoreDataForAllCategories()
    {
        foreach (WindowType windowType in Enum.GetValues(typeof(WindowType)))
        {
            await InitializeStoreData(windowType);
            await DownloadImagesForItems(Bundles[windowType]);
        }
    }

    private async Task InitializeStoreData(WindowType windowType)
    {
        var tcs = new TaskCompletionSource<bool>();
        string storeId = ShopHelper.GetStoreIdByWindowType(windowType);

        var request = new SearchItemsRequest
        {
            Store = new StoreReference { Id = storeId },
            Select = "endDate, startDate",
        };

        void OnSearchItemsSuccess(SearchItemsResponse result)
        {
            foreach (var bundle in result.Items)
            {
                List<CatalogItem> itemsForSale = new();
                Bundles[windowType].Add(bundle);

                foreach (var itemReference in bundle.ItemReferences)
                {
                    var getItemRequest = new GetItemRequest
                    {
                        Id = itemReference.Id
                    };

                    void OnGetItemSuccess(GetItemResponse itemResult)
                    {
                        itemsForSale.Add(itemResult.Item);
                    }

                    void OnGetItemError(PlayFabError error)
                    {
                        Debug.LogError($"Failed to load item for {windowType}: {error.GenerateErrorReport()}");
                        tcs.SetResult(false);
                    }

                    PlayFabEconomyAPI.GetItem(getItemRequest, OnGetItemSuccess, OnGetItemError);
                }

                ItemsFromBundles.Add(bundle.Id, itemsForSale);
            }

            tcs.SetResult(true);
        }

        void OnSearchItemsError(PlayFabError error)
        {
            Debug.LogError($"Failed to load store items for {windowType}: {error.GenerateErrorReport()}");
            tcs.SetResult(false);
        }

        PlayFabEconomyAPI.SearchItems(request, OnSearchItemsSuccess, OnSearchItemsError);

        await tcs.Task;
    }

    private async Task DownloadImagesForItems(List<CatalogItem> items)
    {
        foreach (var item in items)
        {
            if (item.Images.Count > 0)
            {
                string imageURL = item.Images.FirstOrDefault().Url;
                if (!DownloadedImages.ContainsKey(imageURL))
                {
                    ShopLocalDatabase localDatabase = new ShopLocalDatabase(Application.persistentDataPath);
                    Sprite sprite = await localDatabase.GetOrDownloadImage(imageURL);
                    DownloadedImages[imageURL] = sprite;
                }
            }

            else continue;
        }
    }

    private void InitializeIAPPurchasing()
    {
        List<string> bundleStoreIds = new List<string>();
        foreach (WindowType windowType in Enum.GetValues(typeof(WindowType)))
        {
            foreach (var bundle in Bundles[windowType])
            {
                string storeID = bundle.AlternateIds.FirstOrDefault(id => id.Type == "GooglePlay")?.Value;
                if (storeID != null)
                {
                    Debug.Log($"Store ID: {storeID}");
                    bundleStoreIds.Add(storeID);
                }
            }
        }
        IAPManager.Instance.InitializePurchasing(bundleStoreIds);
    }
}