using Data.Resources.AddressableManagement;
using Zenject;

public class EnemyCharacterFactory : IFactory<CampLevelEnemies, IEnemy>
{
    private readonly DiContainer _container;
    private readonly ResourceManager _resourceManager;

    public EnemyCharacterFactory(DiContainer container, ResourceManager resourceManager)
    {
        _container = container;
        _resourceManager = resourceManager;
    }

    public IEnemy Create(CampLevelEnemies enemies)
    {
        string prefabName = "";
        switch (enemies.EnemyRace)
        {
            case CharacterRace.Dummy:
                break;
            case CharacterRace.Human:
                break;
            case CharacterRace.Orc:
                break;
            case CharacterRace.Elf:
                break;
            case CharacterRace.Dwarf:
                break;
            case CharacterRace.Undead:
                break;
            case CharacterRace.Goblin:
                prefabName = "GoblinWarrior" + enemies.CharacterType.ToString();
                break;
            case CharacterRace.Troll:
                break;
            case CharacterRace.Ogre:
                break;
            case CharacterRace.Dragon:
                break;
            case CharacterRace.Demon:
                prefabName = enemies.EnemyRace.ToString() + enemies.CharacterType.ToString();
                break;
            case CharacterRace.Beast:
                break;
            case CharacterRace.Elemental:
                break;
            case CharacterRace.Skeleton:
                prefabName = enemies.EnemyRace.ToString() + enemies.CharacterType.ToString();
                break;
            default:
                prefabName = enemies.EnemyRace.ToString() + enemies.CharacterType.ToString();
                break;
        }

        BaseCharacerView prefab = _resourceManager.LoadEnemyBaseCharacterViewPrefab(prefabName);
        var enemie = _container.InstantiatePrefabForComponent<IEnemy>(prefab);
        return enemie;
    }
}

public class EnemyFactory : PlaceholderFactory<CampLevelEnemies, IEnemy>
{
}