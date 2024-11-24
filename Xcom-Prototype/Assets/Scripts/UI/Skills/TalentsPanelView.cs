using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class TalentsPanelView : MonoBehaviour
{
    [SerializeField] private SkillSlotManager _slotManager;
    [SerializeField] private TextMeshProUGUI talentPointsText;
    [SerializeField] private SkillCard skillprefab;

    [Inject] private PlayerData playerData;
    [Inject] private SkillSelectPopup _skillSelectPopup;

    private BaseCharacterModel _currentCharacter;
    private List<SkillCard> _skillsInUse = new List<SkillCard>();

    public void SetupTalents(BaseCharacterModel baseCharacterModel)
    {
        _skillSelectPopup.ClearPopup();
        CleanSlots();
        _currentCharacter = baseCharacterModel;
        _skillSelectPopup.SetupCharacter(baseCharacterModel);
        _slotManager.SetCharacter(baseCharacterModel);
        _skillSelectPopup.ShowSkillSlots();

        var avalableSkills = baseCharacterModel.GetActiveSkills();

        CheckSkillsAvalability(baseCharacterModel, avalableSkills);

        if (avalableSkills != null && avalableSkills.Count > 0)
        {
            foreach (var skill in avalableSkills)
            {
                if (!IsSkillEquiped(baseCharacterModel, skill))
                {
                    _skillSelectPopup.SetCardInList(skill);
                }
                else
                {
                    SetCardOnCharacter(skill);
                }
            }
        }

        foreach (var slot in _slotManager.GetAllSlots())
        {
            slot.Button.onClick.AddListener(_skillSelectPopup.ShowWindow);
        }  
    }

    public void SetCardOnCharacter(BaseSkillModel skill)
    {
        var slot = _slotManager.GetFreeSlot();
        slot.IsFull = true;
        var skillCard = Instantiate(skillprefab, slot.transform);
        skillCard.Setup(_currentCharacter, _skillSelectPopup, playerData, false);
        _skillsInUse.Add(skillCard);
        skillCard.SetupSkill(skill);
        skillCard.SetInSlot(slot);
        skillCard.transform.localPosition = Vector3.zero;
    }

    public void Hide()
    {
        _skillSelectPopup.ClearPopup();
        _skillSelectPopup.gameObject.SetActive(false);
        CleanSlots();
    }

    private bool IsSkillEquiped(BaseCharacterModel character, BaseSkillModel skill)
    {
        if (character.SkillsInUse.Count <= 0) return false;

        for (int i = 0; i < character.SkillsInUse.Count; i++)
        {
            if (character.SkillsInUse[i].Id == skill.Id)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckSkillsAvalability(BaseCharacterModel character, List<BaseSkillModel> avalableSkills)
    {

        bool isSkillAvalable = false;

        for (int i = 0; i < character.SkillsInUse.Count; i++)
        {
            for(int y = 0; y < avalableSkills.Count; y++)
            {
                if (character.SkillsInUse[i].Id == avalableSkills[y].Id)
                {
                    isSkillAvalable = true;
                }
            }

            if (!isSkillAvalable)
            {
                _skillSelectPopup.RemoveSkillByID(character.SkillsInUse[i]);
            }
        }
    }

    private void CleanSlots()
    {
        foreach (var skillCard in _skillsInUse)
        {
            Destroy(skillCard.gameObject);
        }

        _skillsInUse.Clear();
    }

    public void SetCharacterToSlotManager(BaseCharacterModel character)
    {
        _slotManager.SetCharacter(character);
    }
}
