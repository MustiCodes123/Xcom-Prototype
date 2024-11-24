using PlayFab.EconomyModels;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;

public class PurchaseManager
{
    public readonly PopupManager popupManager;
    public readonly IShopModelDataProvider dataProvider;
    public static PurchaseManager Instance { get; private set; }
    private bool _isPurchaseInProgress = false;

    public PurchaseManager(PopupManager popupManager, IShopModelDataProvider dataProvider)
    {
        this.popupManager = popupManager;
        this.dataProvider = dataProvider;
        Instance = this;
    }

    public bool PurchaseItem(CatalogItem item)
    {
        bool isEnoughMoney = true;

        string googlePlayId = item.AlternateIds.FirstOrDefault(id => id.Type == "GooglePlay")?.Value;
        if (!string.IsNullOrEmpty(googlePlayId))
        {
            Product product = IAPManager.Instance.GetProduct(googlePlayId);
            try
            {
                IAPManager.Instance.SetCurrentItem(item);
                IAPManager.Instance.BuyProduct(product.definition.id);
                return false; // Purchase will be handled in IAPManager
                //IAPManager.Instance.ProcessPurchase(null);
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogError($"Failed to purchase item: {e.Message}");
                popupManager.HidePurchaseConfirmationPopup();
                return false;
            }
        }
        else
        {
            foreach (KeyValuePair<GameCurrencies, int> kvp in GetItemPrices(item))
            {
                GameCurrencies currencyType = kvp.Key;
                uint price = (uint)kvp.Value;

                isEnoughMoney = Wallet.Instance.SpendCachedCurrency(currencyType, price);

                if (!isEnoughMoney)
                {
                    break;
                }
            }
        }

        if (isEnoughMoney)
        {
            foreach (var itemReference in item.ItemReferences)
            {
                CatalogItem rewardItem = dataProvider.GetItemById(itemReference.Id);

                if (rewardItem.Type == "currency")
                {
                    GameCurrencies rewardCurrency = ShopHelper.ConvertItemIDToCurrencyCode(itemReference.Id);
                    Wallet.Instance.AddCachedCurrency(rewardCurrency, (int)itemReference.Amount);
                }
                else
                {
                    ShopHelper.AddRewardToInventory(rewardItem);
                }
            }
            Debug.Log($"Item {item.Id} purchased successfully.");
            popupManager.HidePurchaseConfirmationPopup();
            return true;
        }
        else
        {
            popupManager.ShowNotEnoughPopup();
            return false;
        }
    }

    public void HandleTryPurchase(ShopItem shopItem)
    {
        popupManager.ShowPurchaseConfirmation(shopItem);
    }

    public void HandlePurchase(CatalogItem item)
    {
        bool purchaseResult = PurchaseItem(item);
    }

    public bool PurchaseGamePass(CatalogItem item)
    {
        bool isEnoughMoney = true;

        foreach (KeyValuePair<GameCurrencies, int> kvp in GetItemPrices(item))
        {
            GameCurrencies currencyType = kvp.Key;
            uint price = (uint)kvp.Value;

            isEnoughMoney = Wallet.Instance.SpendCachedCurrency(currencyType, price);

            if (!isEnoughMoney)
            {
                break;
            }
        }

        if (isEnoughMoney)
        {
            GameEconomy.SaveData(
                new Dictionary<string, string>
                {
                    { $"{item.Title["NEUTRAL"]}", "true" }
                },
                result =>
                {
                    Debug.Log("Game pass purchased and saved successfully.");
                },
                error =>
                {
                    Debug.LogError($"Failed to save game pass purchase: {error.ErrorMessage}");
                });

            return true;
        }
        else
        {
            popupManager.ShowNotEnoughPopup();
            return false;
        }
    }

    private Dictionary<GameCurrencies, int> GetItemPrices(CatalogItem item)
    {
        Dictionary<GameCurrencies, int> prices = new Dictionary<GameCurrencies, int>();

        foreach (var price in item.PriceOptions.Prices)
        {
            foreach (var amounts in price.Amounts)
            {
                string currencyID = amounts.ItemId;
                int amount = amounts.Amount;
                GameCurrencies currency = ShopHelper.ConvertItemIDToCurrencyCode(currencyID);

                if (prices.ContainsKey(currency))
                {
                    prices[currency] = amount;
                }
                else
                {
                    prices.Add(currency, amount);
                }
            }
        }

        return prices;
    }

    private GameCurrencies GetPurchaseCurrency(CatalogItem item)
    {
        foreach (var price in item.PriceOptions.Prices)
        {
            foreach (var amounts in price.Amounts)
            {
                string currencyID = amounts.ItemId;
                return ShopHelper.ConvertItemIDToCurrencyCode(currencyID);
            }
        }

        return GameCurrencies.Gold;
    }
}
