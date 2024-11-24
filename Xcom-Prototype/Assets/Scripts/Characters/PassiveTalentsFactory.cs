using System.Collections.Generic;
using UnityEngine;

public class PassiveTalentsFactory
{
    public List<BaseSkillModel> GetPassiveTalentsForCharacter(CharacterID characterID)
    {
        List<BaseSkillModel> passiveTalents = new List<BaseSkillModel>();

        switch (characterID)
        {
            case CharacterID.Barbarian:
                passiveTalents.Add(GetSkillModel(TalentsEnum.Duals));
                break;
            case CharacterID.Knight:
                passiveTalents.Add(GetSkillModel(TalentsEnum.Duals));
                break;

            default:
                Debug.LogWarning($"No passive talents found for character ID: {characterID}");
                break;
        }

        return passiveTalents;
    }

    private BaseSkillModel GetSkillModel(TalentsEnum talentEnum)
    {
        BaseSkillTemplate skillTemplate = SkillsDataInfo.Instance.GetSkillTemplate((int)talentEnum);

        if (skillTemplate != null)
        {
            return skillTemplate.GetSkill();
        }

        Debug.LogError($"Skill template not found for talent enum: {talentEnum}");
        return null;
    }
}