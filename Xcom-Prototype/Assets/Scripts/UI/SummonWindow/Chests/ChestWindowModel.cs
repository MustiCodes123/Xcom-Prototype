using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestWindowModel : MonoBehaviour
{
    [SerializeField] private ChestButton[] _chestButtons;

    [field: SerializeField] public ChestRarityDropChances DropChances { get; private set; }
    public ChestData SelectedChestData { get; set; }

    public List<ItemTemplate> AllItems { get; private set; }

    public uint PriceOfFirstChest { get => _chestButtons[0].Data.Price; }

    public async void Initialize()
    {
        await PlayerInventory.Instance.UpdateInventory();

        SetWeaponItems();

        SetupChestButtons();
    }

    private void SetupChestButtons()
    {
        foreach (ChestButton chestButton in _chestButtons)
        {
            ChestType chestRarity = chestButton.Data.ChestType;
            int chestAmount = PlayerInventory.Instance.GetChestsData(chestRarity).Amount;
            chestButton.ButtonView.DisplayAmount(chestAmount);
        }
    }

    private void SetWeaponItems()
    {
        AllItems = new List<ItemTemplate>();

        AllItems.AddRange(ItemsDataInfo.Instance.Armors);
        AllItems.AddRange(ItemsDataInfo.Instance.Weapons);
        AllItems.AddRange(ItemsDataInfo.Instance.Amulets);
        AllItems.AddRange(ItemsDataInfo.Instance.Armlets);
        AllItems.AddRange(ItemsDataInfo.Instance.Rings);
    }
}
