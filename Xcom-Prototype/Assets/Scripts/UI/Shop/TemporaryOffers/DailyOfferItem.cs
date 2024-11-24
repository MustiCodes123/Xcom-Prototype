using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DailyOfferItem : TemporaryOfferItem, ISwitchableShopItem
{
    [SerializeField] private DailyRewardsPopUpView _dailyRewards;

    [field: SerializeField] public TemporaryOfferCategoryButton LinkedButton { get; private set; }

    private void OnDestroy()
    {
        OfferSwitcher.UnregisterOffer(this);
        PurchaseButton.onClick.RemoveListener(OnTryPurchase);
    }

    protected override void SetDetails()
    {
        OfferSwitcher.RegisterOffer(this);
        _descriptionTMP.text = CatalogItem.Description["NEUTRAL"];
    }

    protected override void CreateRewards()
    {
        DailyOfferRewardData rewardData = ShopHelper.DeserializeCustomData<DailyOfferRewardData>(CatalogItem);

        _dailyRewards.Initialize(rewardData);
    }

    public void CreateLinkedButton(Transform parent)
    {
        var buttonGameObject = Instantiate(LinkedButton.gameObject, parent);
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

[System.Serializable]
public class DailyOfferRewardData
{
    public List<DayReward> Days = new List<DayReward>();

    [System.Serializable]
    public class DayReward
    {
        public int Day;
        public string BundleID;
    }
}

