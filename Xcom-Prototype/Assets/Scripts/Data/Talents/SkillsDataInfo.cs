using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SkillsDataInfo", menuName = "Data/Skills/SkillsDataInfo")]
public class SkillsDataInfo : ScriptableObject
{
    public static SkillsDataInfo Instance;

    public BaseSkillTemplate[] BaseSkillModels;

    public CharacterPreset[] CharacterPresets;

    [Inject]
    public SkillsDataInfo()
    {
        Instance = this;
    }

    public BaseSkillTemplate GetSkillTemplate(int ID)
    {
        foreach (var skill in BaseSkillModels)
        {
            if ((int)skill.Id == ID)
                return skill;
        }

        Debug.LogError($"BaseSkillModels does not contain skill with ID {ID}");

        return null;
    }
}
