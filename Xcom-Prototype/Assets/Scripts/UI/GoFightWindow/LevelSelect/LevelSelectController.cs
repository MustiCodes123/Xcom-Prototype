using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class LevelSelectController : MonoBehaviour
{
    [Inject] private PlayerData _playerData;
    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private UIWindowManager _windowManager;
    [Inject] private EnemiesIconsData _enemiesIconsData;
    [Inject] private FightWindowDataProvider _dataProvider;

    [SerializeField] private RewardPanel _rewardPanel;
    [SerializeField] private Transform _levelButtonsRoot;

    private EnemyIconFactory _enemyIconFactory;
    private FightWindowPool _fightWindowPool;

    private List<LevelButton> _activeButtons = new List<LevelButton>();

    public Stage CurrentStage { private get; set; }

    [Inject]
    public void Constructor(FightWindowPool pool)
    {
        _fightWindowPool = pool;
    }

    #region Initialization
    public void Initialize(Stage currentStage)
    {
        _enemyIconFactory = new EnemyIconFactory(_enemiesIconsData);

        if (CurrentStage == null)
        {
            CurrentStage = currentStage;
        }

        CreateOrUpdateButtons();

        InitializeLevelButtons();
    }

    private void CreateOrUpdateButtons()
    {
        _activeButtons.Clear();

        for (int i = 0; i < CurrentStage.Levels.Length; i++)
        {
            LevelButton button = _fightWindowPool.SpawnLevelButton(_levelButtonsRoot);
            button.ButtonOrder = i;

            _activeButtons.Add(button);
        }

        foreach (var button in _activeButtons.OrderBy(b => b.ButtonOrder))
        {
            button.transform.SetSiblingIndex(button.ButtonOrder);
        }
    }

    private void InitializeLevelButtons()
    {
        Dictionary<string, List<int>> progress = _playerData.GetCompanyProgres();
        List<int> levelStars = progress.ContainsKey(CurrentStage.Name) ? progress[CurrentStage.Name] : new List<int>();

        for (int i = 0; i < _activeButtons.Count; i++)
        {
            _activeButtons[i].Initialize(CurrentStage.Levels[i]);
            _activeButtons[i].SetupButtonState(levelStars, i);
            _activeButtons[i].Click += HandleLevelButtonClick;
            _activeButtons[i].StartBattleButton.Click += OnStartBattleClick;

            if (i < levelStars.Count)
            {
                _activeButtons[i].InitializeStars(levelStars[i]);
            }
        }

        foreach (LevelButton button in _activeButtons)
        {
            if (button.transform.GetSiblingIndex() == 0)
            {
                button.ButtonView.SetState(new SelectedState());
                _rewardPanel.DisplayRewards(button.RewardData);
            }
        }

        SetupActiveButtons();
    }
    #endregion

    #region Events
    private void OnStartBattleClick(CampLevel levelData)
    {
        _temploaryInfo.LevelInfo = levelData;

        _dataProvider.Level = levelData.Id + 1;

        _windowManager.ShowWindow(WindowsEnum.PreBattleWindow);

    }

    public void OnDisableScreen()
    {
        foreach (LevelButton levelButton in _activeButtons)
        {
            levelButton.StartBattleButton.Click -= OnStartBattleClick;

            _fightWindowPool.RemoveLevelButton(levelButton);
        }

        _activeButtons.Clear();
    }

    private void HandleLevelButtonClick(LevelButton clickedButton)
    {
        if (clickedButton.IsUnlocked)
        {
            foreach (LevelButton button in _activeButtons)
            {
                if (button == clickedButton)
                    button.ButtonView.SetState(new SelectedState());

                else if (button.IsUnlocked)
                    button.ButtonView.SetState(new UnselectedState());
            }

            _rewardPanel.DisplayRewards(clickedButton.RewardData);
        }
    }
    #endregion

    #region Utility Methods
    private void SetupActiveButtons()
    {
        SetupWarriorsIcons();
    }

    private void SetupWarriorsIcons()
    {
        _dataProvider.LevelEnemiesIcons = new List<Sprite>();

        HashSet<Sprite> uniqueIcons = new HashSet<Sprite>();

        foreach (var button in _activeButtons)
        {
            CampLevel level = button.LevelData;

            HashSet<CharacterRace> uniqueEnemyTypes = new HashSet<CharacterRace>();

            foreach (var wave in level.Waves)
            {
                foreach (var enemy in wave.Enemie)
                {
                    uniqueEnemyTypes.Add(enemy.EnemyRace);
                }
            }

            List<Sprite> icons = new List<Sprite>();
            foreach (var enemyType in uniqueEnemyTypes)
            {
                Sprite icon = _enemyIconFactory.CreateEnemyIcon(enemyType);
                icons.Add(icon);

                if (uniqueIcons.Add(icon))
                {
                    _dataProvider.LevelEnemiesIcons.Add(icon);
                }
            }

            button.ButtonView.DisplayEnemyIcons(icons.ToArray());
        }
    }

    public void SetupEnemiesIconsForCompany()
    {
        _enemyIconFactory = new EnemyIconFactory(_enemiesIconsData);

        _dataProvider.LevelEnemiesIcons = new List<Sprite>();

        HashSet<Sprite> uniqueIcons = new HashSet<Sprite>();

        CampLevel level = _temploaryInfo.LevelInfo;

        HashSet<CharacterRace> uniqueEnemyTypes = new HashSet<CharacterRace>();

        foreach (var wave in level.Waves)
        {
            foreach (var enemy in wave.Enemie)
            {
                uniqueEnemyTypes.Add(enemy.EnemyRace);
            }
        }

        List<Sprite> icons = new List<Sprite>();
        foreach (var enemyType in uniqueEnemyTypes)
        {
            Sprite icon = _enemyIconFactory.CreateEnemyIcon(enemyType);
            icons.Add(icon);

            if (uniqueIcons.Add(icon))
            {
                _dataProvider.LevelEnemiesIcons.Add(icon);
            }
        }
    }
    public void SetupEnemiesIconsForPVP()
    {
        _dataProvider.LevelEnemiesIcons = new List<Sprite>();

        foreach (var enemy in _temploaryInfo.FakeLeader.Characters)
        {
            Sprite icon = enemy.CharacterPreset.CharacterSprite;
            _dataProvider.LevelEnemiesIcons.Add(icon);
        }
    }
#endregion
}
    