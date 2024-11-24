using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TemploaryInfo : MonoBehaviour
{
    public CampLevel LevelInfo;
    public Stage CurrentStage;
    public FakeLeader FakeLeader;
    public BossData CurrentBoss;
    public List<FakeLeader> FakeTeamMembers = new List<FakeLeader>();
    public Mode CurrentMode;
    public PlayerContent CurrentPlayerContent;
    public int DamageDealt;
    public Dictionary<int, List<BaseCharacterModel>> SelectedCharacterGroups = new Dictionary<int, List<BaseCharacterModel>>();
    public List<BossReward> TestOfStrenghtReward = new List<BossReward>();
    public List<BaseItem> CompanyItemRewards = new List<BaseItem>();

    public bool Autobattle;
    public bool IsGameSpeedDouble;
    public int AutoBattleCount;
    public int AutoBattleCountMax;
    public int MainMenuSceneIndex { get; private set; } = 1;
    public WindowsEnum FirstWinfow = WindowsEnum.None;
    public CombatScore Score;

    public List<BaseCharacterModel> SelectedCharacters = new List<BaseCharacterModel>();

    public List<BaseCharacerView> CreatedCharacters = new List<BaseCharacerView>();
    public List<BaseCharacerView> EnemiesCharacters = new List<BaseCharacerView>();

    [Inject] private SaveManager _saveManager;

    private void Awake()
    {
        StartCoroutine(MinuteCorutine());
        Score = new CombatScore(this);
    }

    public void ChangeAutoBAttle()
    {
        Autobattle = !Autobattle;
        UpdateAutoBattle(Autobattle);
    }
    public void OnToggleValueChanged(bool isOn)
    {
        IsGameSpeedDouble = isOn;
    }

    public void UpdateAutoBattle(bool isOn)
    {
        foreach (var character in CreatedCharacters)
        {
            character.ChangeAutoAtack(isOn);
        }
    }

    public void UpdateForNextLevel(CampLevel nextLevel)
    {
        foreach (var character in CreatedCharacters)
        {
            character.RestoreAllComponents();
        }
    }
    
    public void BattleEnd()
    {
        CreatedCharacters.Clear();        
    }

    public void AddCreatedCharacter(BaseCharacerView view)
    {
        CreatedCharacters.Add(view);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            _saveManager.SaveGame();
            _saveManager.CheckEnergy();
        }
    }

    private void OnApplicationQuit()
    {
        _saveManager.SaveGame();
    }
    private IEnumerator MinuteCorutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            _saveManager.CheckEnergy();
        }
    }
}
