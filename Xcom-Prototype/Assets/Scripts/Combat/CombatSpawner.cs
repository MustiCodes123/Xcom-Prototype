using System.Linq;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class CombatSpawner : MonoBehaviour
{
    public TemploaryInfo TemploaryInfo => _temploaryInfo;
    public int CurrentWave { get; private set; }

    [SerializeField] private BattleScene[] _battleScenes;
    [SerializeField] private BattleController _startBattleController;
    [SerializeField] private UICombatCharacterHolder _uICharacterHolder;

    private int _currentLevelId => _temploaryInfo.LevelInfo.Id;

    [Inject] private ResourceManager _resourceManager;
    [Inject] private PlayerData _playerData;
    [Inject] private SaveManager _saveManager;
    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private CharactersRegistry _charactersRegistry;
    [Inject] private BaseCharacerView.Factory _characterFactory;
    [Inject] private ItemView.Factory _itemFactory;
    [Inject] private EnemyFactory _enemyFactory;

    private void Start()
    {
        if (_battleScenes.Length == 0)
        {
            return;
        }
        SpawnPlayerTeam();
        SpawnEnemies();

        _startBattleController.OnMoveToCombatZone?.Invoke();
        _startBattleController.OnMoveToCombatZone += ResetWave;
        _startBattleController.OnMoveToCombatZone += SpawnEnemies;
        _startBattleController.OnMoveToCombatZone += StopBots;       
    }

    public void SpawnEnemies()
    {
        var enemies = _temploaryInfo.LevelInfo.Waves[CurrentWave].Enemie;
        for (int i = 0; i < enemies.Length; i++)
        {
            var enemy = _enemyFactory.Create(enemies[i]);
            var enemyModel = new BaseCharacterModel(_temploaryInfo.LevelInfo.Waves[CurrentWave].Enemie[i].Stats);
            enemyModel.Name = enemies[i].CharacterType.ToString() + i;

            bool isLastenemy = (CurrentWave + 1 >= _temploaryInfo.LevelInfo.Waves.Count() && i + 1 >= enemies.Length);
            enemy.Setup(enemyModel, _battleScenes[_currentLevelId].StartEnemiesPositions[i].position, isLastenemy, _temploaryInfo, _resourceManager, _itemFactory);
            enemy.SetupTeam(Team.Enemies);
        }
        
        CurrentWave++;
    }
    
    private void SpawnPlayerTeam()
    {
        var charactersToCreate = _temploaryInfo.SelectedCharacters;
        
        for (int i = 0; i < charactersToCreate.Count; i++)
        {
            charactersToCreate[i].Health = charactersToCreate[i].GetMaxHP();
            charactersToCreate[i].Mana = charactersToCreate[i].GetMaxMana();
            if (PlayerPrefs.HasKey("ENABLE_INFINITY_MANA"))
                charactersToCreate[i].Mana = int.MaxValue;

            var character = _characterFactory.Create(charactersToCreate[i]);
            _temploaryInfo.AddCreatedCharacter(character);
            character.transform.position = _battleScenes[_currentLevelId].SpawnCharacterPositions[i].position;
            character.SetupTeam(Team.Allies);

            if (PlayerPrefs.HasKey("ENABLE_INFINITY_HEALTH"))
                character.IsImmortal = true;           
        }
    }

    private void StopBots()
    {
        foreach (var character in _charactersRegistry.Characters)
        {
            if (character.CharacterView.IsBot)
                character.CharacterView.SetState(new StartBattleState(character.CharacterView, Vector3.zero, _startBattleController));
        }
    }

    private void ResetWave() => CurrentWave = 0;
}
