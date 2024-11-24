using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ShopModel))]
[RequireComponent(typeof(ShopView))]
public class ShopPresenter : UIWindowView
{
    [Inject] private readonly PlayerData _playerData;
    [Inject] private readonly OfferSwitcher _offerSwitcher;
    [Inject] private readonly ShopModel _model;
    [Inject] private readonly ShopView _view;
    [Inject] private readonly PurchaseConfirmPopUp _purchaseConfirmPopUp;
    [Inject] private readonly NotEnoughPopUp _notEnoughPopUp;

    public PurchaseManager PurchaseManager;

    private PopupManager _popupManager;

    public PlayerData PlayerData => _playerData;

    private IShopModelDataProvider _dataProvider;

    public ShopPresenter(
        PlayerData playerData,
        OfferSwitcher offerSwitcher,
        ShopModel model,
        ShopView view,
        PurchaseConfirmPopUp purchaseConfirmPopUp,
        NotEnoughPopUp notEnoughPopUp)
    {
        _playerData = playerData;
        _offerSwitcher = offerSwitcher;
        _model = model;
        _view = view;
        _purchaseConfirmPopUp = purchaseConfirmPopUp;
        _notEnoughPopUp = notEnoughPopUp;      
    }

    #region MonoBeaviour Methods
    protected override void Awake()
    {
        base.Awake();

        InitializeShop();        
        _popupManager = new PopupManager(this, _purchaseConfirmPopUp, _notEnoughPopUp, _dataProvider);
        PurchaseManager = new PurchaseManager(_popupManager, _dataProvider);           

        closeButton.onClick.AddListener(Hide);
    }

    private void OnEnable()
    {
        _view.UpdateCategoryButtons(1); // First category selected by default   
        _view.DisplaySelectedCategory(1, WindowType.Pack);
        _view.InitializeCurrenciesView();

        _purchaseConfirmPopUp.Purchase += PurchaseManager.HandlePurchase;
    }

    private void OnDisable()
    {
        _purchaseConfirmPopUp.Purchase -= PurchaseManager.HandlePurchase;
    }
    #endregion

    #region Initialization
    public void InitializeShop()
    {
        _model.Initialize();
        _dataProvider = new ShopModelData(_model);
        _view.Initialize(this, _offerSwitcher, _dataProvider);
        
    }
    #endregion

    #region View Methods
    public void OnCategorySelected(int ID, WindowType windowType)
    {
        _view.UpdateCategoryButtons(ID);
        _view.DisplaySelectedCategory(ID, windowType);
    }

    public async Task ShowLoadingScreen(Func<Task> operation)
    {
        await _view.LoadingScreen.ShowLoadingScreenAsync(operation);
    }
    #endregion
}