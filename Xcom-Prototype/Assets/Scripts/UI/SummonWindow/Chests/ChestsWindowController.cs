using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChestsWindowController : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    [SerializeField] private ChestWindowModel _model;
    [SerializeField] private ChestWindowView _view;

    [SerializeField] private ChestSummonUIAnimation _summonAnimation;

    [SerializeField] private Button _summonButton;
    [SerializeField] private Button _hideSinglePrefabButton;
    [SerializeField] private Button _exceptionalSummonButton;
    [SerializeField] private Button _crystalsWindowButton;
    [SerializeField] private Button _chestsWindowButton;
    [SerializeField] private Button _exceptedChestsWindowButton;

    [SerializeField] private ChestButton[] _chestButtons;
    [SerializeField] private List<ChestCardPrefab> _chestCardPrefabs = new List<ChestCardPrefab>();
    [SerializeField] private GameObject _singleCardPrefab;
    
    
    private ChestButton _currentSelectedButton;
    private bool _isSummoning = false;

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _model.Initialize();

        InitializeChestButtons();

        _model.SelectedChestData = _chestButtons.FirstOrDefault(button => button.transform.GetSiblingIndex() == 0).Data;
      
    }
    

    public void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Initialization
    private void InitializeChestButtons()
    {
        SetInitialChestButtonsStates();

        SubscribeOnEvents();

        _currentSelectedButton = _chestButtons[0];
    }

    private void SetInitialChestButtonsStates()
    {
        foreach (var button in _chestButtons)
        {
            if (button.transform.GetSiblingIndex() == 0)
            {
                button.ButtonView.SetState(new SelectedChestButtonState());
                _view.ShowChest(button.Chest3DModel);

                continue;
            }

            button.ButtonView.SetState(new UnselectedChestButtonState());
        }
    }
    #endregion

    #region Events
    private void SubscribeOnEvents()
    {
        foreach(var button in _chestButtons)
        {
            button.Click += OnChestButtonClick;
        }

        _summonButton.onClick.AddListener(OnSummonButtonClick);
        _exceptionalSummonButton.onClick.AddListener(OnExceptionalSummonButtonClick);
        _hideSinglePrefabButton.onClick.AddListener(() => _singleCardPrefab.SetActive(false));
        _crystalsWindowButton.onClick.AddListener(OnHideCardsButtonClick);
        _chestsWindowButton.onClick.AddListener(OnHideCardsButtonClick);
        _exceptedChestsWindowButton.onClick.AddListener(OnHideCardsButtonClick);
    }

    private void UnsubscribeFromEvents()
    {
        foreach (var button in _chestButtons)
        {
            button.Click -= OnChestButtonClick;
        }

        _summonButton.onClick.RemoveListener(OnSummonButtonClick);
        _exceptionalSummonButton.onClick.RemoveListener(OnExceptionalSummonButtonClick);
        _hideSinglePrefabButton.onClick.RemoveListener(() => _singleCardPrefab.SetActive(false));
        _crystalsWindowButton.onClick.RemoveListener(OnHideCardsButtonClick);
        _chestsWindowButton.onClick.RemoveListener(OnHideCardsButtonClick);
        _exceptedChestsWindowButton.onClick.RemoveListener(OnHideCardsButtonClick);
    }

    private void OnChestButtonClick(ChestButton clickedButton)
    {
        _currentSelectedButton = clickedButton;

        _chestCardPrefabs.ForEach(chestCard => chestCard.gameObject.SetActive(false));

        UpdateButtonStates(clickedButton);
    }

    private async void OnSummonButtonClick()
    {
        if (_isSummoning)
            return;

        _isSummoning = true;

        bool purchaseOperationResult = Wallet.Instance.SpendCachedCurrency(_model.SelectedChestData.CurrencyType, _model.SelectedChestData.Price);
        if (!purchaseOperationResult)
            return;

        bool substractOperationResult = await PlayerInventory.Instance.TryRemoveChests(_model.SelectedChestData.ChestType);

        if (!substractOperationResult)
            return;

        _view.ShowOpenAnimation(_currentSelectedButton.Chest3DModel);

        SummonService summonService = new SummonService(_model.AllItems, _model.DropChances);
        object summonedItem = summonService.Summon(_model.SelectedChestData.ChestType);

        if (summonedItem is ItemTemplate template)
        {
            _view.ShowCard(summonedItem as ItemTemplate);
            _playerData.AddItemToInventory(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(template));
        }
        else if (summonedItem is BaseItemsSet set)
        {
            _view.ShowMultipleCards(summonedItem as BaseItemsSet);

            List<ItemTemplate> items = SetItemsContainer.Instance.GetItemsFromSet(set);
            foreach(var item in items)
            {
                _playerData.AddItemToInventory(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
            }
        }

        _isSummoning = false;

        await PlayerInventory.Instance.UpdateInventory(true);
    }

    private void OnHideCardsButtonClick()
    {
        _chestCardPrefabs.ForEach(card => card.gameObject.SetActive(false));
        //_summonAnimation.ReturnToBaseView();
    }

    private async void OnExceptionalSummonButtonClick()
    {
        if (_isSummoning)
            return;

        _isSummoning = true;

        bool purchaseOperationResult = Wallet.Instance.SpendCachedCurrency(_model.SelectedChestData.CurrencyType, _model.SelectedChestData.Price);
        if (!purchaseOperationResult)
            return;

        bool substractOperationResult = await PlayerInventory.Instance.TryRemoveChests(_model.SelectedChestData.ChestType);

        if (!substractOperationResult)
            return;

        _view.ShowOpenAnimation(_currentSelectedButton.Chest3DModel);

        SummonService summonService = new SummonService(_model.AllItems, _model.DropChances);
        object summonedItem = summonService.Summon(_model.SelectedChestData.ChestType);

        if (summonedItem is BaseItemsSet)
        {
            _view.ShowMultipleCards(summonedItem as BaseItemsSet);
            PlayerInventory.Instance.AddSetToInventory((summonedItem as BaseItemsSet).PlayFabID);
        }

        _isSummoning = false;
    }
    #endregion

    #region Utility Methods
    private void UpdateButtonStates(ChestButton clickedButton)
    {
        foreach (var button in _chestButtons)
        {
            if(button == clickedButton)
            {
                button.ButtonView.SetState(new SelectedChestButtonState());

                _view.ShowChest(button.Chest3DModel);
                _model.SelectedChestData = button.Data;

                continue;
            }

            button.ButtonView.SetState(new UnselectedChestButtonState());
        }
    }
    #endregion
}