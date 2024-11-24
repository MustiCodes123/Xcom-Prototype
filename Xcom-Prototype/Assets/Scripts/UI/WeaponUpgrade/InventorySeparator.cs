using UnityEngine;
using UnityEngine.UI;

public class InventorySeparator : MonoBehaviour
{
    [SerializeField] private SlotEnum inventoryType;
    [SerializeField] private WeaponTypeEnum weaponInventoryType;
    [SerializeField] private Button helmetButton;
    [SerializeField] private Button chestButton;
    [SerializeField] private Button weaponButton;
    [SerializeField] private Button shieldButton;
    [SerializeField] private Button legsyButton;
    [SerializeField] private Button glovesButton;
    [SerializeField] private Button jevelryButton;
    [SerializeField] private Button allWeaponsButton;
    [SerializeField] private Button swordsButton;
    [SerializeField] private Button axesButton;
    [SerializeField] private Button bowsButton;
    [SerializeField] private Button hummersButton;
    [SerializeField] private Button spearesButton;
    [SerializeField] private Button macesButton;
    [SerializeField] private Button wandsButton;
    [SerializeField] private GameObject weaponTypeSeparator;
    [SerializeField] private GameObject[] buttonsActiveFrame;
    [SerializeField] private WeaponUpgradeWindow weaponUpgradeWindow;

    private const int _helmetButtonIndex = 0;
    private const int _chestButtonIndex = 1;
    private const int _weaponButtonIndex = 2;
    private const int _shieldButtonIndex = 3;
    private const int _legsButtonIndex = 4;
    private const int _glovesButtonIndex = 5;
    private const int _jewelleryButtonIndex = 6;
    private const int _allWeaponsButtonIndex = 7;
    private const int _swordsButtonIndex = 8;
    private const int _axesButtonIndex = 9;
    private const int _bowsButtonIndex = 10;
    private const int _hummersButtonIndex = 11;
    private const int _spearsButtonIndex = 12;
    private const int _wandsButtonIndex = 13;
    private const int _macesButtonIndex = 14;

    private void Start()
    {
        helmetButton.onClick.AddListener(OnHelmetButtonClicked);
        chestButton.onClick.AddListener(OnchestButtonClicked);
        weaponButton.onClick.AddListener(OnWeaponButtonClicked);
        shieldButton.onClick.AddListener(OnshieldButtonClicked);
        legsyButton.onClick.AddListener(OnLegsButtonClick);
        jevelryButton.onClick.AddListener(OnjevelryButtonClicked);
        glovesButton.onClick.AddListener(OnGlovesButtonClick);
        allWeaponsButton.onClick.AddListener(OnAllWeaponsButtonClicked);
        swordsButton.onClick.AddListener(OnSwordsWeaponsButtonClicked);
        axesButton.onClick.AddListener(OnAxesWeaponsButtonClicked);
        bowsButton.onClick.AddListener(OnBowsWeaponsButtonClicked);
        hummersButton.onClick.AddListener(OnHummersWeaponsButtonClicked);
        spearesButton.onClick.AddListener(OnSpearsWeaponsButtonClicked);
        wandsButton.onClick.AddListener(OnWandsWeaponsButtonClicked);
        macesButton.onClick.AddListener(OnMacesWeaponsButtonClicked);
        OnWeaponButtonClicked();
    }

    public bool CheckInventoryType(BaseItem item)
    {
        if(IsJewelleryCase(item))
        {
            return true;
        }

        if (inventoryType == SlotEnum.Weapon && item is WeaponItem weapon)
        {
            if (weapon.weaponType == WeaponTypeEnum.Shield)
            {
                return false;
            }
            if (weaponInventoryType == WeaponTypeEnum.None)
            {
                return true;
            }

            return weapon.weaponType == weaponInventoryType || IsSimilarWeaponType(weapon) == weaponInventoryType;
        }

        if (item.Slot == inventoryType)
        {
            weaponInventoryType = WeaponTypeEnum.None;
            return true;
        }

        return false; 
    }

    private void OnHelmetButtonClicked()
    {
        ChangeInventoryType(SlotEnum.Head);
        buttonsActiveFrame[_helmetButtonIndex].SetActive(true);
        weaponTypeSeparator.SetActive(false);
    }

    private void OnchestButtonClicked()
    {
        ChangeInventoryType(SlotEnum.Chest);
        buttonsActiveFrame[_chestButtonIndex].SetActive(true);
        weaponTypeSeparator.SetActive(false);
    }

