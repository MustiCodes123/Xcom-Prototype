using UnityEngine;
using Zenject;

public class ThreeToOneCombatSpawner : MonoBehaviour
{
    [SerializeField] private BattleScene _battleScene;
    [SerializeField] private UICombatCharacterHolder _uICharacterHolder;

    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private BaseCharacerView.Factory _characterFactory;
    [Inject] private CharactersRegistry _charactersRegistry;
    [Inject] private PvPBattleData _battleData;
    [Inject] private PlayerData _playerData;

    private void Awake()
    {
        if (_temploaryInfo.CurrentMode.GameMode != GameMode.ThreeToOne) return;
        SpawnAllyTeam();
        SpawnBoss();
        foreach (var character in _charactersRegistry.Characters)
        {
            character.CharacterView.SetAttackState();
        }
    }   

    private void SpawnAllyTeam()
    {
        var charactersToCreate = _temploaryInfo.SelectedCharacters;

        int index = 0;

        for (int i = 0; i < charactersToCreate.Count; i++)
        {
            charactersToCreate[i].Health = charactersToCreate[i].GetMaxHP();
            charactersToCreate[i].Mana = charactersToCreate[i].GetMaxMana();
            if (PlayerPrefs.HasKey("ENABLE_INFINITY_MANA"))
                charactersToCreate[i].Mana = int.MaxValue;

            var character = _characterFactory.Create(charactersToCreate[i]);
            _temploaryInfo.AddCreatedCharacter(character);

            character.transform.position = _battleScene.SpawnCharacterPositions[index].position;
            character.SetupTeam(Team.Allies);
            if (PlayerPrefs.HasKey("ENABLE_INFINITY_HEALTH"))
                character.IsImmortal = true;

            index++;
        }

        for (int i = 0; i < _temploaryInfo.FakeTeamMembers.Count; i++)
        {
            for (int j = 0; j < _temploaryInfo.FakeTeamMembers[i].Characters.Length; j++)
            {
                if (j < GameConstants.ThreeToOneCharacterLimit)
                {
                    var allyCharacter = _temploaryInfo.FakeTeamMembers[i].Characters[j];
                    var characterToCreate = allyCharacter.CharacterPreset.CreateAlly(allyCharacter.EnemyStats);
                    characterToCreate.Health = characterToCreate.GetMaxHP();
                    characterToCreate.Mana = characterToCreate.GetMaxMana();
                    var character = _characterFactory.Create(characterToCreate);
                    character.IsBot = true;
                    character.transform.position = _battleScene.SpawnCharacterPositions[index].position;
                    character.SetupTeam(Team.Allies);
                    index++;
                }
            }
        }
    }

    private void SpawnBoss()
    {        
        var boss = _temploaryInfo.CurrentBoss;

        var bossModel = boss.BossPreset.CreateBoss(boss, ((int)boss.Difficulty));
        bossModel.Health = bossModel.GetMaxHP();
        bossModel.Mana = bossModel.GetMaxMana();

        var bossView = _characterFactory.Create(bossModel);
        bossView.IsBot = true;
        bossView.IsBoss = true;
        bossView.transform.position = _battleScene.StartEnemiesPositions[0].position;
        bossView.SetupTeam(Team.Enemies);
        bossView.OnDie += AddProgress;

        _temploaryInfo.EnemiesCharacters.Clear();
        _temploaryInfo.EnemiesCharacters.Add(bossView);
        _uICharacterHolder.CreateBossSlider();
    }

    private void AddProgress()
    {
        _playerData.UpdateBossProgress(_temploaryInfo.CurrentBoss);
    }
}
