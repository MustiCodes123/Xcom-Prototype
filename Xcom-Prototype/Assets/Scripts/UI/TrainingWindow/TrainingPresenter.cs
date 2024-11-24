using System;
using System.Linq;
using Data.Resources.AddressableManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Unity.VisualScripting;

public class TrainingPresenter : MonoBehaviour
{
    public Action CharacterChanged;
    public Action CheckMaxLevel;

    [Inject(Id = "LevelUpStrategy")] private ICharacterUpgradeStrategy _levelUpStrategy;
    [Inject(Id = "RankUpStrategy")] private ICharacterUpgradeStrategy _rankUpStrategy;
    [Inject] private TrainingDataContainer _dataContainer;
    [Inject] private SignalBus _signalBus;
    [Inject] private CharacterHandler _characterHandler;
    [Inject] private PlayerData _playerData;
    [Inject] private ResourceManager _resourceManager;
    [Inject] private ITrainingView _view;
    [Inject] private SkillsDataInfo _skillsData;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TMP_Text _upgradeButtonTMP;
    private enum TrainingState
    {
        LevelUp,
        RankUp
    }
    private TrainingState CurrentState
    {
        get
        {
            if (_dataContainer.CurrentCharacter?.IsMaxLevel == true)
            {
                if (_dataContainer.CurrentCharacter?.IsMaxRank == true)
                    return TrainingState.LevelUp;
                return TrainingState.RankUp;
            }
            return TrainingState.LevelUp;
        }
    }

    private bool _isInitialized = false;

    #region MonoBehaviour Methods
    private void Start()
    {
        Initialize();
        _view.Initialize();
        UpdateStatsAndButton();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        OnCharacterRemoved();
        Unsubscribe();
        CheckMaxLevel?.Invoke();
    }
    #endregion

    #region Initialization
    public void Initialize()
    {
        _dataContainer.TrainingCharacterCard.AddAction(OnCharacterRemoved);
        _signalBus.Subscribe<CharacterSelectSignal>(OnCharacterChanged);

        for (int i = 0; i < _dataContainer.SmallCharacterCards.Length; i++)
        {
            _dataContainer.SmallCharacterCards[i].SetCharacterData();
            _dataContainer.SmallCharacterCards[i].SubscribeToClick(OnSmallCharacterRemoved);
        }
    }

    public void Subscribe()
    {
        _dataContainer.TrainingCharacterCard.OnClosedCharCard += _view.SetActiveUIWithoutAnim;
        _upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
    }

    public void Unsubscribe()
    {
        _dataContainer.TrainingCharacterCard.OnClosedCharCard -= _view.SetActiveUIWithoutAnim;
        _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
    }
    #endregion

    #region Commands
    public ICommand GetUpgradeCommand()
    {
        if (CurrentState == TrainingState.RankUp)
            return new RankUpCommand(_rankUpStrategy, _dataContainer.CurrentCharacter, _dataContainer.SmallCharacterCards, _playerData, _signalBus);

        return new LevelUpCommand(_levelUpStrategy, _dataContainer.CurrentCharacter, _dataContainer.SmallCharacterCards, _playerData, _signalBus);
    }
    #endregion

    #region Callbacks
    public void OnCharacterRemoved()
    {
        _dataContainer.CurrentCharacter = null;

        _view.CharacterRemoved();

        foreach (var characterCard in _dataContainer.SmallCharacterCards)
        {
            characterCard.SetCharacterData();
            characterCard.gameObject.SetActive(false);
        }

        _view.ShowCharacters();
        UpdateStatsAndButton();
    }

    public void OnCharacterSelected(BaseCharacterModel model)
    {
        if (_isInitialized)
        {
            _view.SetActiveUIWithoutAnim(true);
        }
        else
        {
            _view.SetActiveUIwithAnim(true);
            _isInitialized = true;
        }

        if (_dataContainer.TrainingCharacterCard.HasCharacter)
        {
            SetCharacterToSlot(model);
        }
        else
        {
            SelectCharacter(model);
        }

        CheckMaxLevel?.Invoke();
        UpdateStatsAndButton();
    }

    private void OnCharacterChanged(CharacterSelectSignal characterSelectSignal)
    {
        _dataContainer.CurrentCharacter = characterSelectSignal.CharacterInfo;
        CheckMaxLevel?.Invoke();
        UpdateStatsAndButton();
    }

