using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[System.Serializable]
public class SpriteAndParentOfWeaponImageToBeShownInFailAnimation
{
    [SerializeField] private Sprite _weaponSprite;
    public Sprite WeaponSprite { get => _weaponSprite; }

    [SerializeField] private Transform _imageParent;
    public Transform ImageParent { get => _imageParent; }

    public SpriteAndParentOfWeaponImageToBeShownInFailAnimation(Sprite weaponSprite, Transform imageParent)
    {
        _weaponSprite = weaponSprite;
        _imageParent = imageParent;
    }
}

[System.Serializable]
public class ImageForFailAnimation
{
    [SerializeField] private Transform _parent;
    public Transform Parent { get => _parent; }

    [SerializeField] private Image _imageWithWeaponSprite;
    public Image ImageWithWeaponSprite { get => _imageWithWeaponSprite; }

    [SerializeField] private Image _imageWithRuinSign;
    public Image ImageWithRuinSign { get => _imageWithRuinSign; }
}

public class WeaponUpgradeWindow : UIWindowView
{
    [SerializeField] private BaseItemSlot slotPrefab;

    [SerializeField] public Button upgradeButton;
    [SerializeField] private TMP_Text _priceTMP;
    [SerializeField] private Button removeItemButton;
    [SerializeField] private HornAnimator horn;
    [SerializeField] private InventorySeparator invenoryType;
    [SerializeField] private NewStats newStats;
    [SerializeField] private GameObject goldSlot;
    [SerializeField] private GameObject redSlot;

    [SerializeField] private Transform itemsParent;
    [SerializeField] private BaseItemSlot currentItemSlot;
    [SerializeField] private BaseItemSlot[] slots;
    [SerializeField] private List<ImageForFailAnimation> _imagesForFailAnimation = new();
    [SerializeField] private HornPricesView _hornPricesView;
    [SerializeField] private AnimateUIElements _uiElements;

    [Inject] private CharacterHandler characterHandler;
    [Inject] private BaseDragableItem dragableItem;
    [Inject] private ItemsDataInfo itemsDataInfo;
    [Inject] private Tooltip tooltip;


    private List<BaseItemSlot> itemSlots = new List<BaseItemSlot>();
    private List<BaseItemSlot> updateItemSlots = new List<BaseItemSlot>();
    private List<BaseItem> itemsInUpgradeSlots = new List<BaseItem>();
    private BaseItem currentItem;
    private bool[] _isFullSlots = new bool[8];
    private string characterName;

    private const float _upgradeChanseConstant = 0.1f;
    private const int _minItemDegradeLvl = 10;
    private readonly Dictionary<RareEnum, int> _rareDropChanseInfluence = new Dictionary<RareEnum, int>()
    {
        { RareEnum.Common, 7},
        { RareEnum.Rare, 12},
        { RareEnum.Epic, 15},
        { RareEnum.Legendary, 17},
        { RareEnum.Mythical, 20},
    };

    public GameObject UpgradeButtonGameObject { get => upgradeButton.gameObject; }

    private void Start()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        removeItemButton.onClick.AddListener(OnRemoveItemButtonClicked);

