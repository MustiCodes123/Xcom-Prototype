using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TemporaryOfferCategoryButton : ShopSelectableButton
{

    [SerializeField] private TMP_Text _title;

    private OfferSwitcher _offerSwitcher;
    private ISwitchableShopItem _linkedItem;

    public void Initialize(OfferSwitcher offerSwitcher, ISwitchableShopItem linkedItem, string title)
    {
        _offerSwitcher = offerSwitcher;
        _linkedItem = linkedItem;

        if(_title.text == null || _title.text == "<OFFER_NAME>")
            _title.text = title;
    }

    private void OnEnable()
    {
        Button.onClick.AddListener(SwitchToLinkedItem);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(SwitchToLinkedItem);

        Destroy(gameObject);
    }

    public override void ChangeView(ShopCategoryButtonViewConfig viewConfig)
    {
        TextTMP.font = viewConfig.Font;
        Highlight.SetActive(viewConfig.IsSelected);
    }

    private void SwitchToLinkedItem()
    {
        _offerSwitcher.SwitchToOffer(_linkedItem);
    }
}