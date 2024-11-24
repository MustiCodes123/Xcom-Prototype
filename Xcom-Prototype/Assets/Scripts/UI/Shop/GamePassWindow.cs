using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GamePassWindow : ShopItem, ISwitchableShopItem
{
    [SerializeField] private Transform _cardsContainer;
    [SerializeField] private GamePassCard _gamePassCard;
    [SerializeField] private TemporaryOfferCategoryButton _linkedButton;
    [SerializeField] private TMP_Text _description;

    private List<GamePassCard> _gamePassCards = new List<GamePassCard>();

    private OfferSwitcher _offerSwitcher;
    private GamePassWindowData _windowData;

    private int _priceValue;
    private GameCurrencies _currencyValue;
    private bool _isPurchased;

    public void Initialize(OfferSwitcher offerSwitcher, PlayFab.EconomyModels.CatalogItem item, IShopModelDataProvider dataProvider, ShopPresenter presenter)
    {
        Presenter = presenter;
        _offerSwitcher = offerSwitcher;
        CatalogItem = item;
        DataProvider = dataProvider;

        _windowData = new GamePassWindowData
        {
            Title = item.Title["NEUTRAL"],
            Description = item.Description["NEUTRAL"],
            Cards = SetCards(item)
        };

        _windowData.Cards.Sort((card1, card2) => card1.Level.CompareTo(card2.Level));
        _offerSwitcher.RegisterOffer(this);

        int priceValue = 0;
        string currency = "";

        foreach (var price in item.PriceOptions.Prices)
        {
            foreach (var amounts in price.Amounts)
            {
                string currencyID = amounts.ItemId;
                priceValue = amounts.Amount;
                _currencyValue = ShopHelper.ConvertItemIDToCurrencyCode(currencyID);
                currency = _currencyValue.ToString();
            }
        }

        ItemPriceText.text = $"{priceValue} {currency}";
        _description.text = _windowData.Description;

        _priceValue = priceValue;

        CheckPurchaseStatus();

        foreach (var cardData in _windowData.Cards)
        {
            AddCard(cardData);
        }

        PurchaseButton.onClick.AddListener(OnTryPurchase);
    }

    private List<GamePassWindowData.GamePassReward> SetCards(PlayFab.EconomyModels.CatalogItem item)
    {
        var cards = new List<GamePassWindowData.GamePassReward>();
        var bundleItems = DataProvider.GetItemsFromBundle(item.Id);

        foreach (var bundleItem in bundleItems)
        {
            int level = GetLevelFromItem(bundleItem);
            cards.Add(new GamePassWindowData.GamePassReward
            {
                Level = level
            });
        }

        return cards;
    }

    private void OnDestroy()
    {
        _offerSwitcher.UnregisterOffer(this);
        PurchaseButton.onClick.RemoveListener(OnTryPurchase);
    }

    public void AddCard(GamePassWindowData.GamePassReward cardData)
    {
        GameObject cardGameObject = Instantiate(_gamePassCard.gameObject, _cardsContainer);
        GamePassCard gamePassCard = cardGameObject.GetComponent<GamePassCard>();

        gamePassCard.InjectPlayerData(Presenter.PlayerData);

        PlayFab.EconomyModels.CatalogItem bundleItem = DataProvider.GetItemsFromBundle(CatalogItem.Id).First(x => GetLevelFromItem(x) == cardData.Level);

        int quantity = CatalogItem.ItemReferences?
            .FirstOrDefault(x => GetLevelFromItem(DataProvider.GetItemById(x.Id)) == cardData.Level)?.Amount ?? 0;

        gamePassCard.InitializeWithReward(bundleItem, cardData, quantity, Wallet.Instance, Presenter, _isPurchased);

        _gamePassCards.Add(gamePassCard);
    }

    public void CreateLinkedButton(Transform parent)
    {
        var buttonGameObject = Instantiate(_linkedButton.gameObject, parent);
        _linkedButton = buttonGameObject.GetComponent<TemporaryOfferCategoryButton>();
        _linkedButton.Initialize(_offerSwitcher, this, _windowData.Title);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public TemporaryOfferCategoryButton GetLinkedButton()
    {
        return _linkedButton;
    }

    private int GetLevelFromItem(PlayFab.EconomyModels.CatalogItem item)
    {
        var levelData = ShopHelper.DeserializeCustomData<GamePassWindowData.GamePassReward>(item);
        return levelData.Level;
    }

    private async void Purchase()
    {
        if (Wallet.Instance.GetCachedCurrencyAmount(_currencyValue) >= _priceValue)
        {
            _isPurchased = true;

            Presenter.PurchaseManager.PurchaseGamePass(CatalogItem);
            await Task.Delay(100);
            UpdateCardsState();
        }
    }

    private void UpdateCardsState()
    {
        foreach (var card in _gamePassCards)
        {
            card.SetPurchased(_isPurchased);
        }
    }

    private async void CheckPurchaseStatus()
    {
        await Presenter.ShowLoadingScreen(async () =>
        {
            await GameEconomy.GetUserData(
                result =>
                {
                    Dictionary<string, UserDataRecord> data = result.Data;
                    string record = data.TryGetValue($"{CatalogItem.Title["NEUTRAL"]}", out var recordValue) ? recordValue.Value : null;
                    _isPurchased = record == "true";

                    UpdateCardsState();
                },
                error =>
                {
                    Debug.LogError($"Failed to get user data: {error.ErrorMessage}");
                });
        });
    }

    public void OnTryPurchase()
    {
        Purchase();
    }
}

[System.Serializable]
public class GamePassWindowData
{
    public string Title;
    public string Description;
    public List<GamePassReward> Cards;

    [System.Serializable]
    public class GamePassReward
    {
        public int Level;
    }
}