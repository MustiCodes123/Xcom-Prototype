using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using PlayFab.EconomyModels;
using System.Text;
using Zenject;
using System;

public class PurchaseConfirmPopUp : MonoBehaviour
{
    public Action<CatalogItem> Purchase;

    [SerializeField] private TMP_Text _offerNameTMP;
    [SerializeField] private TMP_Text _priceTMP;
    [SerializeField] private TMP_Text _rewardsTMP;
    [SerializeField] private UnityEngine.UI.Image _offerIcon;

    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _cancelButton;

    private ShopItem _purchaseItem;
    private IShopModelDataProvider _dataProvider;

    public void Initialize(ShopItem purchaseItem, IShopModelDataProvider dataProvider)
    {
        _purchaseItem = purchaseItem;
        _dataProvider = dataProvider;

        _offerNameTMP.text = purchaseItem.CatalogItem.Title["NEUTRAL"];
        _priceTMP.text = purchaseItem.ItemPriceText.text;
        _rewardsTMP.text = purchaseItem.CatalogItem.Title["NEUTRAL"];
    }

    private void OnEnable()
    {
        _buyButton.onClick.AddListener(PurchaseItem);
        _cancelButton.onClick.AddListener(Hide);
    }

    private void OnDisable()
    {
        _buyButton.onClick.RemoveListener(PurchaseItem);
        _cancelButton.onClick.RemoveListener(Hide);
    }

    private void PurchaseItem()
    {
        Purchase?.Invoke(_purchaseItem.CatalogItem);
        _buyButton.interactable = false;
        _cancelButton.interactable = false;
        // Hide();
    }

    public void Show()
    {
        _buyButton.interactable = true;
        _cancelButton.interactable = true;
        transform.DOScale(1, 0.3f).From(0.8f).SetEase(Ease.OutBack);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}