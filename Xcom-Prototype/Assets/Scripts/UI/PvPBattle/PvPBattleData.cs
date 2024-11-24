using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "PvPBattleData", menuName = "Data/PvPBattleData")]
[Serializable]
public class PvPBattleData : ScriptableObject
{
    public int EnergyCost => _energyCost;
    public int ExperiencePerBattle => _experiencePerBattle;
    public int ScorePerBattle => _scorePerBattle;
    public int KeysCost => _keysCost;
    public Reward Reward => _reward;
    public LeagueProgress[] LeagueProgress => _leagueProgress;
    public DropCristal[] Cristals => _cristals;
    public string[] Scenes => _scenes;
    public LevelUpReward[] LevelUpRewards => _levelUpRewards;
    public List<FakeLeader> FakeLeaders => _fakeLeaders;

    [SerializeField] private int _energyCost = 5;
    [SerializeField] private int _experiencePerBattle = 50;
    [SerializeField] private int _scorePerBattle = 10;
    [SerializeField] private int _keysCost = 1;
    [SerializeField] private Reward _reward;
    [SerializeField] private LeagueProgress[] _leagueProgress;
    [SerializeField] private DropCristal[] _cristals;
    [SerializeField] private string[] _scenes;
    [SerializeField] private LevelUpReward[] _levelUpRewards;
    
    [SerializeField] private List<FakeLeader> _fakeLeaders = new();
    [SerializeField] private List<TeamLevelBounds> _teamLevelBounds = new();

    public int CurrentRewardMultiplier(FakeLeader fakeLeader)
    {
        List<FakeLeader> fakeLeaders = new List<FakeLeader>();
        fakeLeaders.AddRange(_fakeLeaders);
        fakeLeaders = fakeLeaders.OrderByDescending(s => s.CurrentSaveData.Score).ToList();
        for (int i = 0; i < fakeLeaders.Count; i++)
        {
            if (fakeLeaders[i] == fakeLeader)
            {
                foreach (var progress in _leagueProgress)
                {
                    if (i < progress.MaxLeaderPlace)
                        return progress.RewardMultiplier;
                }
            }
        }
        return 1;
    }
    public TeamLevelBounds GetTeamLevelBoundsForLeagueLevel(int leagueLevel)
    {
        if (leagueLevel >= 0 && leagueLevel <= _teamLevelBounds.Count)
        {
            return _teamLevelBounds[leagueLevel]; 
        }

        Debug.LogError("Invalid league level!" + leagueLevel);
        return null;
    }
    

    private void ResetPlayerProgressToLowestLevel(PlayerPvPSavedData playerData)
    {
        playerData.Score = 0;
        playerData.LeagueLevel = 1;
        playerData.Experience = 0;
    }
}

[Serializable]
public class LeagueProgress
{
    public int MaxLeaderPlace;
    public int RewardMultiplier;
}

[Serializable]
public class TeamLevelBounds
{
    public int MinTeamLvlIndex;
    public int MaxTeamLvlIndex;

    public TeamLevelBounds(int min, int max)
    {
        MinTeamLvlIndex = min;
        MaxTeamLvlIndex = max;
    }
}
