using System;
using System.Collections.Generic;
using System.Linq;

public class LeagueService
{
    private List<FakeLeader> _sortedLeadersData;
    private LeagueProgress[] _leagueProgress;
    private PlayerPvPSavedData _playerData;

    private DateTime _lastResetTime;
    public TimeSpan resetInterval = TimeSpan.FromDays(1);
    
    public LeagueService(List<FakeLeader> sortedLeadersData, LeagueProgress[] leagueProgress, PlayerPvPSavedData playerData)
    {
        _sortedLeadersData = sortedLeadersData;
        _leagueProgress = leagueProgress;
        _playerData = playerData;

        _lastResetTime = DateTime.Now;
    }

    public List<FakeLeader> GetCurrentLeagueLeaders()
    {
        return _sortedLeadersData.OrderByDescending(s => s.CurrentSaveData.Score).ToList();
    }

    public int GetPlayerPlace(PlayerPvPSavedData playerData)
    {
        for (int i = 0; i < _sortedLeadersData.Count; i++)
        {
            if (_sortedLeadersData[i].CurrentSaveData.Score < playerData.Score)
            {
                return i;
            }
        }
        return _sortedLeadersData.Count;
    }

    public LeagueProgress GetLeagueProgress(int playerPlace)
    {
        foreach (LeagueProgress progress in _leagueProgress)
        {
            if (playerPlace <= progress.MaxLeaderPlace)
            {
                return progress;
            }
        }
        return null;
    }
    
    public void ResetPlayerLeagueProgressIfNeeded()
    {
        if (DateTime.Now - _lastResetTime >= resetInterval)
        {
            ResetPlayerProgressToLowestLevel(_playerData);
            _lastResetTime = DateTime.Now;
        }
    }

    private void ResetPlayerProgressToLowestLevel(PlayerPvPSavedData playerData)
    {
        playerData.Score = 0;
        playerData.LeagueLevel = 1;
        playerData.Experience = 0;
    }
}
