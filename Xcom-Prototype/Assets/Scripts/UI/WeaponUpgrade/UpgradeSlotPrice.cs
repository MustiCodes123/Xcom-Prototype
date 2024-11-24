using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Zenject;
using System;

public class UpgradeSlotPrice : MonoBehaviour
{
    public Action BuyButtonClick;
    public int MyIndex;

    [SerializeField] private SpriteToTextConvertor _spriteConvertor;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private GameObject _lock;

    [Inject] private PlayerData _playerData;
    [Inject] private SaveManager _saveManager;
    [Inject] protected UIWindowManager _windowManager;

    private Sprite _priceImage;
    private GameCurrencies _currencyType;
    private uint _price;

    public void SetPrice(Sprite priceImage, GameCurrencies type, uint count)
    {
        _priceImage = priceImage;
        _currencyType = type;
        _price = count;

        _priceText.text = _spriteConvertor.GetSpriteID(type) + count.ToString();

        _buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    public void OnBuyButtonClick()
    {
        if (_price != 0 && _priceImage != null)
        {
            InfoPopup.Instance.ShowTooltip(_priceImage, _currencyType, _price);
            InfoPopup.Instance.ActivateButtons("Buy", "Cancel", () =>
            { BuySlot(); }, null);          
        }
    }

    private void BuySlot()
    {
        if (Wallet.Instance.SpendCachedCurrency(_currencyType, _price))
        {
            _playerData.UpgradeSlotsCount++;
            Debug.Log($"UpgradeSlotsCount after purchasing a new slot {_playerData.UpgradeSlotsCount}");
            _playerData.IsWeaponUpgradeSlotsAvalable[MyIndex] = true;
            _saveManager.SaveGame();
            BuyButtonClick?.Invoke();
        }
        else
        {
            InfoPopup.Instance.ShowTooltipBuyCurrency(_currencyType);
            InfoPopup.Instance.ActivateButtons("Take More Currency", "Cancel", () =>
            { ShowShopWindow(); }, null);
        }

    }

    private void ShowShopWindow()
    {
        _windowManager.ShowWindow(WindowsEnum.ShopWindow);
    }

    private void OnDestroy()
    {
        _buyButton.onClick.RemoveAllListeners();
    }
}