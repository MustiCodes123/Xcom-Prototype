using PlayFab.EconomyModels;

public class PopupManager
{
    private readonly PurchaseConfirmPopUp _purchaseConfirmPopUp;
    private readonly NotEnoughPopUp _notEnoughPopUp;
    private readonly IShopModelDataProvider _dataProvider;

    public PopupManager(ShopPresenter shopPresenter, PurchaseConfirmPopUp purchaseConfirmPopUp, NotEnoughPopUp notEnoughPopUp, IShopModelDataProvider dataProvider)
    {
        _purchaseConfirmPopUp = purchaseConfirmPopUp;
        _notEnoughPopUp = notEnoughPopUp;
        _dataProvider = dataProvider;

        _notEnoughPopUp.Initialize(shopPresenter);
        _purchaseConfirmPopUp.Hide();
    }

    public void ShowPurchaseConfirmation(ShopItem purchaseItem)
    {
        _purchaseConfirmPopUp.Initialize(purchaseItem, _dataProvider);
        _purchaseConfirmPopUp.Show();
    }

    public void ShowNotEnoughPopup()
    {
        _purchaseConfirmPopUp.Hide();
        _notEnoughPopUp.Show();
    }

    public void HidePurchaseConfirmationPopup()
    {
        _purchaseConfirmPopUp.Hide();
    }
}
