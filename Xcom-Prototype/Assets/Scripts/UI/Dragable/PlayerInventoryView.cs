using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.CharacterWindow;
using UnityEngine.UI;
using Zenject;

public class PlayerInventoryView : MonoBehaviour
{
    public BaseItemSlot[] CharacterItemSlots { get; set; }

    [SerializeField] private Transform inventoryConteiner;
    [SerializeField] private int containerID;

    [SerializeField] private Button inventoryExtentionButton;
    [SerializeField] private SlotsExtensionSettings _extensionSettings;
    [SerializeField] private SlotsExtensionPopUp _slotsExtensionPopUp;
    [SerializeField] private WeaponCounter _weaponCounter;
    [SerializeField] private ItemInfoPopup _itemInfoPopUp;

    [Inject] private BaseItemSlot _itemSlot;
    [Inject] private PlayerData _playerData;
    [Inject] private BaseDragableItem _dragableItem;
    [Inject] private CharacterHandler _characterHandler;
    [Inject] private ShopHundler _shopHundler;
    [Inject] private ItemsDataInfo _itemsDataInfo;
    [Inject] private Tooltip _tooltip;
    [Inject] private UIWindowManager _windowManager;

    private List<BaseItemSlot> _itemSlots = new List<BaseItemSlot>();
    private BaseItemSlot _targetSlot;

    private void Start()
    {
        if (_playerData.InventoryExtention >= _extensionSettings.InventorySlotsExtensionLevels.Count)
            inventoryExtentionButton.gameObject.SetActive(false);
        else
            inventoryExtentionButton.onClick.AddListener(OnInventoryExtentionClick);

        _slotsExtensionPopUp.TryPurchaseInventorySlots += OnTryPurchaseSlots;

        UpdateCounter();
    }

    private void OnInventoryExtentionClick()
    {
        _slotsExtensionPopUp.Show(_playerData.InventoryExtention, _extensionSettings, ExtensionType.Inventory);
    }

    private void OnEnable()
    {
        CreateInventory();

        if (_playerData.InventoryExtention >= _extensionSettings.InventorySlotsExtensionLevels.Count)
        {
            inventoryExtentionButton.gameObject.SetActive(false);
        }
        else
        {
            inventoryExtentionButton.gameObject.SetActive(true);
        }

        UpdateCounter();
    }

    public void Show() => gameObject.SetActive(true);

