using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using UI.CharacterWindow;

public class PortalWindowController : UIWindowView
{
    [Inject] private PlayerData _playerData;
    [Inject] private SkillsDataInfo _skillsData;
    [Inject] private CameraHolder _cameraHolder;
    [Inject] private UniRxDisposable _uniRxDisposable;
    [Inject] private UICharacterVIew _characterView;

    [SerializeField] private bool _isNeedToAddCristals = true;

    [SerializeField] private Transform _scrollViewRoot;
    [SerializeField] private Button _closeHeroButton;
    [SerializeField] private RectTransform _crystalsPanel;
    [SerializeField] private RectTransform _summonButtonBlock;
    [SerializeField] private RectTransform _portalTakeButtonBlock;
    [SerializeField] private PortalCharacterCardView _portalCharacterCardView;
    [SerializeField] private PortalSummonButton _summonButton;
    [SerializeField] private Button _portalTakeButton;
    [SerializeField] private CristalData _currentCristal;
    [SerializeField] private CristalButtonView _selectedCristalButton;
    [SerializeField] private CharacterStatsViewer _characterStats;
    [SerializeField] private FadeDecorations _blackDecorations;
    [SerializeField] private GameObject _portalParticle;
    [SerializeField] private GameObject _characterInfo;
    [SerializeField] private GameObject _changeWindowButtons;
    [SerializeField] private GameObject _crystalVFX;
    [SerializeField] private GameObject _chestVFX;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private CharactersCounter _charactersCounter;

    [SerializeField] private CristalButtonView[] _cristalButtonViews;
    [SerializeField] private CristalButton[] _cristalButtons;
    [SerializeField] private GameObject[] _rareStars;
    
    [SerializeField] private AnimateUIElements _crystalUiElements;
    

    private CharacterPreset _currentCharacterPreset;

    private List<CharacterPreset> _allCommonCharacters = new();
    private List<CharacterPreset> _allRareCharacters = new();
    private List<CharacterPreset> _allEpicCharacters = new();
    private List<CharacterPreset> _allLegendaryCharacters = new();
    private List<CharacterPreset> _allMythicalCharacters = new();

    private const int _characterShowStatisticDelay = 2;

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        InitializeCharactersLists();
        SubscribeOnEvents();

        _backgroundImage.enabled = true;
        _closeHeroButton.gameObject.SetActive(false);
        _cameraHolder.ActivateSummonCamera();
        