    private void OnWeaponButtonClicked()
    {
        ChangeInventoryType(SlotEnum.Weapon);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_allWeaponsButtonIndex].SetActive(true);
        weaponTypeSeparator.SetActive(true);
    }

    private void OnshieldButtonClicked()
    {
        ChangeInventoryType(SlotEnum.Shield);
        buttonsActiveFrame[_shieldButtonIndex].SetActive(true);
        weaponTypeSeparator.SetActive(false);
    }

    private void OnLegsButtonClick()
    {
        ChangeInventoryType(SlotEnum.Legs);
        buttonsActiveFrame[_legsButtonIndex].SetActive(true);
        weaponTypeSeparator.SetActive(false);
    }

    private void OnGlovesButtonClick()
    {
        ChangeInventoryType(SlotEnum.Gloves);
        buttonsActiveFrame[_glovesButtonIndex].SetActive(true);
        weaponTypeSeparator.SetActive(false);
    }

    private void OnjevelryButtonClicked()
    {
        ChangeInventoryType(SlotEnum.Ring);
        buttonsActiveFrame[_jewelleryButtonIndex].SetActive(true);
        weaponTypeSeparator.SetActive(false);
    }

    private void OnAllWeaponsButtonClicked()
    {
        ChangeInventoryType(SlotEnum.Weapon);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_allWeaponsButtonIndex].SetActive(true);
    }

    private void OnSwordsWeaponsButtonClicked()
    {
        ChangeInventoryType(WeaponTypeEnum.Sword);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_swordsButtonIndex].SetActive(true);
    }

    private void OnAxesWeaponsButtonClicked()
    {
        ChangeInventoryType(WeaponTypeEnum.Axe);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_axesButtonIndex].SetActive(true);
    }

    private void OnBowsWeaponsButtonClicked()
    {
        ChangeInventoryType(WeaponTypeEnum.Bow);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_bowsButtonIndex].SetActive(true);
    }
    private void OnHummersWeaponsButtonClicked()
    {
        ChangeInventoryType(WeaponTypeEnum.Hummer);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_hummersButtonIndex].SetActive(true);
    }

    private void OnSpearsWeaponsButtonClicked()
    {
        ChangeInventoryType(WeaponTypeEnum.Spear);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_spearsButtonIndex].SetActive(true);
    }

    private void OnWandsWeaponsButtonClicked()
    {
        ChangeInventoryType(WeaponTypeEnum.Wand);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_wandsButtonIndex].SetActive(true);
    } 
    private void OnMacesWeaponsButtonClicked()
    {
        ChangeInventoryType(WeaponTypeEnum.Mace);
        weaponTypeSeparator.SetActive(true);
        buttonsActiveFrame[_weaponButtonIndex].SetActive(true);
        buttonsActiveFrame[_macesButtonIndex].SetActive(true);
    }

    private void ChangeInventoryType(SlotEnum type)
    {
        ResetButtons();
        weaponInventoryType = WeaponTypeEnum.None;
        inventoryType = type;
        weaponUpgradeWindow.ShowItems();
    }

    private void ChangeInventoryType(WeaponTypeEnum type)
    {
        ResetButtons();
        inventoryType = SlotEnum.Weapon;
        weaponInventoryType = type;
        weaponUpgradeWindow.ShowItems();
    }

    private void ResetButtons()
    {
        foreach(var item in buttonsActiveFrame)
        {
            item.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        helmetButton.onClick.RemoveAllListeners();
        chestButton.onClick.RemoveAllListeners();
        weaponButton.onClick.RemoveAllListeners();
        shieldButton.onClick.RemoveAllListeners();
        legsyButton.onClick.RemoveAllListeners();
        jevelryButton.onClick.RemoveAllListeners();
        allWeaponsButton.onClick.RemoveAllListeners();
        swordsButton.onClick.RemoveAllListeners();
        axesButton.onClick.RemoveAllListeners();
        hummersButton.onClick.RemoveAllListeners();
        spearesButton.onClick.RemoveAllListeners();
        bowsButton.onClick.RemoveAllListeners();
        wandsButton.onClick.RemoveAllListeners();
    }

    private bool IsJewelleryCase(BaseItem item)
    {
        if (inventoryType == SlotEnum.Ring)
        {
            if (item.Slot == SlotEnum.Ring || item.Slot == SlotEnum.Amulet || item.Slot == SlotEnum.Armlet)
            {
                return true;
            }
        }

        return false;
    }

    private WeaponTypeEnum IsSimilarWeaponType(WeaponItem weapon)
    {
        if (weapon.weaponType == WeaponTypeEnum.Axe || weapon.weaponType == WeaponTypeEnum.TwoHandedAxe)
        {
            return WeaponTypeEnum.Axe;
        }

        if (weapon.weaponType == WeaponTypeEnum.Sword || weapon.weaponType == WeaponTypeEnum.TwoHandedSword ||
            weapon.weaponType == WeaponTypeEnum.Dagger)
        {
            return WeaponTypeEnum.Sword;
        }

        if (weapon.weaponType == WeaponTypeEnum.Hummer || weapon.weaponType == WeaponTypeEnum.TwoHandedHummer)
        {
            return WeaponTypeEnum.Hummer;
        }

        if(weapon.weaponType == WeaponTypeEnum.Mace || weapon.weaponType == WeaponTypeEnum.TwoHandedMace)
        {
            return WeaponTypeEnum.Mace;
        }

        return WeaponTypeEnum.None;
    }
}
