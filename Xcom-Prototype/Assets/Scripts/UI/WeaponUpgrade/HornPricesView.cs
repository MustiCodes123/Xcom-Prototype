using UnityEngine;
using Zenject;

public class HornPricesView : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    [SerializeField] private WeaponUpgradeSlotPriceModel _priceModel;
    [SerializeField] private WeaponUpgradePrices _upgradePrices;
    [SerializeField] private Transform[] _locks;
    [SerializeField] private BaseItemSlot[] _slots;
    [SerializeField] private UpgradeSlotPrice[] _upgradeSlotPrices;

    private const int _slotsWithPriceToShow = 2;

    private void Start()
    {
        ShowPrices();
    }

    private void OnEnable()
    {
        foreach (UpgradeSlotPrice upgradeSlotPrice in _upgradeSlotPrices)
        {
            upgradeSlotPrice.BuyButtonClick += ShowPrices;
        }
    }

    private void OnDisable()
    {
        foreach (UpgradeSlotPrice upgradeSlotPrice in _upgradeSlotPrices)
        {
            upgradeSlotPrice.BuyButtonClick -= ShowPrices;
        }
    }

    public void ShowPrices()
    {
        int index = 0;

        foreach (UpgradeSlotPrice upgradeSlotPrice in _upgradeSlotPrices)
        {
            upgradeSlotPrice.MyIndex = index;
            index++;
        }

        int slotsToShowPrice = _slotsWithPriceToShow;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_playerData.IsWeaponUpgradeSlotsAvalable[i] == true)
            {
                _locks[i].gameObject.SetActive(false);
                _slots[i].HidePrice();
            }
            else
            {
                _locks[i].gameObject.SetActive(true);

                if (slotsToShowPrice > 0)
                {
                    slotsToShowPrice--;
                    _slots[i].ShowPrice(_priceModel.GetCurrencyByCurrencyType(_priceModel.Prices[i].Currency),
                                        _priceModel.Prices[i].Currency,
                                        _priceModel.Prices[i].Price);
                }
                else
                {
                    _slots[i].HidePrice();
                }
            }
        }
    }

    public uint GetUpgradePrice(BaseItem item)
    {
        return _upgradePrices.CalculatePrice(item);
    }
}
