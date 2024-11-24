using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Zenject;

public class ChestWindowView : MonoBehaviour
{
    [Inject] private PlayerData _playerData;
    [SerializeField] private Chest3DModel[] _chests;
    [SerializeField] private ChestButton[] _chestButtons;
    [SerializeField] private ChestCardPrefab _cardPrefab;
    [SerializeField] private ChestMultipleRewardCardsView _multipleCardsView;
    [SerializeField] private TMP_Text _priceView;
    [SerializeField] private Button _hideSingleCardButton;
    [SerializeField] private AnimateUIElements _chestUIElements;
    [SerializeField] private TMP_Text _itemsCountText;
    [SerializeField] private ChestWindowModel _chestWindowModel;

    private void OnEnable()
    {
        foreach (ChestButton button in _chestButtons)
        {
            button.OnPriceChanged += OnPriceChanged;

            if(button.transform.GetSiblingIndex() == 0)
            {
                OnPriceChanged(button.Data.Price);
            }
        }
        
        _playerData.ItemAddedToInventory += ItemAddedToInventory;
        UpdateTotalItemsAmount();

        _chestUIElements.AnimatePanelsIn();
    }

    private void OnDisable()
    {
        foreach (ChestButton button in _chestButtons)
        {
            button.OnPriceChanged += OnPriceChanged;

            if (button.transform.GetSiblingIndex() == 0)
            {
                OnPriceChanged(button.Data.Price);
            }
        }
        
        _playerData.ItemAddedToInventory -= ItemAddedToInventory;

        _chestUIElements.AnimatePanelsOut();
    }

    public void ShowChest(Chest3DModel selectedChest)
    {
        Array.ForEach(_chests, chest => chest.gameObject.SetActive(false));

        selectedChest.gameObject.SetActive(true);
    }

    public void ShowOpenAnimation(Chest3DModel selectedChest)
    {
        selectedChest.OpenAnimation.OpenChest();
    }

    public void ShowCard(ItemTemplate itemData)
    {
        _cardPrefab.Show(itemData);
        _hideSingleCardButton.gameObject.SetActive(true);
    }

    public void OnPriceChanged(uint price)
    {
        _priceView.text = price.ToString();
    }

    public void ShowPriceOfFirstChest()
    {
        OnPriceChanged(_chestWindowModel.PriceOfFirstChest);
    }

    public void ShowMultipleCards(BaseItemsSet baseItemsSet)
    {
        List<ItemTemplate> items = SetItemsContainer.Instance.GetItemsFromSet(baseItemsSet);

        _multipleCardsView.Show(items);
    }
    
    private void UpdateTotalItemsAmount()
    {
        try
        {
            _itemsCountText.text = $"Items: {_playerData.GetInventoryItems().Count}/{_playerData.CurrentInventorySize}";
        }
        catch (NullReferenceException)
        {

        }
    }
    
    private void ItemAddedToInventory(BaseItem item)
    {
        UpdateTotalItemsAmount();
    }
}