    public void CreateInventory()
    {
        if (_itemSlots.Count <= _playerData.CurrentInventorySize)
        {
            int difference = _playerData.CurrentInventorySize - _itemSlots.Count;
            for (int i = 0; i < difference; i++)
            {
                BaseItemSlot itemSlot = Instantiate(this._itemSlot, inventoryConteiner);
                itemSlot.Construct(_dragableItem, _characterHandler, _tooltip);
                itemSlot.SetItem(null);
                itemSlot.SlotID = i;
                itemSlot.ContainerID = containerID;

                itemSlot.OnEquip += UpdateCounter;

                _itemSlots.Add(itemSlot);               
            }

            inventoryExtentionButton.transform.SetSiblingIndex(_itemSlots.Count);
        }
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].Reset();
        }

        int slotIndex = 0;
        for (int i = 0; i < _playerData.PlayerInventory.Count; i++)
        {
            if (slotIndex >= _playerData.CurrentInventorySize)
            {
                break;
            }

            if (_playerData.PlayerInventory[i] is ArtifactRecipeItem or CraftItem)
                continue;

            if (_playerData.PlayerInventory[i] != null)
            {
                if (slotIndex < _itemSlots.Count)
                {
                    _itemSlots[slotIndex].SetItem(_playerData.PlayerInventory[i], true, OnItemCliked);
                }
                else
                {
                    BaseItemSlot itemSlot = Instantiate(this._itemSlot, inventoryConteiner);
                    itemSlot.Construct(_dragableItem, _characterHandler, _tooltip);
                    itemSlot.SetItem(_playerData.PlayerInventory[i], true, OnItemCliked);

                    itemSlot.SlotID = slotIndex;
                    itemSlot.ContainerID = containerID;

                    itemSlot.OnEquip += UpdateCounter;

                    _itemSlots.Add(itemSlot);
                }
                slotIndex++;
            }
        }
        inventoryExtentionButton.transform.SetSiblingIndex(_itemSlots.Count);
    }

    private void AddNewSlots()
    {
        int difference = _playerData.CurrentInventorySize - _itemSlots.Count;

        for (int i = 0; i < difference; i++)
        {
            BaseItemSlot itemSlot = Instantiate(this._itemSlot, inventoryConteiner);
            itemSlot.Construct(_dragableItem, _characterHandler, _tooltip);
            itemSlot.SetItem(null);
            itemSlot.SlotID = _itemSlots.Count;
            itemSlot.ContainerID = containerID;

            itemSlot.OnEquip += UpdateCounter;

            _itemSlots.Add(itemSlot);
        }

        if (_playerData.InventoryExtention >= _extensionSettings.InventorySlotsExtensionLevels.Count)
            inventoryExtentionButton.gameObject.SetActive(false);
        else
            inventoryExtentionButton.gameObject.SetActive(true);

        inventoryExtentionButton.transform.SetSiblingIndex(_itemSlots.Count);
    }

    private void OnItemCliked(BaseItem item, BaseItemSlot slot)
    {
        _itemInfoPopUp.Show(item);

        foreach (var itemSlot in CharacterItemSlots)
        {
            if (itemSlot.slotType == item.Slot
                || (item is WeaponItem weapon && weapon.weaponType == WeaponTypeEnum.Shield
                && itemSlot.slotType == SlotEnum.OffHand))
            {
                _targetSlot = itemSlot;
            }
        }
        
        if (_targetSlot != null)
        {
            _itemInfoPopUp.ActivateButtons("Equip", () =>
            {
                SetItemInSlot(item, slot);

                _itemInfoPopUp.gameObject.SetActive(false);
            },
            () =>
            {
                _itemInfoPopUp.gameObject.SetActive(false);
            });
        }
    }

    private void SetCheck(BaseItem clickedItem)
    {
        BaseItemsSet itemSet = clickedItem.ItemsSet;

        foreach (var slot in CharacterItemSlots)
        {
            if (slot.slotType == SlotEnum.Ring || slot.slotType == SlotEnum.Amulet || slot.slotType == SlotEnum.Armlet)
                continue;

            if (slot.IsFreeSlot())
                return;

            if (slot.GetItem().Item.ItemsSet.PlayFabID != itemSet.PlayFabID)
                return;
        }

        _characterHandler.GetCurrentCharacterInfo().AddSetBonusStats(itemSet.StatsBonus);
    }

    private void SetItemInSlot(BaseItem item, BaseItemSlot slot)
    {
        Debug.LogError($"Taking item: {item.itemName}, {item.itemID}");

        var weapon = item as WeaponItem;

        if (_targetSlot is OffHandInventorySlot offHandSlot)
        {
            if (offHandSlot.IsSuitableWeapon(weapon) || weapon.weaponType == WeaponTypeEnum.Shield)
                offHandSlot.TryUnequipTwoHandedWeapon();
        }
        if (_targetSlot is WeaponInventorySlot weaponSlot)
        {
            if (weapon == null)
            {
                Debug.Log(item.GetType());
            }

            if (weaponSlot.IsTwoHandedWeapon(weapon))
                weaponSlot.GetOffHand().UnequipOffHand();
        }
        if (_targetSlot.Item != null)
        {
            _characterHandler.UnequipItem(_targetSlot.Item);
        }
        if (item != null)
        {
            slot.Reset();
            _characterHandler.EquipItem(item);
            CreateInventory();
        }

        SetCheck(item);

        UpdateCounter();
    }

    private void OnTryPurchaseSlots(int price, GameCurrencies currency)
    {
        bool isEnoughCurrency = Wallet.Instance.SpendCachedCurrency(currency, (uint)price);

        if (isEnoughCurrency)
        {
            if (_playerData.InventoryExtention < _extensionSettings.InventorySlotsExtensionLevels.Count)
            {
                _slotsExtensionPopUp.gameObject.SetActive(false);
                _playerData.CurrentInventorySize += _extensionSettings.InventorySlotsExtensionLevels[_playerData.InventoryExtention].SlotCount;
                AddNewSlots();

                _playerData.InventoryExtention++;

                if (_playerData.InventoryExtention >= _extensionSettings.InventorySlotsExtensionLevels.Count)
                    inventoryExtentionButton.gameObject.SetActive(false);
            }
            else
            {
                inventoryExtentionButton.gameObject.SetActive(false);
                Debug.LogWarning("No more inventory extensions available.");
            }
        }
        else
        {
            InfoPopup.Instance.ShowTooltipBuyCurrency(currency);
            InfoPopup.Instance.ActivateButtons("Take More Currency", "Cancel", () =>
            { ShowShopWindow(); }, null);
            _slotsExtensionPopUp.gameObject.SetActive(false);
        }

        UpdateCounter();
    }

    private void UpdateCounter()
    {
        int weaponCount = _playerData.GetInventoryItems().Count;
        _weaponCounter.UpdateCounterView(weaponCount, _playerData.CurrentInventorySize);
    }
    private void ShowShopWindow()
    {
        _windowManager.ShowWindow(WindowsEnum.ShopWindow);
    }

    private void Unsubscribe()
    {
        foreach (BaseItemSlot slot in _itemSlots)
        {
            slot.OnEquip -= UpdateCounter;
        }
    }

    private void OnDestroy()
    {
        inventoryExtentionButton.onClick.RemoveAllListeners();
        _slotsExtensionPopUp.TryPurchaseInventorySlots -= OnTryPurchaseSlots;
        Unsubscribe();
    }
}
