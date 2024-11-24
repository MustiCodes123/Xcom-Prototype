using PlayFab.EconomyModels;
using TMPro;
using UnityEngine;

public abstract class TemporaryOfferItem : ShopItem
{
    [SerializeField] protected TMP_Text _descriptionTMP;

    [SerializeField] protected Transform _currencyRewardsContainer;
    [SerializeField] protected Transform _itemsRewardsContainer;

    [SerializeField] protected LimitedItemRewardView _itemRewardPrefab;
    [SerializeField] protected LimitedItemRewardView _currencyRewardPrefab;

    protected OfferSwitcher OfferSwitcher;

    public void Initialize(OfferSwitcher offerSwitcher, CatalogItem item, Wallet wallet, IShopModelDataProvider dataProvider, ShopPresenter presenter)
    {
        base.InitializeBase(item, wallet, dataProvider, presenter);
        this.OfferSwitcher = offerSwitcher;

        SetDetails();
        CreateRewards();
    }

    protected void OnEnable()
    {
        PurchaseButton.onClick.AddListener(OnTryPurchase);
    }

    protected void OnDisable()
    {
        PurchaseButton.onClick.RemoveListener(OnTryPurchase);
    }

    protected abstract void SetDetails();

    protected abstract void CreateRewards();
}