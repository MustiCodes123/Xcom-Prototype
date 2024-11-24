using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SWController : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    [SerializeField] private SWModel _model;
    [SerializeField] private SWView _view;

    [SerializeField] private SlotsExtensionSettings _slotsExtensionSettings;

    private List<SWItem> _storageItems = new List<SWItem>();

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _model.Initialize();
        _storageItems = _view.DisplayItems(_model.ItemsData);

        foreach(SWItem item in _storageItems)
        {            
            item.TakeClick += OnTakeClick;
        }
    }

    private void OnDisable()
    {
        _view.RemoveAllItems();

        foreach (SWItem item in _storageItems)
        {
            item.TakeClick -= OnTakeClick;
        }
    }
    #endregion

    #region Callbacks
    private void OnTakeClick(SWItem item)
    {
        if (item.Data is BaseCharacterModel characterModel)
        {
            if (_playerData.PlayerGroup.GetCharactersFromBothGroup().Count < _playerData.PlayerGroup.MaxGroupSize)
            {
                TakeCharacter(characterModel);
                _view.RemoveSingleItem(item);
                _storageItems.Remove(item);
            }
            else
            {
                _view.ShowNoFreeSlotsPopup(_playerData.CharacterExtension, _slotsExtensionSettings, ExtensionType.Character);
            }
        }
        else if (item.Data is BaseItem inventoryItem)
        {
            if (_playerData.HasFreeInventorySlots())
            {
                TakeItem(inventoryItem);
                _view.RemoveSingleItem(item);
                _storageItems.Remove(item);
            }
            else
            {
                _view.ShowNoFreeSlotsPopup(_playerData.InventoryExtention, _slotsExtensionSettings, ExtensionType.Inventory);
            }
        }
    }
    #endregion

    #region Utility Methods
    private void TakeCharacter(BaseCharacterModel character)
    {      
        _playerData.PlayerGroup.RemoveFromStorage(character);
        _playerData.PlayerGroup.AddCharacterToNotAsignedGroup(character);
    }

    private void TakeItem(BaseItem inventoryItem)
    {
        _playerData.RemoveItemFromStorage(inventoryItem);
        _playerData.AddItemToInventory(inventoryItem);
    }
    #endregion
}