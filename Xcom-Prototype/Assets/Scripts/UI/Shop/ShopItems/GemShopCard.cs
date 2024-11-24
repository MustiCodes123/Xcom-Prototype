using PlayFab.EconomyModels;

public class GemShopCard : ShopItem
{
    public override void Initialize(CatalogItem item, Wallet wallet, IShopModelDataProvider dataProvider, ShopPresenter presenter)
    {
        base.Initialize(item, wallet, dataProvider, presenter);

        PurchaseButton.onClick.RemoveListener(OnTryPurchase);
        PurchaseButton.onClick.AddListener(OnTryPurchase);
    }
}

[System.Serializable]
public class GemShopReward : BaseCustomData
{
}
