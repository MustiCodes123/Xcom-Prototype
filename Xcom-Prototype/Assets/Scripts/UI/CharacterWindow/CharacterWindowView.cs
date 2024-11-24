using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI.CharacterWindow;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterWindowView : UIWindowView
{
    private CharacterHandler _characterHandler;

    [SerializeField] private SmalCharacterCard smalCharacterCard;
    [SerializeField] private Transform characterCardsContainer;
    [SerializeField] private BaseItemSlot[] characterSlots;
    [SerializeField] private Button LevelUpButton;
    [SerializeField] private TalentsPanelView talentsPanelView;
    [SerializeField] private TextMeshProUGUI statPointsText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private GameObject[] stars;

    [SerializeField] private Button _extensionButton;
    [SerializeField] private SlotsExtensionPopUp _slotsExtensionPopUp;
    [SerializeField] private SlotsExtensionSettings _extensionSettings;
    [SerializeField] private PassiveSkillsView _passiveSkillsView;
    [SerializeField] private CharactersCounter _charactersCounter;
    [SerializeField] private WeaponCounter _weaponCounter;
    [SerializeField] private ItemInfoPopup _itemInfoPopup;
    [SerializeField] private List<Button> _passiveSkillsButtons = new List<Button>();
    [SerializeField] private AnimateUIElements _uiElements;

    [SerializeField] private List<GameObject> _disabledWhenCharacterUnselected = new List<GameObject>();
    [SerializeField] private CanvasGroup _characterUnselectedBackground;

    private List<SmalCharacterCard> _smallCharacterCards = new List<SmalCharacterCard>();
    private BaseCharacterModel _currentCharacterInfo;
    private PlayerInventoryView _playerInventoryView;
    private CameraHolder _cameraHolder;
    private UICharacterVIew _characterView;
    private UIWindowManager _windowManager;

    [Inject]
    public void Construct(CharacterHandler characterSelectHandler, SaveManager saveManager, PlayerInventoryView playerInventoryView, CameraHolder cameraHolder, UICharacterVIew characterView, UIWindowManager windowManager)
    {
        _playerInventoryView = playerInventoryView;
        _characterHandler = characterSelectHandler;
        _saveManager = saveManager;
        _playerInventoryView.CharacterItemSlots = characterSlots;
        _cameraHolder = cameraHolder;
        _characterView = characterView;
        _windowManager = windowManager;
    }

    private void Start()
    {
        _disabledWhenCharacterUnselected.ForEach(item => item.SetActive(false));
        _characterUnselectedBackground.gameObject.SetActive(true);

        LevelUpButton.onClick.AddListener(OnLevelUpButtonClicked);

        if (playerData.CharacterExtension >= _extensionSettings.CharacterSlotsExtensionLevels.Count)
            _extensionButton.gameObject.SetActive(false);
        else
            _extensionButton.onClick.AddListener(OnExtensionButtonClick);

        _signalBus.Subscribe<CharacterSelectSignal>(OnCharacterChanged);

        for (int i = 0; i < characterSlots.Length; i++)
        {
            characterSlots[i].OnClickAction += ShowInventory;
        }

        _slotsExtensionPopUp.TryPurchaseCharactersSlots += OnTryPurchaseSlots;

        if (playerData.CharacterExtension >= _extensionSettings.CharacterSlotsExtensionLevels.Count)
        {
            _extensionButton.gameObject.SetActive(false);
        }

        foreach(Button button in _passiveSkillsButtons)
        {
            button.onClick.AddListener(_passiveSkillsView.ShowDescription);
        }

        talentsPanelView.SetCharacterToSlotManager(_currentCharacterInfo);
        UpdateCounters();

        SetupPassiveTalents();


        if (playerData.PlayerGroup.GetCharactersFromBothGroup().Count == 1)
        {
            _smallCharacterCards[0].OnHeroButtonClick();
        }


        InitializeFirstCharacter(_currentCharacterInfo);
    }

    private void OnEnable()
    {
        SetupSmallCards();
    }

    private void ShowInventory(BaseItem item, BaseItemSlot slot)
    {
        _playerInventoryView.Show();
    }

    private void OnLevelUpButtonClicked()
    {
        _characterHandler.LevelUpCharacter(_currentCharacterInfo);
    }

    private void OnCharacterChanged(CharacterSelectSignal characterSelectSignal)
    {
        _disabledWhenCharacterUnselected.ForEach(item => item.SetActive(true));
        AnimateCharacterUnselectedBackground();

        _characterView.ResetCharacterParentRotation();
        _currentCharacterInfo = characterSelectSignal.CharacterInfo;

        SetupSmallCards();
        SetupCharacterInventory();
        talentsPanelView.SetupTalents(_currentCharacterInfo);

        characterNameText.text = _currentCharacterInfo.Name;
        statPointsText.text = _currentCharacterInfo.UnasignedStatPoints.ToString();

        SetupPassiveTalents();

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < _currentCharacterInfo.Stars);
        }
    }

    private void SetupPassiveTalents()
    {
        PassiveTalentsFactory passiveTalentsFactory = new PassiveTalentsFactory();
        List<BaseSkillModel> passiveTalents = passiveTalentsFactory.GetPassiveTalentsForCharacter(_currentCharacterInfo.CharacterID);
        _currentCharacterInfo.CharacterTalents.Talents = new List<BaseSkillModel>(passiveTalents);
        _passiveSkillsView.DisplayPassiveSkills(_currentCharacterInfo);
    }

    private void SetupCharacterInventory()
    {
        foreach (var item in characterSlots)
        {
            item.Reset();
        }

        foreach (var item in _currentCharacterInfo.EquipedItems)
        {
            if (item == null) continue;
            var slot = GetSlot(item.Slot);
            if (item is WeaponItem weapon)
            {
                if (weapon.IsEquipedInOffHand || weapon.weaponType == WeaponTypeEnum.Shield)
                    slot = GetSlot(SlotEnum.OffHand);
            }

            slot.SetItem(item, true, OnItemRemove);
        }

        foreach (var slot in characterSlots)
        {
            if (slot is WeaponInventorySlot weapon)
            {
                weapon.CheckShieldAndTwoHanded();
            }
        }
    }

    private BaseItemSlot GetSlot(SlotEnum slotEnum)
    {
        foreach (var slot in characterSlots)
        {
            if (slot.slotType == slotEnum)
                return slot;
        }
        return null;
    }

    private void OnItemRemove(BaseItem item, BaseItemSlot slot)
    {
        _itemInfoPopup.Show(item);
        _itemInfoPopup.ActivateButtons("Remove", () =>
        {
            RemoveItem(item, slot);
        }, 
        () =>
        {
            _itemInfoPopup.gameObject.SetActive(false);
        });
    }

    private void RemoveItem(BaseItem item, BaseItemSlot slot)
    {
        if (item == null)
            return;

        slot.Reset();
        _characterHandler.UnequipItem(item);

        if (_currentCharacterInfo.EquipedItems.Contains(item))
            _currentCharacterInfo.EquipedItems.Remove(item);

        if (_playerInventoryView.gameObject.activeInHierarchy)
            _playerInventoryView.CreateInventory();
        else
            _playerInventoryView.Show();

        _itemInfoPopup.gameObject.SetActive(false);
    }

    private void SetupCharacters()
    {
        SetupSmallCards();

        if (_currentCharacterInfo == null)
        {
            _currentCharacterInfo = playerData.PlayerGroup.GetCharactersFromBothGroup()[0];

            SetupCharacterInventory();
            talentsPanelView.SetupTalents(_currentCharacterInfo);
        }

        _extensionButton.transform.SetSiblingIndex(characterCardsContainer.transform.childCount - 1);
    }

    private void OnExtensionButtonClick()
    {
        _slotsExtensionPopUp.Show(playerData.CharacterExtension, _extensionSettings, ExtensionType.Character);
    }

    private void OnTryPurchaseSlots(int price, GameCurrencies currency)
    {
        bool isEnoughCurrency = Wallet.Instance.SpendCachedCurrency(currency, (uint)price);

        if (isEnoughCurrency)
        {
            if (playerData.CharacterExtension < _extensionSettings.CharacterSlotsExtensionLevels.Count)
            {
                _slotsExtensionPopUp.gameObject.SetActive(false);
                playerData.PlayerGroup.MaxGroupSize += _extensionSettings.CharacterSlotsExtensionLevels[playerData.CharacterExtension].SlotCount;
                SetupCharacters();

                playerData.CharacterExtension++;

                if (playerData.CharacterExtension >= _extensionSettings.CharacterSlotsExtensionLevels.Count)
                    _extensionButton.gameObject.SetActive(false);
            }
            else
            {
                _extensionButton.gameObject.SetActive(false);
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

        UpdateCounters();
    }

    private void SetupSmallCards()
    {
        List<BaseCharacterModel> inventoryCharacters = playerData.PlayerGroup.GetCharactersFromBothGroup();

        while (_smallCharacterCards.Count < playerData.PlayerGroup.MaxGroupSize)
        {
            SmalCharacterCard card = Instantiate(smalCharacterCard, characterCardsContainer);
            card.Construct(_characterHandler);
            card.SetLocked(false);
            _smallCharacterCards.Add(card);
        }

        for (int i = 0; i < playerData.PlayerGroup.MaxGroupSize; i++)
        {
            if (i < inventoryCharacters.Count)
            {
                bool selected = playerData.PlayerGroup.IsCharacterInGroup(inventoryCharacters[i]);
                _smallCharacterCards[i].SetCharacterData(inventoryCharacters[i], selected, _resourceManager);
            }
            else
            {
                _smallCharacterCards[i].ClearSlot();
            }
        }
    }

    public override void Show()
    {
        _disabledWhenCharacterUnselected.ForEach(item => item.SetActive(false));
        _characterUnselectedBackground.alpha = 1f;
        _characterUnselectedBackground.gameObject.SetActive(true);

        _characterView.ResetCharacterParentRotation();
        _cameraHolder.ActivateCharacterCamera();
        transform.DOKill(true);
        _uiElements.AnimatePanelsIn();
        // gameObject.SetActive(true);
        // transform.localScale = Vector3.zero;
        // transform.DOScale(1, animationDuration);
        SetupCharacters();
        UpdateCounters();
        
        if (playerData.PlayerGroup.GetCharactersFromBothGroup().Count == 1)
        {
            _smallCharacterCards[0].OnHeroButtonClick();
        }

        base.Show();
    }

    public override void Hide()
    {
        transform.DOKill(true);
        // transform.DOScale(0, animationDuration).OnComplete(() => { gameObject.SetActive(false); });
        _uiElements.AnimatePanelsOut();
        _saveManager.SaveGame();
        _playerInventoryView.gameObject.SetActive(false);
        talentsPanelView.Hide();
        _cameraHolder.ActivateMainCamera();
        _characterView.DestroyInvalidBG();
        base.Hide();
    }

    private void UpdateCounters()
    {
        int characterCount = playerData.PlayerGroup.GetCharactersFromBothGroup().Count;
        int itemsCount = playerData.GetInventoryItems().Count;

        _charactersCounter.UpdateCounterView(characterCount, playerData.PlayerGroup.MaxGroupSize);
        _weaponCounter.UpdateCounterView(itemsCount, playerData.CurrentInventorySize);
    }

    public void InitializeFirstCharacter(BaseCharacterModel character)
    {
        characterNameText.text = _currentCharacterInfo.Name;
        statPointsText.text = _currentCharacterInfo.UnasignedStatPoints.ToString();

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < _currentCharacterInfo.Stars);
        }
    }

    private void ShowShopWindow()
    {
        _windowManager.ShowWindow(WindowsEnum.ShopWindow);
    }

    private void AnimateCharacterUnselectedBackground()
    {
        _characterUnselectedBackground.DOFade(0f, 0.3f).OnComplete(() =>
        {
            _characterUnselectedBackground.gameObject.SetActive(false);
        });
    }
    
    protected override void OnDestroy()
    {
        _signalBus.Unsubscribe<CharacterSelectSignal>(OnCharacterChanged);

        LevelUpButton.onClick.RemoveAllListeners();

        for (int i = 0; i < characterSlots.Length; i++)
        {
            characterSlots[i].OnClickAction -= ShowInventory;
        }

        foreach (Button button in _passiveSkillsButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        _slotsExtensionPopUp.TryPurchaseCharactersSlots -= OnTryPurchaseSlots;
    }
}