using ModestTree;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

public class TestOfStrenghtCombatSpawner : MonoBehaviour
{
    [SerializeField] private BattleScene _battleScene;
    [SerializeField] private UICombatCharacterHolder _uICharacterHolder;

    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private BaseCharacerView.Factory _characterFactory;
    [Inject] private CharactersRegistry _charactersRegistry;
    [Inject] private ThreeToOneContainer _container;
    [Inject] private IShapeCollection _shapeCollection;

    private int _currentWaveIndex;

    private void Start()
    {
        if (_temploaryInfo.CurrentMode.GameMode != GameMode.TestOfStrenght) return;
        _uICharacterHolder.InitializeGroupProgressIcons(_temploaryInfo.SelectedCharacterGroups);
        SpawnAllyTeam();
        _uICharacterHolder.CreateCharactersCards();
        SpawnBoss();
        foreach (var character in _charactersRegistry.Characters)
        {
            character.CharacterView.SetAttackState();
        }
    }

    private void SpawnAllyTeam()
    {
        var charactersToCreate = _temploaryInfo.SelectedCharacterGroups;

        for (int i = 0; i < charactersToCreate[_currentWaveIndex].Count; i++)
        {
            var model = charactersToCreate[_currentWaveIndex][i];
            model.Health = model.GetMaxHP();
            model.Mana = model.GetMaxMana();

            var character = _characterFactory.Create(model);
            _temploaryInfo.AddCreatedCharacter(character);

            character.transform.position = _battleScene.SpawnCharacterPositions[i].position;
            character.SetupTeam(Team.Allies);

            character.OnDie += ChangeSliderValue;
            character.OnDie += TrySpawnNextWave;
        }

        foreach (var character in _charactersRegistry.Characters)
        {
            character.CharacterView.SetAttackState();
        }
        _uICharacterHolder.RefreshGroupProgress(_currentWaveIndex);
        _currentWaveIndex++;
    }

    private void ChangeSliderValue()
    {
        _uICharacterHolder.UpdateGroupSlider(_currentWaveIndex - 1);
    }

    private void TrySpawnNextWave()
    {
        if (_currentWaveIndex < _temploaryInfo.SelectedCharacterGroups.Keys.Count)
        {
            for (int i = 0; i < _charactersRegistry.Characters.Count; i++)
            {
                if (_charactersRegistry.Characters[i].Team == Team.Allies && _charactersRegistry.Characters[i].IsDead == false)
                {
                    return;
                }
            }
            SpawnAllyTeam();
        }
    }

    private void SpawnBoss()
    {
        _temploaryInfo.TestOfStrenghtReward.Clear();
        _temploaryInfo.DamageDealt = 0;
        var boss = _temploaryInfo.CurrentBoss;

        var bossModel = boss.BossPreset.CreateBoss(boss, (int)boss.Difficulty);
        bossModel.Health = bossModel.GetMaxHP();
        bossModel.Mana = bossModel.GetMaxMana();

        var bossView = _characterFactory.Create(bossModel);
        bossView.IsBot = true;
        bossView.IsBoss = true;
        bossView.IsImmortal = true;
        bossView.transform.position = _battleScene.StartEnemiesPositions[0].position;
        bossView.SetupTeam(Team.Enemies);
        bossView.OnDie += _uICharacterHolder.SetNextSliderColors;
        bossView.OnDie += UpgradeBoss;
        bossView.OnDie += AddReward;
        bossView.OnTakeDamage += CalculateTotalDamage;

        _temploaryInfo.EnemiesCharacters.Clear();
        _temploaryInfo.EnemiesCharacters.Add(bossView);

        _uICharacterHolder.CreateBossSlider();
    }

    private void AddReward()
    {
        _temploaryInfo.TestOfStrenghtReward.Add(_temploaryInfo.CurrentBoss.Rewards[(int)_temploaryInfo.CurrentBoss.Difficulty]);
    }

    private void UpgradeBoss()
    {
        for (int i = 0; i < _charactersRegistry.Characters.Count; i++)
        {
            if (_charactersRegistry.Characters[i].Team == Team.Enemies &&
                _charactersRegistry.Characters[i].CharacterView.characterData is BossModel bossModel)
            {
                bossModel.BossStatsIncrease();
                break;
            }
        }
    }

    private void CalculateTotalDamage(int value)
    {
        _temploaryInfo.DamageDealt += value;
    }
}
