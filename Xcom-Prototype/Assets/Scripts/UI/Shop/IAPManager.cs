using System;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using Zenject;
using System.Linq;
using PlayFab.EconomyModels;

public class IAPManager : MonoBehaviour, IStoreListener
{
    [Inject] private ShopRepository _shopRepository;
    private IStoreController controller;
    private IExtensionProvider extensions;
    private List<string> productIds = new List<string>();
    private static IAPManager _instance;
    public static IAPManager Instance => _instance ??= new IAPManager();
    private CatalogItem _currentPurchasingItem;

    public void InitializePurchasing(List<string> productIds)
    {
        if (IsInitialized())
            return;

        this.productIds = productIds;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var id in productIds)
            builder.AddProduct(id, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return controller != null && extensions != null;
    }

    public void BuyProduct(string productId)
    {
        if (!IsInitialized())
        {
            Debug.LogError("IAPManager is not initialized.");
            return;
        }

        Product product = controller.products.WithID(productId);

        if (product == null)
        {
            Debug.LogError($"Product {productId} not found.");
            return;
        }

        if (!product.availableToPurchase)
        {
            Debug.LogError($"Product {product.definition.id} is not available for purchase.");
            return;
        }

        controller.InitiatePurchase(product);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (_currentPurchasingItem == null)
        {
            Debug.LogError("Current purchasing item is null.");
            return PurchaseProcessingResult.Complete;
        }

        foreach (var itemReference in _currentPurchasingItem.ItemReferences)
        {
            CatalogItem rewardItem = PurchaseManager.Instance.dataProvider.GetItemById(itemReference.Id);

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

        _currentPurchasingItem = null;
        PurchaseManager.Instance.popupManager.HidePurchaseConfirmationPopup();
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase {product.definition.id} failed: {failureReason}");
        _currentPurchasingItem = null;
        PurchaseManager.Instance.popupManager.HidePurchaseConfirmationPopup();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAPManager initialization failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAPManager initialization failed: {error} - {message}");
    }

    public Product GetProduct(string productId)
    {
        if (IsInitialized())
            return controller.products.WithID(productId);
        return null;
    }

    public void SetCurrentItem(CatalogItem item)
    {
        if (_currentPurchasingItem != null)
        {
            throw new InvalidOperationException("Сurrent purchasing item is not null.");
        }

        _currentPurchasingItem = item;
    }
}
