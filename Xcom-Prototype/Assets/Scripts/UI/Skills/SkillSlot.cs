using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, IDropHandler
{
    public bool IsFull;
    public Button Button;
    public Button RemoveSkillButton;
    
    [SerializeField] private Image _IconSlot;
    [SerializeField] private Transform _lock;
    

    [SerializeField] private SkillSlotManager _slotManager;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        
        if(!IsFull)
        {
            SkillCard skillCard = eventData.pointerDrag.GetComponent<SkillCard>();
            if (skillCard == null) return;

            SetSkill(skillCard);
            skillCard.SetInSlot(this);
            skillCard.Skill.isInUse = true;
            skillCard.SetInSlotState();
            IsFull = true;
        }
    }

    public void SetSkill(SkillCard skillCard)
    {
        skillCard.transform.parent = transform;
        skillCard.transform.localPosition = Vector3.zero;

        BaseCharacterModel character = _slotManager.GetCurrrentCharacter();
        
        character.SkillsInUse.Add(skillCard.Skill);
        RemoveButtonSetActive(true);
    }

    public void TryAssignSkill(SkillCard skillCard)
    {
        if (!IsFull)
        {
            SetSkill(skillCard);
            skillCard.SetInSlot(this);
            skillCard.Skill.isInUse = true;
            skillCard.SetInSlotState();
            IsFull = true;
        }
    }
    
    
    public void SetLock()
    {
        IsFull = true;
        _lock.gameObject.SetActive(true);
        RemoveButtonSetActive(false);
    }

    public void SetOpen()
    {
        IsFull = false;
        _lock.gameObject.SetActive(false);
        RemoveButtonSetActive(false);
    }

    private void OnDestroy()
    {
        Button.onClick.RemoveAllListeners();
        RemoveSkillButton.onClick.RemoveAllListeners();
    }

    public void RemoveButtonSetActive(bool value)
    {
        RemoveSkillButton.gameObject.SetActive(value);
    }
}