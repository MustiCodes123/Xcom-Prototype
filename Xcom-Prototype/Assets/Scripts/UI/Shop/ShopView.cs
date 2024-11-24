using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System.Linq;
using PlayFab.EconomyModels;

public class ShopView : MonoBehaviour
{
    [Inject] private readonly Wallet _wallet;

    public LoadingScreen LoadingScreen;

    [SerializeField] private WalletView _walletView;
    [SerializeField] private ShopPrefabRepository _prefabRepository;
    [SerializeField] private List<ShopCategoryButtonView> _categoryButtons = new();
    [SerializeField] private List<GameObject> _categoryPanels = new();

    [SerializeField] private Transform _limitedOfferButtonsRoot;
    [SerializeField] private Transform _dailyOfferButtonsRoot;
    [SerializeField] private Transform _gamePassButtonsRoot;

    private List<ShopItem> _shopItems = new();

    private OfferSwitcher _offerSwitcher;
    private ShopPresenter _presenter;

    private IShopModelDataProvider _dataProvider;

    #region Initialization
    public void Initialize(ShopPresenter presenter, OfferSwitcher offerSwitcher, IShopModelDataProvider dataProvider)
    {
        _presenter = presenter;
        _offerSwitcher = offerSwitcher;
        _dataProvider = dataProvider;
        _prefabRepository.Initialize();

        _categoryButtons.ForEach(button => button.Initialize(presenter));
    }

    public void InitializeCurrenciesView()
    {
        _walletView.DisableTMPComponents();

        List<GameCurrencies> baseCurrencies = new List<GameCurrencies>()
        {
            GameCurrencies.Gold,
            GameCurrencies.Gem,
            // GameCurrencies.Energy
        };

        baseCurrencies.ForEach(currency => _walletView.DisplayCurrency(currency));
    }
    #endregion

    #region CategoryButtons
    public void UpdateCategoryButtons(int selectedCategoryID)
    {
        foreach (var button in _categoryButtons)
        {
            if (button.CategoryID == selectedCategoryID)
                button.SetState(new SelectedShopCategoryButtonState());
            else
                button.SetState(new UnselectedShopCategoryButtonState());
        }
    }
    #endregion

    #region DisplayItems
    public void DisplaySelectedCategory(int categoryID, WindowType windowType)
    {
        ClearShopItems();

        _categoryPanels.ForEach(panel => panel.SetActive(false));
        GameObject selectedPanel = _categoryPanels[categoryID];
        selectedPanel.SetActive(true);

        Transform itemContainer = selectedPanel.transform.Find("Offers");

        switch (windowType)
        {
            case (WindowType.GamePass):
                DisplayGamePassItems(itemContainer);
                break;

            case (WindowType.Limited):
                DisplayLimitedItems(itemContainer);
                break;

            case (WindowType.Daily):
                DisplayDailyItems(itemContainer);
                break;

            default:
                Transform content = selectedPanel.transform.Find("Scroll View/Viewport/Content");
                DisplaySimpleCategory(windowType, content);
                break;
        }

        AnimatePanel(selectedPanel);
    }

    private void DisplayGamePassItems(Transform itemContainer)
    {
        List<CatalogItem> items = _dataProvider.GetBundles(WindowType.GamePass);

        foreach (CatalogItem storeItem in items)
        {
            GameObject gamePassWindowGameObject = Instantiate(_prefabRepository.GetPrefabForWindowType(WindowType.GamePass), itemContainer.transform);
            GamePassWindow gamePassWindowComponent = gamePassWindowGameObject.GetComponent<GamePassWindow>();

            gamePassWindowComponent.Initialize(_offerSwitcher, storeItem, _dataProvider, _presenter);
            gamePassWindowComponent.CreateLinkedButton(_gamePassButtonsRoot);
            gamePassWindowComponent.TryPurchase += _presenter.PurchaseManager.HandleTryPurchase;

            _shopItems.Add(gamePassWindowComponent);
        }

        ActivateFirstOffer();
    }

    private void DisplayLimitedItems(Transform itemContainer)
    {
        List<CatalogItem> bundles = _dataProvider.GetBundles(WindowType.Limited);

        foreach (CatalogItem storeItem in bundles)
        {
            GameObject itemGameObject = Instantiate(_prefabRepository.GetPrefabForWindowType(WindowType.Limited), itemContainer.transform);
            LimitedOfferItem shopItem = itemGameObject.GetComponent<LimitedOfferItem>();

            shopItem.Initialize(_offerSwitcher, storeItem, _wallet, _dataProvider, _presenter);
            shopItem.CreateLinkedButton(_limitedOfferButtonsRoot);
            shopItem.TryPurchase += _presenter.PurchaseManager.HandleTryPurchase;

            _shopItems.Add(shopItem);
        }

        ActivateFirstOffer();
    }

    private void DisplayDailyItems(Transform itemContainer)
    {
        List<CatalogItem> bundles = _dataProvider.GetBundles(WindowType.Daily);

        foreach (CatalogItem storeItem in bundles)
        {
            GameObject itemGameObject = Instantiate(_prefabRepository.GetPrefabForWindowType(WindowType.Daily), itemContainer.transform);
            DailyOfferItem shopItem = itemGameObject.GetComponent<DailyOfferItem>();

            shopItem.Initialize(_offerSwitcher, storeItem, _wallet, _dataProvider, _presenter);
            shopItem.CreateLinkedButton(_dailyOfferButtonsRoot);
            shopItem.TryPurchase += _presenter.PurchaseManager.HandleTryPurchase;

            _shopItems.Add(shopItem);
        }

        ActivateFirstOffer();
    }

    private void DisplaySimpleCategory(WindowType windowType, Transform itemContainer)
    {
        List<CatalogItem> bundles = _dataProvider.GetBundles(windowType);
        GameObject prefab = _prefabRepository.GetPrefabForWindowType(windowType);

        bundles = bundles.OrderBy(bundle => ShopHelper.GetPriceFromCatalogItem(bundle)).ToList();

        foreach (CatalogItem bundle in bundles)
        {
            GameObject itemGameObject = Instantiate(prefab, itemContainer);
            ShopItem shopItem = itemGameObject.GetComponent<ShopItem>();

            shopItem.Initialize(bundle, _wallet, _dataProvider, _presenter);
            shopItem.TryPurchase += _presenter.PurchaseManager.HandleTryPurchase;

            _shopItems.Add(shopItem);
        }
    }

    private void ActivateFirstOffer()
    {
        if (_shopItems.Count > 0 && _shopItems[0] is ISwitchableShopItem firstOffer)
        {
            _offerSwitcher.SwitchToOffer(firstOffer);
        }
    }
    #endregion

    #region Utility Methods
    private void ClearShopItems()
    {
        UnsubscribeFromPurchaseEvents();

        foreach (ShopItem itemView in _shopItems)
        {
            Destroy(itemView.gameObject);
        }

        _shopItems.Clear();
    }

    private void AnimatePanel(GameObject panel)
    {
        panel.transform.DOScale(1, 0.3f).From(0.9f).SetEase(Ease.OutBack);
    }

    private void UnsubscribeFromPurchaseEvents()
    {
        foreach (ShopItem shopItem in _shopItems)
        {
            shopItem.TryPurchase -= _presenter.PurchaseManager.HandleTryPurchase;
        }
    }
    #endregion
}