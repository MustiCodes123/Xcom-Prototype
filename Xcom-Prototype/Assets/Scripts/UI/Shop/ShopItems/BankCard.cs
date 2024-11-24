using PlayFab.EconomyModels;

public class BankCard : ShopItem
{
    public override void Initialize(CatalogItem item, Wallet wallet, IShopModelDataProvider dataProvider, ShopPresenter presenter)
    {
        base.Initialize(item, wallet, dataProvider, presenter);

        PurchaseButton.onClick.RemoveListener(OnTryPurchase);
        PurchaseButton.onClick.AddListener(OnTryPurchase);
    }
}