        UpdateCristalCountView();
        _charactersCounter.UpdateCounterView(playerData.PlayerGroup.GetCharactersFromBothGroup().Count, playerData.PlayerGroup.MaxGroupSize, "Hero: ");
    } 

    private void OnDisable()
    {
        UnsubscribeFromEvents();
        _closeHeroButton.gameObject.SetActive(false);
        _currentCristal.CrystalAnimation.ShowCrystalPanel(_crystalsPanel);
    }
    #endregion

    #region Overrided Methods
    protected override void Awake()
    {
        base.Awake();
        _closeHeroButton.onClick.AddListener(OnCloseHeroButtonClick);
        _portalTakeButton.onClick.AddListener(OnCloseHeroButtonClick);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _closeHeroButton.onClick.RemoveAllListeners();
        _portalTakeButton.onClick.RemoveAllListeners();
    }

    public override void Show()
    {
        base.Show();
        _crystalUiElements.AnimatePanelsIn();
    }

    public override void Hide()
    {
        base.Hide();
        _crystalUiElements.AnimatePanelsOut();
        _cameraHolder.ActivateMainCamera();
        _blackDecorations.gameObject.SetActive(false);
        _characterView.DestroyInvalidBG();
        gameObject.SetActive(false);
    }

    protected override void OnCloseClicked()
    {
        base.OnCloseClicked();
        _cameraHolder.ActivateMainCamera();
    }
    #endregion

    #region Initialization
    private void InitializeCharactersLists()
    {
        foreach(CharacterPreset character in _skillsData.CharacterPresets)
        {
            if (character.Rare == RareEnum.Common)
                _allCommonCharacters.Add(character);

            else if (character.Rare == RareEnum.Rare)
                _allRareCharacters.Add(character);

            else if (character.Rare == RareEnum.Epic)
                _allEpicCharacters.Add(character);

            else if (character.Rare == RareEnum.Legendary)
                _allLegendaryCharacters.Add(character);

            else if (character.Rare == RareEnum.Mythical)
                _allMythicalCharacters.Add(character);
        }
    }

    private List<CharacterPreset> GetCharactersByRare(RareEnum rare)
    {
        switch (rare)
        {
            case RareEnum.Common: return _allCommonCharacters;
            case RareEnum.Rare:return _allRareCharacters;
            case RareEnum.Epic: return _allEpicCharacters;
            case RareEnum.Legendary: return _allLegendaryCharacters;
            case RareEnum.Mythical: return _allMythicalCharacters;
            default: return _allCommonCharacters;
        }
    }
    #endregion

    #region Events
    private void OnCristalButtonClick(CristalButton clickedButton, CristalData clickedButtonCristal)
    {
        _currentCristal.gameObject.SetActive(false);
        _currentCristal.CrystalAnimation.Refresh();
        _currentCristal = clickedButtonCristal;

        SwitchCristalButtonsView(clickedButton);

        _currentCristal.gameObject.SetActive(true);
        _changeWindowButtons.gameObject.SetActive(true);
        _characterInfo.gameObject.SetActive(false);
        _characterStats.gameObject.SetActive(false);
        _portalCharacterCardView.HideCard();
    }

    private async void OnSummonButtonClick(PortalSummonButton summonButton)
    {
        _summonButton.GetButton().enabled = false;
        int cristalCount = PlayerInventory.Instance.GetCristalsData(_currentCristal.CristalEnum).Amount;

        if (cristalCount > 0)
        {
            bool success = await PlayerInventory.Instance.TryRemoveCristals(_currentCristal.CristalEnum, 1);

            if (success)
            {
                SummonCharacter();

                _portalParticle.SetActive(false);
                _characterStats.gameObject.SetActive(false);
                _blackDecorations.gameObject.SetActive(true);
                closeButton.gameObject.SetActive(false);

                _blackDecorations.ShowBlackDecorations();
                _currentCristal.CrystalAnimation.RefreshImmidiatly();
                _crystalUiElements.AnimatePanelsOut();
                _currentCristal.CrystalAnimation.AnimateButton(_summonButtonBlock);
                _currentCristal.CrystalAnimation.ActivateDestroyAnimation(_backgroundImage, this);

                UpdateCristalCountView();
            }
            else
            {
                Debug.Log($"Not enough cristals");
                _summonButton.GetButton().enabled = true;
            }
        }
    }

    private void OnCharacterClicked(BaseCharacterModel characterModel)
    {
        _playerData.PlayerGroup.AddCharacterToNotAsignedGroup(characterModel);

        _portalCharacterCardView.HideCard();
        _saveManager.SaveGame();
    }

    private void OnCloseHeroButtonClick()
    {
        _summonButton.GetButton().enabled = true;
        _crystalUiElements.AnimatePanelsIn();
        _closeHeroButton.gameObject.SetActive(false);
        _characterInfo.gameObject.SetActive(false);
        _characterStats.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(true);
        _currentCristal.CrystalAnimation.Refresh();
        _currentCristal.CrystalAnimation.AnimateButton(_portalTakeButtonBlock);
        _currentCristal.CrystalAnimation.ShowButton(_summonButtonBlock);

    }
    #endregion

    #region Utility Methods
    private void SummonCharacter()
    {
        var playerLuck = UnityEngine.Random.Range(0f, 1f);

        var characterCategories = new List<List<CharacterPreset>>
        {
            _allCommonCharacters,
            _allRareCharacters,
            _allEpicCharacters,
            _allLegendaryCharacters,
            _allMythicalCharacters
        };

        Dictionary<float, List<CharacterPreset>> rarityChances = new Dictionary<float, List<CharacterPreset>>();
        float totalRarityChance = 0f;

        var iterations = Mathf.Min(_currentCristal.SummonChances.Length, _currentCristal.SummonedEnums.Length);
        for (var i = 0; i < iterations; i++)
        {
            totalRarityChance += _currentCristal.SummonChances[i];
            rarityChances.Add(totalRarityChance, GetCharactersByRare(_currentCristal.SummonedEnums[i]));
        }

        List<CharacterPreset> selectedList = null;

        foreach (var chance in rarityChances.Keys)
        {
            if (playerLuck <= chance)
            {
                selectedList = rarityChances[chance];
                break;
            }
        }

        if (selectedList != null && selectedList.Count > 0)
        {
            _currentCharacterPreset = selectedList[UnityEngine.Random.Range(0, selectedList.Count)];

            SaveCharacterToInventory();
            
            windowManager.GetSignalBus().Fire(new SummonHeroSignal { Hero = _currentCharacterPreset });
        }
        else
        {
            Debug.LogError("No characters available to summon in the selected rarity category.");
        }
    }

    private void SubscribeOnEvents()
    {
        foreach (CristalButton button in _cristalButtons)
        {
            button.Click += OnCristalButtonClick;
        }

        _summonButton.Click += OnSummonButtonClick;
    }

    private void UnsubscribeFromEvents()
    {
        foreach (CristalButton button in _cristalButtons)
        {
            button.Click -= OnCristalButtonClick;
        }

        _summonButton.Click -= OnSummonButtonClick;
    }

    private void SwitchCristalButtonsView(CristalButton clickedButton)
    {
        foreach (CristalButtonView buttonView in _cristalButtonViews)
        {
            buttonView.SwitchButtonView(buttonView.CristalButton == clickedButton);
        }
    }

    private void UpdateCristalCountView()
    {
        foreach (CristalButtonView buttonView in _cristalButtonViews)
        {
            SummonCristalsEnum cristalType = buttonView.CristalButton.ButtonCristalData.CristalEnum;

            int cristalCount = PlayerInventory.Instance.GetCristalsData(cristalType).Amount;

            Debug.Log($"{cristalType} = {cristalCount}");

            buttonView.ShowCristalCount(cristalCount);
        }
    }

    private void SaveCharacterToInventory()
    {
        _playerData.PlayerGroup.AddCharacterToNotAsignedGroup(_currentCharacterPreset.CreateCharacter());
        _saveManager.SaveGame();
    }

    public void ActivatePortal()
    {
        _portalParticle.SetActive(true);

        _uniRxDisposable.MainMenuTimerDisposable.Clear();
        Observable.Timer(TimeSpan.FromSeconds(_characterShowStatisticDelay)).Subscribe(_ =>
        {
            _blackDecorations.HideBlackDecorations();
            ShowCharacterStats();
            _uniRxDisposable.MainMenuTimerDisposable.Clear();
            _closeHeroButton.gameObject.SetActive(true);
            _currentCristal.CrystalAnimation.ShowButton(_portalTakeButtonBlock);

        }).AddTo(_uniRxDisposable.MainMenuTimerDisposable);
    }

    public void ShowCharacterStats()
    {
        _characterInfo.gameObject.SetActive(true);
        _characterName.text = _currentCharacterPreset.PresetName;
        _characterStats.gameObject.SetActive(true);
        _characterStats.SetCharacterStats(_currentCharacterPreset);

        for (int i = 0; i < _rareStars.Length; i++)
        {
            _rareStars[i].SetActive(i <= (int)_currentCharacterPreset.Rare);
        }
    }
    #endregion
}