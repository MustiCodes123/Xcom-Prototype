using DG.Tweening;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIGameWinWIndow : UIWindowView
{
    [SerializeField] CharacterCardView characterCardView;
    [SerializeField] CharacterCardView characterRewardCardView;
    [SerializeField] Transform characterCardContainer;
    [SerializeField] Transform characterCardStart;

    [SerializeField] BaseDragableItem itemCardView;
    [SerializeField] BaseDragableItem crystalRewardCardView;
    [SerializeField] private RewardSlot _resourceSlotPrefab;
    [SerializeField] private Transform _rewardsContainer;
    [SerializeField] Transform itemCardContainer;
    [SerializeField] UIBattleWindow combatWindow;

    [SerializeField] Button continueButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button changeHeroesButton;
    [SerializeField] Button toMenu;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI expText;
    [SerializeField] TextMeshProUGUI gemsText;
    [SerializeField] TextMeshProUGUI enrgyText;
    [SerializeField] TextMeshProUGUI keysText;
    [SerializeField] GameObject goldSlot;
    [SerializeField] GameObject expSlot;
    [SerializeField] GameObject gemsSlot;
    [SerializeField] GameObject energySlot;
    [SerializeField] GameObject keysSlot;
    [SerializeField] TextMeshProUGUI _energyToNextLvl;
    [SerializeField] TextMeshProUGUI _energyToRestartLvl;
    [SerializeField] private TextMeshProUGUI _levelNumberTMP;
    [SerializeField] CombatTimer timer;
    

    [SerializeField] GameObject[] starsGO;

    [SerializeField] private BattleController _battleController;

    private List<Button> _allButtons = new List<Button>();

    private List<BaseDragableItem> _items = new List<BaseDragableItem>();

    [Inject] private SaveManager saveManager;
    [Inject] private ItemsDataInfo itemsDataInfo;
    [Inject] private LevelFinishHandler levelFinishHandler;
    [Inject] private PvPBattleData _pvpBattleData;
    [Inject] private PvPPlayerService _pvpPlayerService;

    private IRewardHandler rewardHandler;
    private LevelRewardHandler levelRewardHandler;
    private int _moneyRewardCount;
    public static bool IsVictoryWindowShown = false;

    private void OnEnable()
    {
        if (temploaryInfo.LevelInfo != null && temploaryInfo.LevelInfo.nextLevel != null && _energyToNextLvl != null)
            _energyToNextLvl.text = temploaryInfo.LevelInfo.nextLevel.EnergyCost.ToString();

        if (_energyToRestartLvl != null)
            _energyToRestartLvl.text = temploaryInfo.LevelInfo.EnergyCost.ToString();
        
        if (temploaryInfo.LevelInfo != null && _levelNumberTMP)
            _levelNumberTMP.text = temploaryInfo.LevelInfo.Name.ToString();
    InitializeCharacters();
    }

    private void Start()
    {
        if (toMenu)
        {
            _allButtons.Add(toMenu);
            toMenu.onClick.AddListener(OnToMenulick);
        }
        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
            toMenu.onClick.AddListener(OnToMenulick);
            continueButton.onClick.AddListener(OnContinueButtonClick);

            _allButtons.Add(restartButton);
            _allButtons.Add(changeHeroesButton);
            _allButtons.Add(continueButton);
        }
        if (changeHeroesButton != null)
        {
            changeHeroesButton.onClick.AddListener(OnChangeHeroesButtonClick);
        }

        ActivateButtons(false);

        CalculateRewardsForDrop();
    }

    private void ActivateButtons(bool value)
    {
        foreach (var button in _allButtons)
        {
            button.interactable = value;
        }
    }

    private void OnContinueButtonClick()
    {
        IsVictoryWindowShown = false;
        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            if (CanUseEnergy(temploaryInfo.LevelInfo.EnergyCost)
                && !string.IsNullOrEmpty(temploaryInfo.LevelInfo.SceneName))
            {
                var nextLevel = temploaryInfo.LevelInfo.nextLevel;
                if (nextLevel != null && _resourceManager.GetActiveSceneName() == nextLevel.SceneName)
                {
                    temploaryInfo.LevelInfo = nextLevel;
                    Hide();
                    
                    foreach (var character in _tempInfo.CreatedCharacters)
                    {
                        if (character.IsDead)
                        {
                            character.ResurectMe();
                        }
                    }

                    _battleController.OnMoveToCombatZone?.Invoke();
                    CalculateRewardsForDrop();
                }
                else if (nextLevel != null)
                {
                    temploaryInfo.LevelInfo = nextLevel;
                    _resourceManager.ShowLoadingScreen();
                    _resourceManager.LoadLevelAsync(temploaryInfo.LevelInfo.SceneName);
                }
                else if (nextLevel == null && temploaryInfo.CurrentStage.NextStage != null)
                {
                    temploaryInfo.CurrentStage = temploaryInfo.CurrentStage.NextStage;
                    temploaryInfo.LevelInfo = temploaryInfo.CurrentStage.Levels[0];
                    _resourceManager.ShowLoadingScreen();
                    _resourceManager.LoadLevelAsync(temploaryInfo.LevelInfo.SceneName);
                }
            }
            else
            {
                ShowNotEnoughtEnergyTooltip();
            }
        }
        else if (temploaryInfo.CurrentMode.GameMode == GameMode.PvP)
        {
            OnToMenulick();
        }
    }

    private void OnRestartButtonClick()
    {
        IsVictoryWindowShown = false;

        if (CanUseEnergy(temploaryInfo.LevelInfo.EnergyCost))
        {
            _resourceManager.ShowLoadingScreen();
            _resourceManager.LoadLevelAsync(_resourceManager.GetActiveSceneName());

        InitializeCharacters();
        }
        else
        {
            ShowNotEnoughtEnergyTooltip();
        }
    }

   private async void OnChangeHeroesButtonClick()
    {
        IsVictoryWindowShown = false;

        temploaryInfo.SelectedCharacters.Clear();
        temploaryInfo.FirstWinfow = WindowsEnum.PreBattleWindow;
        _resourceManager.ShowLoadingScreen();
        
        await _resourceManager.LoadMainMenuSceneAsync();
    }

    private async void OnToMenulick()
    {
        IsVictoryWindowShown = false;

        _resourceManager.ShowLoadingScreen();

        temploaryInfo.FirstWinfow = WindowsEnum.None;
        temploaryInfo.SelectedCharacters.Clear();
        await PlayerInventory.Instance.UpdateInventory(true);

        await _resourceManager.LoadMainMenuSceneAsync();
    }

     public override void Show()
    {
        timer.Deactivate();
        transform.DOKill(true);
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        DestroyOldItems();
        transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
        {
            ActivateButtons(true);
            AnimateBody();

        });
        goldText.gameObject.SetActive(true);
        combatWindow.HideImmediate();
        DisplayEnergy();
        IsVictoryWindowShown = true;
    }

    private void AddRewardFromBoss()
    {
        rewardHandler = new BossRewardHandler(playerData, saveManager, temploaryInfo, RewardHandlerWindow.Win);
        rewardHandler.ProcessRewards(crystalRewardCardView, itemCardView, _rewardsContainer);

        BossData bossData = temploaryInfo.CurrentBoss;       
        var resources = bossData.Rewards[(int)bossData.Difficulty].ResourcesReward.Resources;

        foreach (var resource in resources)
        {
            DisplayResourcesCount(resource);
        }
    }

    private void DisplayResourcesCount(Resource resource)
    {
        if (resource.Type == ResourceType.Gold)
        {
            goldText.text = resource.Count.ToString();
            _moneyRewardCount = resource.Count;
            goldSlot.gameObject.SetActive(true);
        }
        else if (resource.Type == ResourceType.Gems)
        {
            gemsText.text = resource.Count.ToString();
            gemsSlot.gameObject.SetActive(true);
        }
        else if(resource.Type == ResourceType.Energy)
        {
            enrgyText.text = resource.Count.ToString();
            energySlot.gameObject.SetActive(true);
        }
        else if (resource.Type == ResourceType.Keys)
        {
            keysText.text = resource.Count.ToString();
            keysSlot.gameObject.SetActive(true);
        }
    }

    public override void Hide()
    {
        ActivateButtons(false);
        transform.DOKill(true);
        transform.DOScale(Vector3.one * 0.01f, animationDuration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            IsVictoryWindowShown = false;
        });
    }

    private void AnimateBody()
    {
        _moneyRewardCount = 0;
        int experience = 0;
        if (temploaryInfo.CurrentMode.GameMode == GameMode.ThreeToOne)
            AddRewardFromBoss();
        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            experience = temploaryInfo.LevelInfo.XP;
            expText.text = experience.ToString();
            SaveStars();
            rewardHandler.ProcessRewards(crystalRewardCardView, itemCardView, _rewardsContainer.transform);
        }
        else if (temploaryInfo.CurrentMode.GameMode == GameMode.PvP)
        {
            experience = _pvpBattleData.ExperiencePerBattle;
            expText.text = experience.ToString();
            if (levelRewardHandler != null)
            {
                levelRewardHandler.ProcessRewards(crystalRewardCardView, itemCardView, _rewardsContainer.transform);
            }
            else
            {
                rewardHandler = new PvPRewardHandler(playerData, saveManager, temploaryInfo, _pvpPlayerService, _pvpBattleData, RewardHandlerWindow.Win);
                rewardHandler.ProcessRewards(crystalRewardCardView, itemCardView, _rewardsContainer.transform);
            }
        }



        goldText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            expText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBounce);
        });

        AnimateCharacters();

        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            _moneyRewardCount = temploaryInfo.LevelInfo.Gold;
            levelFinishHandler.LevelComplete(temploaryInfo.CurrentStage, temploaryInfo.LevelInfo, GameMode.Default, true, temploaryInfo.SelectedCharacters.Count);
            UpDateTimerText();
        }
        else if (temploaryInfo.CurrentMode.GameMode == GameMode.PvP)
        {
            int rewardMultiplier = _pvpBattleData.CurrentRewardMultiplier(temploaryInfo.FakeLeader);
            for (int i = 0; i < _pvpBattleData.Reward.Resources.Count; i++)
            {
                if (_pvpBattleData.Reward.Contains(ResourceType.Gold, out var resource))
                    _moneyRewardCount = resource.Count * rewardMultiplier;
            }
            _pvpPlayerService.AddExperience(experience, _pvpBattleData, playerData);
            _pvpPlayerService.AddScore(_pvpBattleData.ScorePerBattle);
            levelFinishHandler.LevelComplete(temploaryInfo.CurrentStage, temploaryInfo.LevelInfo, GameMode.PvP, true, temploaryInfo.SelectedCharacters.Count);
        }

        goldText.text = _moneyRewardCount.ToString();
        playerData.PlayerXP += experience;

        if (_moneyRewardCount <= 0)
            goldSlot.SetActive(false);
        if (experience <= 0)
            expSlot.SetActive(false);
            

        saveManager.SaveGame();
    }

    private void SaveStars()
    {
        int stars = 3;
        for (int i = 0; i < starsGO.Length; i++)
        {
            starsGO[i].SetActive(stars > i);
        }

        playerData.AddFinishedLevel(temploaryInfo.CurrentStage, temploaryInfo.LevelInfo, stars, timer.GetFullTime());
    }

    private void AnimateCharacters()
    {
        Extension.DestroyChilds(characterCardContainer);

        var characters = temploaryInfo.SelectedCharacters;

    if (characters == null || characters.Count == 0)
    {
        return;
    }

    int xpToCharacter = characters.Count > 0 ? (temploaryInfo.LevelInfo.XP / characters.Count) : temploaryInfo.LevelInfo.XP;
    if (temploaryInfo.CurrentMode.GameMode == GameMode.PvP && characters.Count > 0)
    {
        xpToCharacter = (_pvpBattleData.ExperiencePerBattle / characters.Count);
    }

        for (int i = 0; i < characters.Count; i++)
        {
        
            var character = characters[i];
            var characterCard = Instantiate(characterCardView, characterCardContainer);
        if (characterCard == null)
        {
            continue;
        }

            characterCard.SetCharacterInfo(character, false, null, null, temploaryInfo, _resourceManager);
            characterCard.AnimateXPBar(xpToCharacter);
            if (character.AddXP(xpToCharacter))
            {
                characterCard.ShowLvlUp();
            }
            if (character.IsDead())
            {
                characterCard.SetDefeated();
            }
        }
    }
