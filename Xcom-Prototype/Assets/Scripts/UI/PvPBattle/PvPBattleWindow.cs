using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PvPBattleWindow : UIWindowView
{
    private const int UpgradeLeadersCount = 5;
    private const int BattleContentCount = 10;
    private const string Separator = " / ";

    [SerializeField] private Sprite[] _shieldsSprites;
    [SerializeField] private Sprite[] _levelSprites;
    [SerializeField] private Image _shieldImage;
    [SerializeField] private Image _nextShieldImage;
    [SerializeField] private Image _levelImage;
    [SerializeField] private Image _nextLevelImage;
    [SerializeField] private Image _BG;
    [SerializeField] private LeaderScoreContent[] _leaderScoreContents;
    [SerializeField] private LeaderScoreContent _playerScoreContent;
    [SerializeField] private Transform _battleContentParent;
    [SerializeField] private PvPBattleContent _battleContentPrefab;
    [SerializeField] private TMP_Text _moneyCountText;
    [SerializeField] private TMP_Text _energyText;
    [SerializeField] private TMP_Text _experienceText;
    [SerializeField] private Slider _experienceSlider;
    [SerializeField] private Button _updateListButton;
    [SerializeField] private PvPRewardPopUp _rewardPopUp;

    [Inject] private PvPPlayerService _playerPvPData;
    [Inject] private PvPBattleData _battleData;
    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private UIWindowManager _uIWindowManager;

    private DateTime _lastResetTime;
    private TimeSpan _resetInterval;
    private List<FakeLeader> _sortedLeadersData = new();
    private List<Button> _rewardButtons = new();
    private LeagueService _leagueService;
    private PlayerPvPSavedData _playerSavedData => _playerPvPData.Data;
    private int _previousLeagueLevel;

    protected void OnEnable()
    {
        CheckLeagueLevelChange();
        Debug.Log(_previousLeagueLevel + "/" + _playerSavedData.LeagueLevel);
    }

    private void OnDisable()
    {
        _updateListButton.onClick.RemoveListener(CreateBattleContent);
    }

    private void OnDestroy()
    {
        _rewardButtons.ForEach(button => button.onClick.RemoveListener(() => _rewardPopUp.SetActive(true)));
    }

    private void CheckLeagueLevelChange()
    {
        if (_previousLeagueLevel != _playerSavedData.LeagueLevel)
        {
            ShowRewardPopUp();
            _previousLeagueLevel = _playerSavedData.LeagueLevel;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeUI();
        InitializeLeague();
        InitializeExperiencePanel();
    }

    private void Start()
    {
        CreateBattleContent();

        _updateListButton.onClick.AddListener(CreateBattleContent);
        SetResetInterval(TimeSpan.FromDays(1));
    }

    private void Update()
    {
        if (DateTime.Now - _lastResetTime >= _resetInterval)
        {
            _leagueService.ResetPlayerLeagueProgressIfNeeded();
            _lastResetTime = DateTime.Now;
            InitializeExperiencePanel();
        }
    }

    public void SetResetInterval(TimeSpan interval)
    {
        _resetInterval = interval;
        _lastResetTime = DateTime.Now;
    }

    public void ShowRewardPopUp()
    {
        _rewardPopUp.SetActive(true);
    }

    private void InitializeUI()
    {
        UpgradeLeaders();
        _sortedLeadersData.AddRange(_battleData.FakeLeaders.OrderByDescending(s => s.CurrentSaveData.Score));
    }

    private void InitializeExperiencePanel()
    {
        _experienceText.text = _playerSavedData.Experience + Separator + _playerPvPData.NextLevelExp;
        _experienceSlider.value = ((float)_playerSavedData.Experience / _playerPvPData.NextLevelExp);

        int level = _playerSavedData.LeagueLevel;
        int nextLevel = level + 1;

        _shieldImage.sprite = _shieldsSprites[level];
        _levelImage.sprite = _levelSprites[level];

        _nextShieldImage.sprite = _shieldsSprites[Mathf.Min(level + 1, _shieldsSprites.Length - 1)];
        _nextLevelImage.sprite = _levelSprites[Mathf.Min(nextLevel, _levelSprites.Length - 1)];
    }

    private void InitializeLeague()
    {
        _leagueService = new LeagueService(_sortedLeadersData, _battleData.LeagueProgress, _playerPvPData.Data);
        int playerPlace = _leagueService.GetPlayerPlace(_playerSavedData);
        CreateCurrentLeagueBoard(playerPlace);
    }

    private void UpgradeLeaders()
    {
        for (int i = 0; i < UpgradeLeadersCount; i++)
        {
            int random = Random.Range(0, _battleData.FakeLeaders.Count);
            _battleData.FakeLeaders[random].AddScore(_battleData.ScorePerBattle * 2);
            _battleData.FakeLeaders[random].AddLevel();
        }
    }

    private void StartBattle(FakeLeader fakeLeader)
    {
        _temploaryInfo.FakeLeader = fakeLeader;
        _uIWindowManager.ShowWindow(WindowsEnum.PreBattleWindow);
        HideImmediate();
    }

    private void CreateBattleContent()
    {
        ClearBattleContent();

        int currentLeagueLevel = _playerSavedData.LeagueLevel;
        TeamLevelBounds teamLevelBounds = _battleData.GetTeamLevelBoundsForLeagueLevel(currentLeagueLevel);
        if (teamLevelBounds != null)
        {
            int minTeamLvlIndex = teamLevelBounds.MinTeamLvlIndex;
            int maxTeamLvlIndex = teamLevelBounds.MaxTeamLvlIndex;


            var filteredLeaders = _sortedLeadersData
                .Where(leader => leader.TeamLvlIndex >= minTeamLvlIndex && leader.TeamLvlIndex <= maxTeamLvlIndex)
                .OrderBy(leader => leader.CurrentSaveData.Level)
                .ToList();

            var shuffledLeaders = filteredLeaders.OrderBy(x => UnityEngine.Random.value).ToList();

            for (int i = 0; i < BattleContentCount && i < shuffledLeaders.Count; i++)
            {
                var battleContent = Instantiate(_battleContentPrefab, _battleContentParent);
                battleContent.Initialize(shuffledLeaders[i], StartBattle, _battleData);

                _rewardButtons.Add(battleContent.ShowRewardsButton);              
            }

            _rewardButtons.ForEach(button => button.onClick.AddListener(() => _rewardPopUp.SetActive(true)));
        }
        else
        {
            Debug.LogError("Team level bounds not found for league level " + currentLeagueLevel);
        }
    }


    private List<int> GenerateUniqueRandomIndexes(int maxIndex)
    {
        List<int> indexes = new List<int>();
        while (indexes.Count < BattleContentCount)
        {
            int index = Random.Range(0, maxIndex);
            if (!indexes.Contains(index))
            {
                indexes.Add(index);
            }
        }

        return indexes;
    }

    private void CreateCurrentLeagueBoard(int playerPlace)
    {
        var currentLeagueLeaders = _leagueService.GetCurrentLeagueLeaders();

        _playerScoreContent.Initialize(_playerSavedData.Name, _playerSavedData.Score.ToString(), "");

        for (int i = 0; i < _leaderScoreContents.Length; i++)
        {
            string placeText = (i + 1).ToString();
            if (i == playerPlace)
            {
                _leaderScoreContents[i].Initialize(_playerSavedData.Name, _playerSavedData.Score.ToString(), placeText);
                _playerScoreContent.Initialize(_playerSavedData.Name, _playerSavedData.Score.ToString(), placeText);
            }
            else if (i > playerPlace)
            {
                _leaderScoreContents[i].Initialize(currentLeagueLeaders[i - 1].Name,
                    currentLeagueLeaders[i - 1].CurrentSaveData.Score.ToString(), placeText);
            }
            else
            {
                _leaderScoreContents[i].Initialize(currentLeagueLeaders[i].Name,
                    currentLeagueLeaders[i].CurrentSaveData.Score.ToString(), placeText);
            }
        }
    }

    private void ClearBattleContent()
    {
        foreach (Transform child in _battleContentParent)
        {
            Destroy(child.gameObject);
        }
    }

    public override void Show()
    {
        base.Show();
        _BG.gameObject.SetActive(true);
    }

    public override void Hide()
    {
        base.Hide();
        _BG.gameObject.SetActive(false);
        // gameObject.SetActive(false);
    }
}