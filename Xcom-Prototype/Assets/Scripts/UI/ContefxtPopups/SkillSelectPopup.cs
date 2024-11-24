using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class SkillSelectPopup : MonoBehaviour
{
    [Inject] private PlayerData playerData;

    [SerializeField] private Transform skillParent;
    [SerializeField] private Transform _avalableSkills;
    [SerializeField] private SkillCard skillprefab;
    [SerializeField] private TalentsPanelView talentsPanelView;
    [SerializeField] private SkillSlotManager _skillSlotManager;
    [SerializeField] private Button _closeButton;

    private List<SkillCard> _avalableSkillCards = new List<SkillCard>();
    private List<SkillSlot> _avalableSkillSlots = new List<SkillSlot>();

    private BaseCharacterModel _currentCharacter;

    protected virtual void Awake()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    public void SetupCharacter(BaseCharacterModel character)
    {
        _currentCharacter = character;
    }

    public void ShowWindow()
    {
        gameObject.SetActive(true);
    }

    public void ShowSkillSlots()
    {
        var allSkillSlots = _skillSlotManager.GetAllSlots();

        foreach (var slot in allSkillSlots)
        {
            slot.gameObject.SetActive(true);
            slot.SetLock();
        }

        int slotsCount = _skillSlotManager.GetSkillCountByRare(_currentCharacter.Rare);

        for (int i = 0; i < slotsCount; i++)
        {
            allSkillSlots[i].SetOpen();
            _avalableSkillSlots.Add(allSkillSlots[i]);
        }
    }

    public BaseCharacterModel GetCurrentCharacter()
    {
        return _currentCharacter;
    }

    public Transform GetAvalableSkillsParent()
    {
        return skillParent;
    }

    public void SetCardInList(BaseSkillModel skill)
    {
        var skillCard = Instantiate(skillprefab, skillParent);
        _avalableSkillCards.Add(skillCard);
        skillCard.Setup(_currentCharacter, this, playerData, true);
        skillCard.SetupSkill(skill);
        skillCard.OnButtonClicked = null;
    }

    public void ClearPopup()
    {
        foreach (var card in _avalableSkillCards)
        {
            Destroy(card.gameObject);
        }

        _avalableSkillCards.Clear();
    }
    public void RemoveSkillByID(BaseSkillModel skillToRemove)
    {
        foreach (var skill in _currentCharacter.SkillsInUse)
        {
            if (skill.Id == skillToRemove.Id)
            {
                _currentCharacter.SkillsInUse.Remove(skill);
                skill.isInUse = false;
                return;
            }
        }
    }

    public List<SkillSlot> GetAvalableSkillSlots()
    {
        return _avalableSkillSlots;
    }
    
    public void AddCard(SkillCard skillCard)
    {
        _avalableSkillCards.Add(skillCard);
    }

    public List<SkillCard> GetAvalableSkillCards()
    {
        return _avalableSkillCards;
    }

    private void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }


    protected virtual void OnDestroy()
    {
        _closeButton.onClick.RemoveAllListeners();
    }
}