        currentItemSlot.Construct(dragableItem, characterHandler, tooltip);
    }

    public void OnRemoveItemButtonClicked()
    {
        foreach (var slot in updateItemSlots)
        {
            slot.Reset();
        }

        horn.RestoreParametersAfterAnimation();
        currentItemSlot.Reset();
        currentItem = null;
        updateItemSlots.Clear();
        itemsInUpgradeSlots.Clear();
        ShowItems();
        RefreshSlots();
        goldSlot.SetActive(false);
        redSlot.SetActive(false);
        newStats.gameObject.SetActive(false);
        removeItemButton.gameObject.SetActive(false);
    }

    private void OnUpgradeButtonClicked()
    {
        int currentItems = 0;

        for (int i = 0; i < updateItemSlots.Count; i++)
        {
            if (updateItemSlots[i].Item != null)
            {
                currentItems++;
            }
        }

        if (currentItems > 0 && !IsMaxLevel(currentItem))
        {
            horn.ActivateLines(currentItems);
            horn.ActivateHorn();
            horn.ShowWeaponUpgradingAnimation();
            _signalBus.Fire(new UpgradeSignal() { IsCharacterUpgrade = false, Item = currentItem });

            Wallet.Instance.SpendCachedCurrency(GameCurrencies.Gold, _hornPricesView.GetUpgradePrice(currentItem));
        }
    }

    private void OnCancelItemClick(BaseItemSlot cancelSlot)
    {
        for (int i = 0; i < updateItemSlots.Count; i++)
        {
            if (updateItemSlots[i] == cancelSlot)
            {
                var freeSlot = GetFreeInventorySlot();
                freeSlot.SetItem(cancelSlot.Item);
                freeSlot.gameObject.SetActive(true);

                itemsInUpgradeSlots.Remove(cancelSlot.Item);

                updateItemSlots[i].Reset();
                cancelSlot = null;
                break;
            }
        }
        UpdateItemStats();
    }

    public void TryToUpgradeItem()
    {
        float chanse = CalculateChance(currentItem);
        float random = new System.Random().Next(0, 100);

        bool IsUpgradeSuccessful = chanse > random;
        horn.ShouldShowSuccessAnimation = IsUpgradeSuccessful;
        if (IsUpgradeSuccessful)
        {
            UpgradeItem();
        }
        else
        {
            UpgradeFail();
        }
    }

    public void ShowItems()
    {
        ClearInventory();

        for (int i = 0; i < playerData.PlayerInventory.Count; i++)
        {
            if (currentItem != null)
            {
                if (NoEqualItemType(currentItem, playerData.PlayerInventory[i]))
                {
                    continue;
                }

                if (currentItem == playerData.PlayerInventory[i])
                {
                    continue;
                }

                if (playerData.PlayerInventory[i].CurrentLevel != currentItem.CurrentLevel)
                {
                    continue;
                }
            }

            if (!itemsInUpgradeSlots.Contains(playerData.PlayerInventory[i]))
            {
                if (invenoryType.CheckInventoryType(playerData.PlayerInventory[i]))
                {
                    Debug.Log($"Adding item to list: {playerData.PlayerInventory[i].itemName}");

                    var slot = Instantiate(slotPrefab, itemsParent);
                    slot.Construct(dragableItem, characterHandler, tooltip);
                    slot.SetItem(playerData.PlayerInventory[i], true, OnItemClicked);
                    itemSlots.Add(slot);
                }
            }
        }

        if (CheckCharacterInventory())
        {
            foreach (var character in playerData.PlayerGroup.GetCharactersFromBothGroup())
            {
                var currentCharacter = character;
                foreach (var item in character.EquipedItems)
                {
                    if (currentItem == item)
                    {
                        continue;
                    }

                    if (invenoryType.CheckInventoryType(item))
                    {
                        if (currentItem == null)
                        {
                            characterName = currentCharacter.Name;
                            var inventorySlot = Instantiate(slotPrefab, itemsParent);
                            inventorySlot.Construct(dragableItem, characterHandler, tooltip);
                            inventorySlot.SetItem(item, true, OnItemClicked);
                            itemSlots.Add(inventorySlot);
                            var dragItem = inventorySlot.transform.GetComponentInChildren<BaseDragableItem>();
                            dragItem.SetOwnerImage(currentCharacter, _resourceManager);
                        }
                        else if (!NoEqualItemType(currentItem, item) && currentItem.CurrentLevel == item.CurrentLevel)
                        {
                            characterName = currentCharacter.Name;
                            var inventorySlot = Instantiate(slotPrefab, itemsParent);
                            inventorySlot.Construct(dragableItem, characterHandler, tooltip);
                            inventorySlot.SetItem(item, true, OnItemWithOwnerClicked);
                            itemSlots.Add(inventorySlot);
                            var dragItem = inventorySlot.transform.GetComponentInChildren<BaseDragableItem>();
                            dragItem.SetOwnerImage(currentCharacter, _resourceManager);
                            characterHandler.SetActiveCharacter(currentCharacter);
                        }
                    }
                }
            }
        }

        upgradeButton.enabled = false;
    }

    public void SetItemInHorn(BaseItem item, BaseItemSlot slot)
    {
        if (currentItem == null)
        {
            currentItem = item;
            currentItemSlot.SetItem(item);
            ShowItems();

            for (int i = 0; i < playerData.UpgradeSlotsCount; i++)
            {
                if (updateItemSlots.Count <= i)
                {
                    var updateItem = GetFreeSlot();
                    updateItem.Construct(dragableItem, characterHandler, tooltip);
                    updateItem.Reset();
                    updateItem.IsDragable = false;
                    updateItemSlots.Add(updateItem);
                }
            }

            _priceTMP.text = _hornPricesView.GetUpgradePrice(item).ToString();
        }
        else
        {
            SetupUpgradeItems(item, slot);
            var dragItem = slot.transform.GetComponentInChildren<BaseDragableItem>();

            if (characterHandler.GetCurrentCharacterInfo() != null && dragItem.HasOwner())
            {
                characterHandler.UnequipItem(item);
            }

            slot.gameObject.SetActive(false);
        }

        UpdateItemStats();
        removeItemButton.gameObject.SetActive(true);
    }

    private void ClearInventory()
    {
        foreach (var item in itemSlots)
        {
            Destroy(item.gameObject);
        }
        itemSlots.Clear();
    }

    private bool CheckCharacterInventory()
    {
        foreach (var character in playerData.PlayerGroup.GetCharactersFromBothGroup())
        {
            foreach (var item in character.EquipedItems)
            {
                return true;
            }
        }
        return false;
    }

    private void SetupUpgradeItems(BaseItem item, BaseItemSlot slot)
    {
        int maxToAddItems = playerData.UpgradeSlotsCount;
        int currentItems = updateItemSlots.Count(s => s.Item != null);

        if (currentItems < maxToAddItems)
        {
            BaseItemSlot freeSlot = updateItemSlots.FirstOrDefault(s => s.Item == null);

            if (freeSlot != null)
            {
                freeSlot.SetItem(item, true);
                var dragItem = freeSlot.transform.GetComponentInChildren<BaseDragableItem>();
                dragItem.EnableCacelButton().onClick.AddListener(() => OnCancelItemClick(freeSlot));
            }
            else
            {
                var updateItem = GetFreeSlot();
                updateItem.Construct(dragableItem, characterHandler, tooltip);
                updateItem.SetItem(item, true);
                var dragItem = updateItem.transform.GetComponentInChildren<BaseDragableItem>();
                dragItem.EnableCacelButton().onClick.AddListener(() => OnCancelItemClick(updateItem));
                updateItem.IsDragable = false;
                updateItemSlots.Add(updateItem);
            }

            slot.Reset();

            itemsInUpgradeSlots.Add(item);

            upgradeButton.enabled = updateItemSlots.Count > 0;
        }
        else
        {
            upgradeButton.enabled = true;
            Debug.Log("Upgrade is full");
        }
    }

    private void OnItemClicked(BaseItem item, BaseItemSlot slot)
    {
        InfoPopup.Instance.ShowTooltip(item);
        InfoPopup.Instance.ActivateButtons("Use", "Cancel", () =>
        { SetItemInHorn(item, slot); }, null);
    }

    private void OnItemWithOwnerClicked(BaseItem item, BaseItemSlot slot)
    {
        string text = "This item is already in use by " + characterName + ", If you use it to improve, the character will lose the item.";
        InfoPopup.Instance.ShowTooltip(item, text);
        InfoPopup.Instance.ActivateButtons("Use", "Cancel", () =>
        { SetItemInHorn(item, slot); }, null);
    }

    private bool NoEqualItemType(BaseItem currentItemToCheck, BaseItem ItemToCheck)
    {
        if (currentItemToCheck == ItemToCheck)
            return false;

        else if (currentItemToCheck is WeaponItem && ItemToCheck is ArmorItem)
            return true;

        else if (currentItemToCheck is WeaponItem currentWeapon && ItemToCheck is WeaponItem weapon && currentWeapon.weaponType != weapon.weaponType)
            return true;

        else if (currentItemToCheck is ArmorItem && ItemToCheck is WeaponItem)
            return true;

        else if (currentItemToCheck is ArmorItem currentArmor && ItemToCheck is ArmorItem armor && currentArmor.Slot != armor.Slot)
            return true;

        else
            return false;
    }

    private BaseItemSlot GetFreeSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!_isFullSlots[i] && playerData.IsWeaponUpgradeSlotsAvalable[i])
            {
                _isFullSlots[i] = true;
                return slots[i];
            }
        }

        return null;
    }

    private BaseItemSlot GetFreeInventorySlot()
    {
        foreach (var slot in itemSlots)
        {
            if (slot.Item == null)
            {
                return slot;
            }
        }

        var newSlot = Instantiate(slotPrefab, itemsParent);
        newSlot.Construct(dragableItem, characterHandler, tooltip);
        itemSlots.Add(newSlot);
        return newSlot;
    }

    public void RefreshSlots()
    {
        updateItemSlots.Clear();

        for (int i = 0; i < _isFullSlots.Length; i++)
        {
            _isFullSlots[i] = false;
        }

        _hornPricesView.ShowPrices();
    }

    public override void Show()
    {
        base.Show();
        _uiElements.AnimatePanelsIn();
        characterHandler.SetActiveCharacter(playerData.PlayerGroup.GetCharactersFromBothGroup()[0]);
        ShowItems();

        _hornPricesView.ShowPrices();

    }

    public override void Hide()
    {
        base.Hide();
        _uiElements.AnimatePanelsOut();
        OnRemoveItemButtonClicked();
        _saveManager.SaveGame();
    }

    public float CalculateChance(BaseItem item)
    {
        float x = item.CurrentLevel * 0.1f;
        float result = Mathf.Pow(_upgradeChanseConstant, x) * 100;

        foreach (var itemToDestroy in updateItemSlots)
        {
            if (itemToDestroy.Item != null)
            {
                result += _rareDropChanseInfluence[itemToDestroy.Item.Rare];
            }
        }

        if (result > 100)
            return 100;
        else
            return result;
    }

    private void UpgradeFail()
    {
        redSlot.SetActive(true);

        float chanse = CalculateChance(currentItem);
        float random = new System.Random().Next(0, 100);

        if (currentItem.CurrentLevel >= _minItemDegradeLvl && chanse < random)
        {
            currentItem.CurrentLevel -= 1;
            SaveItemData(currentItem);
        }

        horn.EmptyImagesForFailAnimation.Clear();
        horn.SpritesDataForFailAnimation.Clear();
        for (int i = 0; i < slots.Length; i++)
        {
            BaseDragableItem item = slots[i].GetComponentInChildren<BaseDragableItem>();
            if (item != null)
            {
                horn.EmptyImagesForFailAnimation.Add(slots[i].transform, _imagesForFailAnimation.Find(imageForFailAnimation => imageForFailAnimation.Parent.Equals(slots[i].transform)));
                horn.SpritesDataForFailAnimation.Add(new(item.Item.itemSprite, slots[i].transform));
            }
        }

        ResetUpdateItemSlots();
    }

    private void UpgradeItem()
    {
        currentItem.CurrentLevel++;
        currentItemSlot.UpdateLevelCount();
        goldSlot.SetActive(true);

        SaveItemData(currentItem);

        ResetUpdateItemSlots();

        UpdateItemStats();
    }

    private void ResetUpdateItemSlots()
    {
        currentItemSlot.SetItem(currentItem);

        for (int i = 0; i < updateItemSlots.Count; i++)
        {
            playerData.PlayerInventory.Remove(updateItemSlots[i].Item);
            updateItemSlots[i].Reset();
        }

        ShowItems();
    }

    private void SaveItemData(BaseItem item)
    {
        var armor = item as ArmorItem;

        if (armor != null && itemsDataInfo.Armors.First(x => x.itemID == item.itemID).ArmorUpgradeStats.Length > item.CurrentLevel)
        {
            var data = itemsDataInfo.Armors.First(x => x.itemID == item.itemID).ArmorUpgradeStats[item.CurrentLevel];
            ArmorUpgradeStats nextLevelData;

            if (itemsDataInfo.Armors.First(x => x.itemID == item.itemID).ArmorUpgradeStats.Length > item.CurrentLevel + 1)
            {
                nextLevelData = itemsDataInfo.Armors.First(x => x.itemID == item.itemID).ArmorUpgradeStats[item.CurrentLevel + 1];
            }
            else
            {
                nextLevelData = null;
            }

            armor.SetupLevel(data, nextLevelData);
        }

        var weapon = item as WeaponItem;

        if (weapon != null && itemsDataInfo.Weapons.First(x => x.itemID == item.itemID).WeaponUpgradeStats.Length > item.CurrentLevel)
        {
            var data = itemsDataInfo.Weapons.First(x => x.itemID == item.itemID).WeaponUpgradeStats[item.CurrentLevel];
            WeaponUpgradeStats nextLevelData;

            if (itemsDataInfo.Weapons.First(x => x.itemID == item.itemID).WeaponUpgradeStats.Length > item.CurrentLevel + 1)
            {
                nextLevelData = itemsDataInfo.Weapons.First(x => x.itemID == item.itemID).WeaponUpgradeStats[item.CurrentLevel + 1];
            }
            else
            {
                nextLevelData = null;
            }

            weapon.SetupLevel(data, nextLevelData);
        }
    }

    private void UpdateItemStats()
    {
        newStats.gameObject.SetActive(true);

        if (updateItemSlots[0].Item != null)
        {
            newStats.SetupWeaponStats(currentItem, CalculateChance(currentItem));
        }
        else
        {
            newStats.SetupWeaponStats(currentItem, 0);
        }
    }

    private bool IsMaxLevel(BaseItem item)
    {
        if (item is ArmorItem armor)
        {
            if (itemsDataInfo.Armors.First(x => x.itemID == item.itemID).ArmorUpgradeStats.Length <= item.CurrentLevel + 1)
            {
                return true;
            }
        }
        else if (item is WeaponItem weapon)
        {
            if (itemsDataInfo.Weapons.First(x => x.itemID == item.itemID).WeaponUpgradeStats.Length <= item.CurrentLevel + 1)
            {
                return true;
            }
        }

        return false;
    }
}