using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using System.Collections.Generic;

public class UIGameLoseWindow : UIWindowView
{
    [SerializeField] private TMP_Text _experienceText;

    [SerializeField] private int looseXPDecrease = 10;
    [SerializeField] CharacterCardView characterCardView;
    [SerializeField] Transform characterCardContainer;
    [SerializeField] UIBattleWindow combatWindow;

    [SerializeField] BaseDragableItem itemCardView;
    [SerializeField] private RewardSlot _resourceSlotPrefab;
    [SerializeField] private Transform _rewardsContainer;
    
    [SerializeField] Button restartButton;
    [SerializeField] Button changeHeroesButton;
    [SerializeField] Button toMenu;
    [SerializeField] TextMeshProUGUI _energyToRestartLvl;
    [SerializeField] private TextMeshProUGUI _levelNumberTMP;
    
    [SerializeField] CombatTimer timer;
    [SerializeField] GameObject[] starsGO;

    private List<BaseDragableItem> _items = new List<BaseDragableItem>();

    [Inject] private SaveManager saveManager;
    [Inject] private PvPBattleData _pvpBattleData;
    [Inject] private ItemsDataInfo itemsDataInfo;
    [Inject] private LevelFinishHandler _levelFinishHandler;

    private IRewardHandler rewardHandler;

    private void OnEnable()
    {
        if(_energyToRestartLvl != null)
            _energyToRestartLvl.text = temploaryInfo.LevelInfo.EnergyCost.ToString();
    }

    private void Start()
    {
        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
            changeHeroesButton.onClick.AddListener(OnChangeHeroesButtonClick);
        }
        
        toMenu.onClick.AddListener(OnToMenuClick);
    }

    private void OnRestartButtonClick()
    {
        if (CanUseEnergy(temploaryInfo.LevelInfo.EnergyCost))
        {
            _resourceManager.ShowLoadingScreen();
            _resourceManager.LoadLevelAsync(_resourceManager.GetActiveSceneName());
        }
        else
        {
            InfoPopup.Instance.ShowTooltipNotEnpughtEnergy();
            InfoPopup.Instance.ActivateButtons("Go to the store", "Cancel", GoToStore, null);
        }
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

    private async void OnChangeHeroesButtonClick()
    {
        temploaryInfo.SelectedCharacters.Clear();
        temploaryInfo.FirstWinfow = WindowsEnum.PreBattleWindow;
        _resourceManager.ShowLoadingScreen();
        await _resourceManager.LoadMainMenuSceneAsync();
    }

    private async void OnToMenuClick()
    {
        temploaryInfo.FirstWinfow = WindowsEnum.None;
        temploaryInfo.SelectedCharacters.Clear();
        _resourceManager.ShowLoadingScreen();
        await _resourceManager.LoadMainMenuSceneAsync();
    }

    public override void Show()
    {
        if (_levelNumberTMP != null && temploaryInfo.LevelInfo != null)
        {
            _levelNumberTMP.text = temploaryInfo.LevelInfo.Name;
        }
        
        timer.Deactivate();
        transform.DOKill(true);
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(1, animationDuration);
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(AnimateBody);
        combatWindow.HideImmediate();
        DisplayEnergy();
    }

    private void AddRewardFromBoss()
    {
        var boss = temploaryInfo.CurrentBoss;
        
        rewardHandler = new BossRewardHandler(playerData, saveManager, temploaryInfo, RewardHandlerWindow.Lose);
        rewardHandler.ProcessRewards(itemCardView, itemCardView, _rewardsContainer);

        _experienceText.text = "Exp: " + boss.ExperiencePerBattle[(int)boss.Difficulty];
        playerData.PlayerXP += boss.ExperiencePerBattle[(int)boss.Difficulty];

        saveManager.SaveGame();
    }

    private void AnimateBody()
    {
        AnimateCharacters();

        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            playerData.PlayerXP += temploaryInfo.LevelInfo.XP / looseXPDecrease;            
        }

        saveManager.SaveGame();

        if (temploaryInfo.CurrentMode.GameMode == GameMode.TestOfStrenght)
        {
            AddRewardFromBoss();
        }

        _levelFinishHandler.LevelComplete(temploaryInfo.CurrentStage, temploaryInfo.LevelInfo, temploaryInfo.CurrentMode.GameMode, false, temploaryInfo.SelectedCharacters.Count);
    }

    private void AnimateCharacters()
    {
        var characters = temploaryInfo.SelectedCharacters;

        int xpToCharacter = 0;

        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
            xpToCharacter = (temploaryInfo.LevelInfo.XP / characters.Count) / 2;
        else if (temploaryInfo.CurrentMode.GameMode == GameMode.PvP)
            xpToCharacter = _pvpBattleData.ExperiencePerBattle / characters.Count / 2;

        _experienceText.text = "Exp: " + xpToCharacter.ToString();

        for (int i = 0; i < characters.Count; i++)
        {
            var character = characters[i];
            var characterCard = Instantiate(characterCardView, characterCardContainer);
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DOTween.KillAll(true);
    }
}