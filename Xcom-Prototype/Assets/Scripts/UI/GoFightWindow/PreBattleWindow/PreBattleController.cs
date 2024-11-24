using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Data.Resources.AddressableManagement;
using Zenject;
using TMPro;

[Serializable]
public class CurrencyIcons
{
    [field: SerializeField] public GameCurrencies Currency { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}

public class PreBattleController : MonoBehaviour
{
    [HideInInspector] [Inject] public TemploaryInfo TemploaryInfo;
    [HideInInspector] [Inject] public UIWindowManager UIWindowManager;
    [HideInInspector] [Inject] public PlayerData PlayerData;

    [Inject] public  SignalBus SignalBus;
    [Inject] private FightWindowDataProvider _dataProvider;
    [Inject] private PvPBattleData _pvpBattleData;
    [Inject] private ThreeToOneContainer _threeToOneContainer;
    [Inject] private ResourceManager _resourceManager;
    [Inject] private UIWindowManager _uiWindowManager;

    public PreBattleScreen PreBattleScreen;
    public SortPanel SortPanel;
    public SelectedPlayerTeamView CharacterView;
    public CharacterButton CharacterButtonPrefab;
    public Transform CharacterButtonsRoot;

    [HideInInspector] public FightWindowPool FightWindowPool;
    [HideInInspector] public List<CharacterButton> ActiveCharacterButtons = new List<CharacterButton>();
    [HideInInspector] public int SelectedCharactersCount = 0;
    [HideInInspector] public int MaxSelectedCharacters = 5;

    [SerializeField] private Button _startFightButton;
    [SerializeField] private TMP_Text _startButtonPriceTMP;
    [SerializeField] private Image _startButtonCurrencyIcon;
    [SerializeField] private Image[] _enemiesIcons;
    [SerializeField] private CurrencyIcons[] _currencySprites;

    private PreBattleState _currentState;

    #region MonoBehaviour Methods
    private void Awake()
    {
        SetState(new DefaultPreBattleState(this));
    }
    #endregion

    #region Initialization
    [Inject]
    public void Constructor(FightWindowPool pool)
    {
        FightWindowPool = pool;
    }

