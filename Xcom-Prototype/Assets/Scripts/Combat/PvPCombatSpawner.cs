using System;
using UnityEngine;
using Zenject;

public class PvPCombatSpawner : MonoBehaviour
{
    [SerializeField] private BattleScene _battleScene;

    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private CharactersRegistry _charactersRegistry;
    [Inject] private BaseCharacerView.Factory _characterFactory;
    [Inject] private PvPBattleData _battleData;
    [Inject] private IShapeCollection _shapeCollection;

    private void Awake()
    {
        SpawnPlayerTeam();
        SpawnEnemies();

        foreach (var character in _charactersRegistry.Characters)
        {
            character.CharacterView.SetAttackState();
        }
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
            character.transform.position = _battleScene.SpawnCharacterPositions[i].position;
            character.SetupTeam(Team.Allies);

            if (PlayerPrefs.HasKey("ENABLE_INFINITY_HEALTH"))
                character.IsImmortal = true;
        }
    }

    private void SpawnEnemies()
    {
        _temploaryInfo.EnemiesCharacters.Clear();
        var characters = _temploaryInfo.FakeLeader.Characters;
        for (var i = 0; i < characters.Length; i++)
        {
            var characterPreset = characters[i].CharacterPreset;
            var characterToCreate = characters[i].CharacterPreset.CreateCharacter(characters[i].EnemyStats, characterPreset);
            characterToCreate.Health = characterToCreate.GetMaxHP();
            characterToCreate.Mana = characterToCreate.GetMaxMana();
            var character = _characterFactory.Create(characterToCreate);
            character.IsBot = true;
            character.transform.position = _battleScene.StartEnemiesPositions[i].position;
            character.SetupTeam(Team.Enemies);
            _temploaryInfo.EnemiesCharacters.Add(character);
        
            var box = character.gameObject.AddComponent<BoxShape>();
            box.Initialize(_shapeCollection);
        }
    }
}
