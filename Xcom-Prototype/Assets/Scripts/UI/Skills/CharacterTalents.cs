using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class CharacterTalents 
{

    [JsonIgnore] public List<BaseSkillModel> Talents;
    public Dictionary<int,int> SkillsIdsLevels;
    public int TalentPoints;

    private BaseCharacterModel _ownerModel;

    public CharacterTalents (BaseCharacterModel ownerModel)
    {
        _ownerModel = ownerModel;
    }

    public void RemoveSkill(BaseSkillModel toRemove)
    {
        _ownerModel.SkillsInUse.Remove(toRemove);
    }

    public void AddSkill(BaseSkillModel skillToAdd)
    {
        _ownerModel.SkillsInUse.Add(skillToAdd);
    }

    public bool HasDualSkill()
    {
        foreach (var skill in _ownerModel.CharacterTalents.Talents)
        {
            if (skill.Id == TalentsEnum.Duals)
                return true;
        }

        return false;
    }

    public void GenerateTalents(TalentsEnum[] talents)
    {
        _ownerModel.SkillsInUse = new List<BaseSkillModel>();
        for (int i = 0; i < talents.Length; i++)
        {
            for (int x = 0; x < SkillsDataInfo.Instance.BaseSkillModels.Length; x++)
            {
                if (SkillsDataInfo.Instance.BaseSkillModels[x].Id == talents[i])
                {
                    var skill = SkillsDataInfo.Instance.BaseSkillModels[x].GetSkill();
                    _ownerModel.SkillsInUse.Add(skill);
                }
            }
        }
    }

    public bool AddSkillPoint(TalentsEnum skillId)
    {
        for (int i = 0; i < _ownerModel.SkillsInUse.Count; i++)
        {
            if (_ownerModel.SkillsInUse[i].Id == skillId)
            {
                _ownerModel.SkillsInUse[i].Level++;
                TalentPoints--;
                return true;
            }
        }
        return false;
    }

    [OnSerializing]
    protected void OnSerializing (StreamingContext context)
    {
        //_ownerModel.SkillsInUse = new List<BaseSkillModel>();
        SkillsIdsLevels = new Dictionary<int, int>();
        if (_ownerModel.SkillsInUse != null)
        {
            foreach (var skill in _ownerModel.SkillsInUse)
            {
                if (skill != null)
                {
                    if (SkillsIdsLevels.ContainsKey((int)skill.Id))
                    {
                        SkillsIdsLevels[(int)skill.Id] = skill.Level;
                    }
                    else
                    {
                        SkillsIdsLevels.Add((int)skill.Id, skill.Level);
                    }
                }
            }
        }
       
    }

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext context)
    {
        if (SkillsDataInfo.Instance != null)
        {
            if (_ownerModel.SkillsInUse == null)
            {
                _ownerModel.SkillsInUse = new List<BaseSkillModel>();
            }

            int skillIndex = 0;
            if (SkillsIdsLevels != null)
            {
                foreach (var item in SkillsIdsLevels)
                {
                    for (int x = 0; x < SkillsDataInfo.Instance.BaseSkillModels.Length; x++)
                    {
                        if ((int)SkillsDataInfo.Instance.BaseSkillModels[x].Id == item.Key)
                        { 
                            var skillInfo = SkillsDataInfo.Instance.BaseSkillModels[x].GetSkill();
                            skillInfo.Level = item.Value;
                            _ownerModel.SkillsInUse.Add(skillInfo);
                            skillIndex++;
                        }
                    }
                }
            }
        }
    }

    public void AddTalentPoint(int points)
    {
        TalentPoints += (1 + points);
    }
}