    public void Initialize()
    {
        _startButtonCurrencyIcon.sprite = _currencySprites.FirstOrDefault(currencySprites => currencySprites.Currency == GameCurrencies.Energy).Icon;

        if (TemploaryInfo.CurrentMode.GameMode == GameMode.ThreeToOne)
        {
            MaxSelectedCharacters = GameConstants.ThreeToOneCharacterLimit;
            _startButtonCurrencyIcon.sprite = _currencySprites.FirstOrDefault(currencySprites => currencySprites.Currency == GameCurrencies.Key).Icon;
            _startButtonPriceTMP.text = TemploaryInfo.CurrentBoss.KeysCost.ToString();

            for (int i = 1; i < _enemiesIcons.Length; i++)
            {
                _enemiesIcons[i].transform.parent.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            MaxSelectedCharacters = GameConstants.CharacterLimit;
        }

        InitializeSortPanel();

        if (TemploaryInfo.CurrentMode.GameMode == GameMode.TestOfStrenght)
        {
            SetState(new TestOfStrengthPreBattleState(this));
            _startButtonCurrencyIcon.sprite = _currencySprites.FirstOrDefault(currencySprites => currencySprites.Currency == GameCurrencies.Key).Icon;
            _startButtonPriceTMP.text = TemploaryInfo.CurrentBoss.KeysCost.ToString();
        }
        else if(TemploaryInfo.CurrentMode.GameMode == GameMode.PvP)
        {
            SetState(new DefaultPreBattleState(this));
            _startButtonCurrencyIcon.sprite = _currencySprites.FirstOrDefault(currencySprites => currencySprites.Currency == GameCurrencies.Key).Icon;
            _startButtonPriceTMP.text = _pvpBattleData.KeysCost.ToString();

        }
        else if (TemploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            SetState(new DefaultPreBattleState(this));
            _startButtonCurrencyIcon.sprite = _currencySprites.FirstOrDefault(currencySprites => currencySprites.Currency == GameCurrencies.Energy).Icon;
            _startButtonPriceTMP.text = TemploaryInfo.LevelInfo.EnergyCost.ToString();
        }

        _currentState.Initialize();

        _startFightButton.onClick.AddListener(OnStartButtonClicked);       
    }

    private void InitializeSortPanel()
    {
        SortPanel.SublcribeOnLevelSortEvent(OnSortByLevelButtonClick);
        SortPanel.SublcribeOnNameSortEvent(OnSortByNameButtonClick);
        SortPanel.SublcribeOnRaritySortEvent(OnSortByRarityButtonClick);
    }

    public void InitializeEnemiesIcons()
    {
        foreach (var icon in _enemiesIcons)
        {
            icon.transform.parent.parent.gameObject.SetActive(false);           
        }

        if (_dataProvider.LevelEnemiesIcons == null)
        {
            var levelController = gameObject.transform.GetComponent<LevelSelectController>();

            if (TemploaryInfo.CurrentMode.GameMode == GameMode.Default)
            {
                levelController.SetupEnemiesIconsForCompany();
            }
            else if(TemploaryInfo.CurrentMode.GameMode == GameMode.PvP)
            {
                levelController.SetupEnemiesIconsForPVP();
            }
            
        }

        for (int i = 0; i < _dataProvider.LevelEnemiesIcons.Count; i++)
        {
            _enemiesIcons[i].sprite = _dataProvider.LevelEnemiesIcons[i];
            _enemiesIcons[i].transform.parent.parent.gameObject.SetActive(true);
            _enemiesIcons[i].gameObject.SetActive(true);
        }

    }

    public void InitializeBossIcon()
    {
        List<GameObject> parentObjects = new List<GameObject>();
        foreach (var icon in _enemiesIcons)
        {
            GameObject parentObject = icon.transform.parent.gameObject;

            parentObjects.Add(parentObject);

            parentObject.SetActive(false);
            icon.gameObject.SetActive(false);
        }

        Sprite bossIcon = TemploaryInfo.CurrentBoss.BossPreset.CharacterSprite;

        parentObjects[0].SetActive(true);
        _enemiesIcons[0].sprite = bossIcon;
        _enemiesIcons[0].gameObject.SetActive(true);
    }
    #endregion

    #region Callbacks
    public void OnClose()
    {
        _currentState.OnClose();
    }

    private void OnCharacterButtonClick(CharacterButton button)
    {
        _currentState.OnCharacterButtonClick(button);
    }

    public void OnStartButtonClicked()
    {
        _currentState.OnStartButtonClicked();
    }

    public void OnSortByNameButtonClick()
    {
        _currentState.OnSortByNameButtonClick();
    }

    public void OnSortByRarityButtonClick()
    {
        _currentState.OnSortByRarityButtonClick();
    }

    public void OnSortByLevelButtonClick()
    {
        _currentState.OnSortByLevelButtonClick();
    }
    #endregion

    #region Utility Methods
    private void SetState(PreBattleState state)
    {
        _currentState = state;
    }

    public async void SetupCampaignGameMode()
    {
        if (Wallet.Instance.SpendCachedCurrency(GameCurrencies.Energy, (uint)TemploaryInfo.LevelInfo.EnergyCost))
        {
            if (!string.IsNullOrEmpty(TemploaryInfo.LevelInfo.SceneName))
            {
                _resourceManager.ShowLoadingScreen();
                _resourceManager.LoadLevelAsync(TemploaryInfo.LevelInfo.SceneName);
            }
            else
            {
                _resourceManager.ShowLoadingScreen();
                await _resourceManager.LoadMainMenuSceneAsync();
            }
        }
        else
        {
            InfoPopup.Instance.ShowTooltipNotEnpughtEnergy();
            InfoPopup.Instance.ActivateButtons("Go to the store", "Cancel", () =>
            {
                PreBattleScreen.Hide();
                _uiWindowManager.ShowWindow(WindowsEnum.ShopWindow); 
            }, 
            null);
        }
    }

    public void SetupPvPGameMode()
    {
        if (Wallet.Instance.SpendCachedCurrency(GameCurrencies.Key, (uint)_pvpBattleData.KeysCost))
        {
            int randomSceneIndex = UnityEngine.Random.Range(0, _pvpBattleData.Scenes.Length);
            string pvpSceneName = _pvpBattleData.Scenes[randomSceneIndex];

            if (!string.IsNullOrEmpty(pvpSceneName))
            {
                _resourceManager.ShowLoadingScreen();
                _resourceManager.LoadLevelAsync(pvpSceneName);
            }
            else
            {
                Debug.LogError($"PvPBattleData does not contain scene with name {pvpSceneName}");
            }
        }
        else
        {
            Debug.LogError("Not enough keys");
        }
    }

    public void SetupThreeToOneGameMode()
    {
        if (Wallet.Instance.SpendCachedCurrency(GameCurrencies.Key, (uint)_pvpBattleData.KeysCost))
        {
            BossData bossData = TemploaryInfo.CurrentBoss;

            string bossSceneName = bossData.SceneName;

            if (!string.IsNullOrEmpty(bossSceneName))
            {
                _resourceManager.ShowLoadingScreen();
                _resourceManager.LoadLevelAsync(bossSceneName);
            }
            else
            {
                Debug.LogError(
                    $"Latest BossData in ThreeToOneContainer does not contain scene with name {bossSceneName}");
            }
        }
        else
        {
            Debug.LogError("Not enough energy");
        }
    }

    public ResourceManager GetResourceManager()
    {
        return _resourceManager;
    }
    #endregion
}