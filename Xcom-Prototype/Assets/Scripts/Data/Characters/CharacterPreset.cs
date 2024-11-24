using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;
using UniRx;
using UnityEngine.Serialization;

public enum CharacterID
{
    Barbarian,
    FatBarbarian,
    GoblinCommon,
    GoblinWarriorAxe,
    GoblinWarriorRare,
    Knight,
    Ninja,
    RedOrc,
    Sceleton,
    SceletonRare,
    SceletonUncommon,
    SwordWarrior,
    DwarfGirl,
    MageGirl,
    Bandit,
    Dwarf,
    Genie,
    DragonBurnBlue,
    DragonBurnGold,
    DragonBurnGreen,
    DragonBurnRed,
    ElfGirl,
}

[CreateAssetMenu(fileName = "CharacterPreset", menuName = "Data/Characters/CharacterPreset")]
public class CharacterPreset : ScriptableObject
{
    public CharacterID CharacterID;
    public Sprite CharacterSprite;
    public RareEnum Rare;
    public CharacterRace Race;
    public CharacterBehaviourEnum CharacterBehaviourEnum;
    public string PresetName;
    public string BackGroundPath;
    public TalentsEnum[] Talents;
    public CharacterPreset ParentPreset;
    [FormerlySerializedAs("Parameters")] [SerializeField] private CharacterParameters _parameters;
    public CharacterParameters Parameters => ParentPreset != null ? ParentPreset.Parameters : _parameters;
    [FormerlySerializedAs("LevelUpParameters")] [SerializeField] private LevelUpParameters _levelUpParameters;
    public LevelUpParameters LevelUpParameters => ParentPreset != null ? ParentPreset.LevelUpParameters : _levelUpParameters;
    [FormerlySerializedAs("RareLevelUpModel")] [SerializeField] private RareToLevelUpModel _rareLevelUpModel;
    public RareToLevelUpModel RareLevelUpModel => ParentPreset != null ? ParentPreset.RareLevelUpModel : _rareLevelUpModel;
    [FormerlySerializedAs("Stats")] [SerializeField] private Stat[] _stats;
    public Stat[] Stats => ParentPreset != null ? ParentPreset.Stats : _stats;
    public int Stars;
    public List<ItemOffsetData> ItemsOffsetData = new();

    public BaseCharacterModel CreateCharacter()
    {
        BaseCharacterModel character = new WarriorCharacterInfo();

        character.CharacterID = this.CharacterID;
        character.LevelUpParameters = LevelUpParameters;

        character.ItemsOffsetData = ItemsOffsetData;

        character.Rare = Rare;
        character.Level = 1;
        character.Name = PresetName;
        character.Avatar = CharacterSprite;
        character.AvatarId = CharacterSprite.name;
        character.BackGroundPath = BackGroundPath;
        character.Stars = (int)Rare + 1;
        character.Race = Race;

        character.MaxHealth = Parameters.Health;
        character.Health = Parameters.Health;
        character.MaxMana = Parameters.Mana;
        character.MoveSpeed = Parameters.MoveSpeed;
        character.LiftingCapacity = Parameters.LiftingCapacity;
        character.Attack = Parameters.Attack;
        character.AdditionalAttackSpeed = Parameters.AdditionalAttackSpeed;
        character.CriticalDamage = Parameters.CriticalDamage;
        character.CriticalChance = Parameters.CriticalChance;
        character.MagicCriticalDamage = Parameters.MagicCriticalDamage;
        character.MagicCriticalChance = Parameters.MagicCriticalChance;
        character.Magic = Parameters.Magic;
        character.MagicDefense = Parameters.MagicDefense;
        character.BlockChance = Parameters.BlockChance;
        character.DodgeAdditional = Parameters.DodgeAdditional;
        character.MagicalResistance = Parameters.MagicalResistance;
        character.PhysicalResistance = Parameters.PhysicalResistance;
        character.Stats = Stats;

        character.CharacterTalents = new CharacterTalents(character);
        character.CharacterTalents.GenerateTalents(Talents);
        character.characterBehaviourEnum = CharacterBehaviourEnum;

        character.LevelUpParameters.RareToLevelUpCoef = GeteRareToLevelUpFactor(RareLevelUpModel);
        character.XpToNextLevel = LevelUpParameters.XpToNextLevel;

        SetupPassiveTalents(character);

        return character;
    }

    public BaseCharacterModel CreateCharacter(RareEnum rare, int level, int currentXP, int addXP)
    {
        BaseCharacterModel character = this.CreateCharacter();

        while (character.Rare < rare)
        {
            while (!character.IsMaxLevel)
                character.LevelUp();
            character.RankUp();
        }

        while (character.Level < level)
            character.LevelUp();

        character.Xp = currentXP;
        character.AddXP(addXP);
        return character; 
    }

    public BaseCharacterModel CreateCharacter(EnemyStats enemyStats, CharacterPreset characterPreset)
    {
        BaseCharacterModel model = new BaseCharacterModel(characterPreset, enemyStats);

        model.CharacterID = this.CharacterID;
        model.Name = PresetName;
        model.Avatar = CharacterSprite;
        model.AvatarId = CharacterSprite.name;
        model.ItemsOffsetData = ItemsOffsetData;

        SetupPassiveTalents(model);

        return model;
    }

    public BaseCharacterModel CreateAlly(EnemyStats enemyStats)
    {
        BaseCharacterModel model = new BaseCharacterModel(this, enemyStats);

        model.CharacterID = this.CharacterID;
        model.LevelUpParameters = LevelUpParameters;
        model.Name = PresetName;
        model.Avatar = CharacterSprite;
        model.AvatarId = CharacterSprite.name;
        model.BackGroundPath = BackGroundPath;
        model.Stars = (int)Rare + 1;
        model.Race = Race;
        model.LevelUpParameters.RareToLevelUpCoef = GeteRareToLevelUpFactor(RareLevelUpModel);
        model.XpToNextLevel = LevelUpParameters.XpToNextLevel;

        model.ItemsOffsetData = ItemsOffsetData;

        for (int i = 0; i < enemyStats.Level; i++)
        {
            model.LevelUp();
        }

        SetupPassiveTalents(model);

        return model;
    }

    [ContextMenu("AddCharacter")]
    public void AddCharacter()
    {
        for(int i = 0; i < 5; i++)
        {
            PlayerData playerData = FindObjectOfType<BossWindowView>(true).PlayerData;
            var character = CreateCharacter();
            playerData.PlayerGroup.AddCharacterToNotAsignedGroup(character);
        }
    }

    private Dictionary<RareEnum, int> GeteRareToLevelUpFactor(RareToLevelUpModel levelUpModel)
    {
        return new Dictionary<RareEnum, int>()
        {
            {RareEnum.Common, levelUpModel.Common},
            {RareEnum.Rare, levelUpModel.Rare},
            {RareEnum.Epic, levelUpModel.Epic},
            {RareEnum.Legendary, levelUpModel.Legendary},
            {RareEnum.Mythical, levelUpModel.Mythical}
        };
    }

    private void SetupPassiveTalents(BaseCharacterModel character)
    {
        PassiveTalentsFactory passiveTalentsFactory = new PassiveTalentsFactory();
        List<BaseSkillModel> passiveTalents = passiveTalentsFactory.GetPassiveTalentsForCharacter(character.CharacterID);
        character.CharacterTalents.Talents = new List<BaseSkillModel>(passiveTalents);
    }
}
