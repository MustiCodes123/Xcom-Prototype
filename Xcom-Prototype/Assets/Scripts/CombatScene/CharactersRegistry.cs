using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class CharactersRegistry 
{
    public List<IDamageable> Characters => _characters;
    public BaseCharacerView.Factory GetCharacterFactory => _characterFactory;

    private List<IDamageable> _characters = new List<IDamageable>();

    private readonly float maxDistance = 1000;
    private readonly int openDebrifingDelay = 10;

    private UIWindowManager _windowManager;
    private UIBattleWindow _battleWindow;
    private CombatSpawner _spawner;
    private BaseCharacerView.Factory _characterFactory;

    [Inject]
    public CharactersRegistry(UIWindowManager windowManager,
        BaseCharacerView.Factory characterFactory,
        CombatSpawner combatSpawner,
        UIBattleWindow battleWindow)
    {
        _windowManager = windowManager;
        _characterFactory = characterFactory;
        _spawner = combatSpawner;
        _battleWindow = battleWindow;
    }   

    public IDamageable GetClosestEnemy(Team team, Vector3 selfPos)
    {
        float closestEnemy = maxDistance;
        IDamageable closestEnemyView = null;

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].Team != team && !_characters[i].IsDead)
            {
                float distance = Vector3.Distance(selfPos, _characters[i].Position);
                if (distance < closestEnemy)
                {
                    closestEnemy = distance;
                    closestEnemyView = _characters[i];
                }
            }
        }
        return closestEnemyView;
    }

    public List<IDamageable> GetAllTargetsFromAnotherTeam(Team team)
    {
        List<IDamageable> targets = new List<IDamageable>();

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].Team != team)
            {
                targets.Add(_characters[i]);
            }    

        }

        return targets;

    }

    public BaseCharacerView GetRandomCharacter(Team team, IDamageable ignore = null)
    {
        List<BaseCharacerView> teamCharacters = new List<BaseCharacerView>();

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].Team == team && !_characters[i].IsDead)
            {
                if(ignore != null)
                {
                    if(ignore == _characters[i])
                    {
                        continue;
                    }    
                }
                teamCharacters.Add(_characters[i] as BaseCharacerView);
            }
        }

        if (teamCharacters.Count > 0)
        {
            int randomIndex = Random.Range(0, teamCharacters.Count);
            return teamCharacters[randomIndex];
        }
        else
        {
            return null;
        }
    }

    public BaseCharacerView GetMinHPFriend(Team team)
    {
        float minHP = float.MaxValue;
        BaseCharacerView minHPFriend = null;

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].Team == team && !_characters[i].IsDead)
            {
                var baseCharacter = _characters[i] as BaseCharacerView;
                if (baseCharacter != null)
                {
                    if (baseCharacter.characterData.Health < minHP)
                    {
                        minHP = baseCharacter.characterData.Health;
                        minHPFriend = baseCharacter;
                    }
                }
            }
        }
        return minHPFriend;

    }

    public void CheckForDeadCharacters()
    {
        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].IsDead)
            {
                _characters.RemoveAt(i);
            }
        }

        List<IDamageable> allies = new List<IDamageable>();
        List<IDamageable> enemies = new List<IDamageable>();

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].Team == Team.Allies)
            {
                allies.Add(_characters[i]);
            }
            else
            {
                enemies.Add(_characters[i]);
            }
        }

        _battleWindow.UpdateWaveBar(enemies.Count);

        if (allies.Count == 0)
        {
            
            _windowManager.ShowWindow(WindowsEnum.BattleLoseWindow);
        }
        else if (enemies.Count == 0)
        {
            if(_spawner != null && _spawner.TemploaryInfo.CurrentMode.GameMode == GameMode.Default 
                && _spawner.CurrentWave < _spawner.TemploaryInfo.LevelInfo.Waves.Length)
            {
                _spawner.SpawnEnemies();
            }
            else
            {
                _windowManager.ShowWindowWithDelay(WindowsEnum.BattleWinWindow, openDebrifingDelay);
            }
        }
 
    }
    
    public void AddCharacter(IDamageable viewer)
    {
        _characters.Add(viewer);
    }

    public void RemoveCharacter(IDamageable viewer)
    {
        _characters.Remove(viewer);
    }
}
