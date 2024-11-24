using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class PassiveSkillData
{
    [field: SerializeField] public GameObject SkillObject { get; private set; }
    [field: SerializeField] public Image Icon { get; private set; }
    public BaseSkillModel Skill { get; set; }
}

[System.Serializable]
public class PassiveSkillsViewData
{
    [field: SerializeField] public PassiveSkillData[] Skills { get; private set; }
}

public class PassiveSkillsView : MonoBehaviour
{
    [SerializeField] private PassiveSkillsViewData _passiveSkillsViewData;

    [SerializeField] private GameObject _descriptionObject;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;

    public void DisplayPassiveSkills(BaseCharacterModel currentCharacter)
    {
        CharacterTalents characterTalents = currentCharacter.CharacterTalents;
        List<BaseSkillModel> passiveSkills = SortPassiveSkills(characterTalents);

        if(passiveSkills != null)
        {
            DisplaySkills(passiveSkills);
        }
    }

    public void ShowDescription()
    {
        _descriptionObject.SetActive(true);
    }

    private void DisplaySkills(List<BaseSkillModel> passiveSkills)
    {
        for (int i = 0; i < _passiveSkillsViewData.Skills.Length; i++)
        {
            PassiveSkillData skillData = _passiveSkillsViewData.Skills[i];

            if (i < passiveSkills.Count)
            {
                BaseSkillModel passiveSkill = passiveSkills[i];
                skillData.Skill = passiveSkill;
                skillData.Icon.sprite = passiveSkill.Icon;
                skillData.SkillObject.SetActive(true);
                _title.text = passiveSkill.Name;
                _description.text = passiveSkill.Description;
            }
            else
            {
                skillData.Skill = null;
                skillData.SkillObject.SetActive(false);
            }
        }
    }

    private List<BaseSkillModel> SortPassiveSkills(CharacterTalents characterTalents)
    {
        if (characterTalents == null || characterTalents.Talents == null)
        {
            return new List<BaseSkillModel>();
        }

        List<BaseSkillModel> passiveSkills = characterTalents.Talents
            .Where(skill => !skill.IsUssable)
            .ToList();

        return passiveSkills;
    }
}