using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;

public class ThreeToOneWindow : BossWindowView
{
    [SerializeField] private TeamMemberContent[] _teamMembers;
    [SerializeField] private TeamMemberContent _playerContent;

    [Inject] private PvPBattleData _pvpBattleData;

    private readonly Dictionary<Difficulty, int> _difficultToTeamLvlIndex = new Dictionary<Difficulty, int>()
    {
        { Difficulty.Easy, 35 },
        { Difficulty.Rare, 55 },
        { Difficulty.Epic, 75 },
        { Difficulty.Legendary, 95 },
        { Difficulty.Mythical, 115 }
    };

    public override void Show()
    {
        base.Show();

        InitializeTeamMembers(BossContent.BossData);
    }

    public override void Hide()
    {
        base.Hide();
        // gameObject.SetActive(false);
    }


    public void InitializeTeamMembers(BossData data)
    {
        List<FakeLeader> leadersList = new List<FakeLeader>(_pvpBattleData.FakeLeaders);

        FakeLeader leader = GetEqualLeader(leadersList, data.Difficulty);
        leadersList.Remove(leader);
        FakeLeader secondLeader = GetEqualLeader(leadersList, data.Difficulty);
        _teamMembers[0].Init(data, leader, PlayerData.MaxKeysCount);
        _teamMembers[1].Init(data, secondLeader, PlayerData.MaxKeysCount);
        
        _playerContent.InitPlayer(PlayerData);

        TemploaryInfo.FakeTeamMembers.Clear();
        TemploaryInfo.FakeTeamMembers.Add(leader);
        TemploaryInfo.FakeTeamMembers.Add(secondLeader);
    }

    public override void StartBattle()
    {
        UIWindowManager.ShowWindow(WindowsEnum.PreBattleWindow);

        HideImmediate();
    }

    private FakeLeader GetEqualLeader(List<FakeLeader> fakeLeadersList, Difficulty difficulty)
    {
        int number = _difficultToTeamLvlIndex[difficulty];
        int delta = number;
        FakeLeader result = new FakeLeader();

        foreach (var leader in fakeLeadersList)
        {
            if (Math.Abs(number - leader.TeamLvlIndex) < delta)
            {
                delta = Math.Abs(number - leader.TeamLvlIndex);
                result = leader;
            }
        }

        if (result.Name == null)
        {
            result = fakeLeadersList[0];
        }
        return result;
    }
}


