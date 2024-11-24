using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class FakeLeader
{
    public LeaderSaveData CurrentSaveData {  get; private set; }
    public LeaderSaveData LeaderData => _leaderData;
    public string Name => _name;
    public int TeamLvlIndex;
    public Sprite Icon => _icon;
    public EnemyCharacter[] Characters => _characters;

    [SerializeField] private LeaderSaveData _leaderData;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private EnemyCharacter[] _characters = new EnemyCharacter[4];

    public void SetCurrentData(LeaderSaveData leaderData)
    {
        CurrentSaveData = leaderData;
        //TeamLvlIndex = CalculateTeamLvlIndex();
    }

    public void AddScore(int count)
    {
        CurrentSaveData.Score += count;
    }

    public void AddLevel()
    {
        if (CurrentSaveData.Level == GameConstants.MaxPvPLevel) return;
        CurrentSaveData.Level++;
    }

    public int CalculateTeamLvlIndex()
    {
        List<EnemyCharacter> charactersList = new List<EnemyCharacter>();
        charactersList.AddRange(_characters);

        List<int> LvlList = new List<int>();
        List<int> highestList = new List<int>();

        int counter = 0;
        int result = 0;
        foreach (EnemyCharacter character in charactersList)
        {
            LvlList.Add(character.EnemyStats.Level);
        }

        LvlList.Sort();

        while (counter < GameConstants.ThreeToOneCharacterLimit)
        {
            counter++;
            if (LvlList.Count >= GameConstants.ThreeToOneCharacterLimit)
            {
                result += LvlList[LvlList.Count - counter];
            }
            else
            {
                result = LvlList.Sum();
            }
        }

        return result;
    }
}