using System;
using UnityEngine;

[Serializable]
public class EnemyCharacter
{
    public CharacterPreset CharacterPreset => _characterPreset;
    public EnemyStats EnemyStats => _enemyStats;

    [SerializeField] private CharacterPreset _characterPreset;
    [SerializeField] private EnemyStats _enemyStats;

    public void SetBaseEnemyStats()
    {
        _enemyStats.Level = 1;
        _enemyStats.Strength = 3;
        _enemyStats.Agility = 3;
        _enemyStats.Intelligence = 3;
        _enemyStats.HP = 1000;
        _enemyStats.MP = 100;
        _enemyStats.Speed = 3.5f;
    }
}