    private BaseCharacterModel GetNewCharacter(RareEnum nextRare, int addXP)
    {
        foreach (CharacterPreset charPreset in _skillsData.CharacterPresets)
            if (charPreset.CharacterID == _dataContainer.CurrentCharacter.CharacterID)
                return charPreset.CreateCharacter(nextRare, _dataContainer.CurrentCharacter.Level, _dataContainer.CurrentCharacter.Xp, addXP);
        return null;
    }

    private void UpdateStatsAndButton()
    {
        if (_dataContainer.CurrentCharacter == null) return;

        ICommand upgradeCommand = GetUpgradeCommand();
        UpgradeButtonUpdate(upgradeCommand);

        int addXP = upgradeCommand.CalculateXP();
        RareEnum nextRare = _dataContainer.CurrentCharacter.Rare;
        if (CurrentState == TrainingState.RankUp)
            nextRare = _dataContainer.CurrentCharacter.Rare + 1;

        BaseCharacterModel newCharacter = GetNewCharacter(nextRare, addXP);
        
        _view.SetupXPBar(_dataContainer.CurrentCharacter, newCharacter);

        int upLevels = Math.Max(newCharacter.Level - _dataContainer.CurrentCharacter.Level, 1);
        uint price = _dataContainer.CharacterUpgradePrices.CalculatePrice(_dataContainer.CurrentCharacter, upLevels);
        _view.UpdatePrice(price);
        
        _view.SetCharacterStats(newCharacter);

        // If the selected characters already give the maximum level, then the remaining slots are locked
        if (CurrentState == TrainingState.LevelUp)
        {
            for (int i = 0; i < _dataContainer.SmallCharacterCards.Length; i++)
            {
                bool shouldLock = _dataContainer.SmallCharacterCards[i].IsEmpty && newCharacter.IsMaxLevel;
                _dataContainer.SmallCharacterCards[i].SetLocked(shouldLock);
                _dataContainer.Locks[i].gameObject.SetActive(shouldLock);
            }
        }
    }

    private void UpgradeButtonUpdate(ICommand upgradeCommand)
    {
        _upgradeButton.gameObject.SetActive(_dataContainer.CurrentCharacter != null);

        _upgradeButtonTMP.text = "LEVEL UP";
        if (CurrentState == TrainingState.RankUp)
            _upgradeButtonTMP.text = "RANK UP";

        _upgradeButton.interactable = false;
        if (upgradeCommand.CanExecute())
            _upgradeButton.interactable = true;
    }

    private void OnUpgradeButtonClick()
    {
        ICommand upgradeCommand = GetUpgradeCommand();
        
        if (!upgradeCommand.CanExecute())
            return;

        upgradeCommand.Execute();
        
        _dataContainer.TrainingCharacterCard.UpdateStars(_dataContainer.CurrentCharacter);
    }

    private void OnSmallCharacterRemoved(BaseCharacterModel model)
    {
        for (int i = 0; i < _dataContainer.SmallCharacterCards.Length; i++)
        {
            if (_dataContainer.SmallCharacterCards[i].baseCharacterInfo == model)
            {
                _dataContainer.SmallCharacterCards[i].SetCharacterData();
                _dataContainer.TrainingCharacterList.BackCharacterToList(model);
                _dataContainer.SmallCharacterCards[i].gameObject.SetActive(false);
                break;
            }
        }
        UpdateStatsAndButton();
    }

    #endregion

    #region Utility Methods
    private void SelectCharacter(BaseCharacterModel model)
    {
        _dataContainer.CurrentCharacter = model;
        _dataContainer.TrainingCharacterCard.SetData(model);
        _dataContainer.TrainingCharacterList.RemoveCharacterFromList(model);
        _characterHandler.ChangeCharacter(model);

        _view.SetCharacterStats(model);
        _view.SetupXPBar(model);

        for (int i = 0; i < _dataContainer.SmallCharacterCards.Length; i++)
        {
            _dataContainer.SmallCharacterCards[i].SetLocked(false);
            if (CurrentState == TrainingState.RankUp)
                _dataContainer.SmallCharacterCards[i].SetLocked(_dataContainer.CurrentCharacter.Stars <= i);
        }
    }

    private void SetCharacterToSlot(BaseCharacterModel model)
    {
        int currentCharactersCount = _dataContainer.SmallCharacterCards.Count(card => !card.IsEmpty);
        int maxCharacters = _dataContainer.Locks.Count(lockObj => !lockObj.activeSelf);

        if (currentCharactersCount >= maxCharacters)
            return;

        SmalCharacterCard firstEmptyCard = _dataContainer.SmallCharacterCards.First(card => card.IsEmpty);
        firstEmptyCard.SetCharacterData(model, false, _resourceManager);
    }
    #endregion
}