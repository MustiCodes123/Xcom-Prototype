using PlayFab.EconomyModels;
using UnityEngine;

public class LimitedOfferItem : TemporaryOfferItem, ISwitchableShopItem
{
    [SerializeField] private LimitedOfferTimer _timer;

    [field: SerializeField] public TemporaryOfferCategoryButton LinkedButton { get; private set; }

    private void OnDestroy()
    {
        OfferSwitcher.UnregisterOffer(this);
    }

    protected override void SetDetails()
    {
        OfferSwitcher.RegisterOffer(this);

        _timer.Initialize(CatalogItem);
        _descriptionTMP.text = CatalogItem.Description["NEUTRAL"];
    }

    protected override void CreateRewards()
    {
        foreach (CatalogItemReference itemReference in CatalogItem.ItemReferences)
        {
            CatalogItem item = DataProvider.GetItemById(itemReference.Id);

            if (item != null)
            {
                if (item.Type == "currency")
                {
                    CreateCurrencyReward(item, (int)itemReference.Amount);
                }
                else
                {
                    CreateItemReward(item, (int)itemReference.Amount);
                }
            }
        }
    }

    private void CreateCurrencyReward(CatalogItem item, int amount)
    {
        LimitedItemRewardView currencyPrefab = Instantiate(_currencyRewardPrefab, _currencyRewardsContainer);
        currencyPrefab.Initialize($"{amount} {item.Title["NEUTRAL"]}");
    }

    private void CreateItemReward(CatalogItem item, int amount)
    {
        LimitedItemRewardView itemReward = Instantiate(_itemRewardPrefab, _itemsRewardsContainer);
        itemReward.Initialize($"{amount} {item.Title["NEUTRAL"]}");
    }

    public void CreateLinkedButton(Transform parent)
    {
        GameObject buttonGameObject = Instantiate(LinkedButton.gameObject, parent);
        LinkedButton = buttonGameObject.GetComponent<TemporaryOfferCategoryButton>();
        LinkedButton.Initialize(OfferSwitcher, this, CatalogItem.Title["NEUTRAL"]);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public TemporaryOfferCategoryButton GetLinkedButton()
    {
        return LinkedButton;
    }
}