private void InitializeCharacters()
{
    Extension.DestroyChilds(characterCardContainer);

    if (temploaryInfo.SelectedCharacters == null)
    {
        temploaryInfo.SelectedCharacters = new List<BaseCharacterModel>();
    }

    AnimateCharacters();
}

    [ContextMenu("TryDropCharacter")]
    public void TryDropCharacter()
    {
        float randomValue = UnityEngine.Random.Range(0, 100);

        for (int i = 0; i < temploaryInfo.CurrentStage.DropCharacters.Length; i++)
        {
            var characterTemplate = temploaryInfo.CurrentStage.DropCharacters[i];

            if (randomValue < characterTemplate.Chance)
            {
                var character = characterTemplate.Character.CreateCharacter();
                var characterCard = Instantiate(characterRewardCardView, _rewardsContainer);
                characterCard.SetCharacterInfo(character, false, null, null, temploaryInfo, _resourceManager);

                playerData.PlayerGroup.AddCharacterToNotAsignedGroup(character);

                return;
            }
        }
    }

    private void CalculateRewardsForDrop()
    {
        levelRewardHandler = new LevelRewardHandler(playerData, saveManager, temploaryInfo, RewardHandlerWindow.Win);
        rewardHandler = levelRewardHandler;

        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            temploaryInfo.CompanyItemRewards = levelRewardHandler.CalculateRewardItems(itemCardView, _rewardsContainer);
        }
    }

 
    private void ShowNotEnoughtEnergyTooltip()
    {
        InfoPopup.Instance.ShowTooltipNotEnpughtEnergy();
        InfoPopup.Instance.ActivateButtons("Go to the store", "Cancel", GoToStore, null);
    }

    private void GoToStore()
    {
        temploaryInfo.SelectedCharacters.Clear();
        temploaryInfo.FirstWinfow = WindowsEnum.ShopWindow;
        _resourceManager.ShowLoadingScreen();
        LoadMainMenuSceneAsyncWrapper();
    }

    private async void LoadMainMenuSceneAsyncWrapper()
    {
        await _resourceManager.LoadMainMenuSceneAsync();
    }
    private void UpDateTimerText()
    {
        var bestTime = playerData.GetBestTime(temploaryInfo.CurrentStage, temploaryInfo.LevelInfo);
        timer.BestTime.text = timer.TranslateToTimeText(bestTime);
    }

    private void DestroyOldItems()
    {
        foreach (var item in _items)
        {
            Destroy(item.gameObject);
        }
        _items.Clear();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DOTween.KillAll(true);
    }
}
