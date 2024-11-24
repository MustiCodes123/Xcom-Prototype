using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SkillSlotManager : MonoBehaviour
{
    [SerializeField] private List<SkillSlot> _slots;

    private BaseCharacterModel _currentCharacter;

    private Dictionary<RareEnum, int> rareToSkillSlotsCount = new Dictionary<RareEnum, int>()
    {
        {RareEnum.Common, 1 },
        {RareEnum.Rare, 2 },
        {RareEnum.Epic, 3 },
        {RareEnum.Legendary, 4 },
        {RareEnum.Mythical, 5 },
    };

    public void SetCharacter(BaseCharacterModel character)
    {
        _currentCharacter = character;
    }

    public SkillSlot GetFreeSlot()
    {
        foreach(var slot in _slots)
        {
            if(!slot.IsFull)
            {
                slot.IsFull = true;
                return slot;
            }
        }
        return null;
    }

    public int GetSkillCountByRare(RareEnum rare)
    {
        if (rareToSkillSlotsCount.ContainsKey(rare))
        {
            return rareToSkillSlotsCount[rare];
        }

        return 0;
    }

    public List<SkillSlot> GetAllSlots()
    {
        return _slots;
    }

    public BaseCharacterModel GetCurrrentCharacter()
    {
        return _currentCharacter;
    }
}