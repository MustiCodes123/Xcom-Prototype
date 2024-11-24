using UnityEngine;

public class EnemyIconFactory
{
    private EnemiesIconsData _enemiesIconsData;

    public EnemyIconFactory(EnemiesIconsData enemiesIconsData) 
    {
        _enemiesIconsData = enemiesIconsData;
    }

    public Sprite CreateEnemyIcon(CharacterRace race)
    {
        return _enemiesIconsData.GetIcon(race);
    }
}