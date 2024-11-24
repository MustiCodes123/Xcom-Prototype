using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPreset", menuName = "Data/Characters/BossPreset")]
public class BossPreset : ScriptableObject
{
    public Sprite CharacterSprite;
    public RareEnum Rare;
    public CharacterRace Race;
    public CharacterBehaviourEnum CharacterBehaviourEnum;
    public string PresetName;
    public string BackGroundPath;
    public TalentsEnum[] Talents;
    public BossStats[] BossStats;
    public BossLevelUpModel LevelUpModel;

    public BaseCharacterModel CreateBoss(BossData bossData, int difficultyIndex)
    {
        BossModel model = new BossModel(bossData, difficultyIndex);
        model.Name = PresetName;
        model.Avatar = CharacterSprite;
        model.AvatarId = CharacterSprite.name;

        model.LevelUpFactor = GetLevelUpFactor((Difficulty)difficultyIndex);
        return model;
    }

    public float GetLevelUpFactor(Difficulty difficulty)
    {
        float levelUpFactor = GeteDifficultyToLevelUpFactor(LevelUpModel)[difficulty];
        return levelUpFactor;
    }

    private Dictionary<Difficulty, float> GeteDifficultyToLevelUpFactor(BossLevelUpModel levelUpModel)
    {
        return new Dictionary<Difficulty, float>()
        {
            {Difficulty.Easy, levelUpModel.Easy},
            {Difficulty.Rare, levelUpModel.Rare},
            {Difficulty.Epic, levelUpModel.Epic},
            {Difficulty.Legendary, levelUpModel.Legendary},
            {Difficulty.Mythical, levelUpModel.Mythical},
        };
    }
}