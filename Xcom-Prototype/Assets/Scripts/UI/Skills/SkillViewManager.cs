using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillViewManager: MonoBehaviour
{    
    [SerializeField] private UISkillHolder _skillHolder;

    private SkillView[] _skillObjects => _skillHolder.SkillObjects;
    private List<BaseSkillBehaviour> _skills = new List<BaseSkillBehaviour>();
    private BaseCharacerView _currentCharacter;

    public void UpDateCharracter(BaseCharacerView character)
    {
        ClearSkillsObjects();
        _currentCharacter = character;
        var talents = _currentCharacter.SkillServiceProvider.SkillsInUse;

        if (talents.Count > 0)
        {
            _skills = talents.ToList();

            for (int i = 0; i < _skills.Count; i++)
            {
                if (IsSkillUsable(talents[i], _currentCharacter))
                {
                    _skillObjects[i].AactivateJoystic();
                    _skillHolder.UpDateSkillSlot(_skillObjects[i], talents[i]);
                    _skillObjects[i].talent = talents[i];
                    _skillObjects[i].character = _currentCharacter;
                    if(_skillObjects[i].talent.GetDecale() == null)
                    {
                        _skillObjects[i].DeactivateJoystic();
                    }
                }
            }
        }
    }

    private bool IsSkillUsable(BaseSkillBehaviour skill, BaseCharacerView caster)
    {
        if(skill != null && skill.IsUsable() &&
          (skill.Skill.WeaponSkill == WeaponSkillEnum.None || caster.characterData.IsSkillWeaponEquip(skill.Skill.WeaponSkill)))
        {
            return true;
        }
        return false;
    }

    private void ClearSkillsObjects()
    {
        _skills.Clear();

        foreach (SkillView skill in _skillObjects)
        {
            skill.gameObject.SetActive(false);
        }
    }
}
