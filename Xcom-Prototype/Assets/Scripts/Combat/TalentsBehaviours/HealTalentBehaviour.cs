using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTalentBehaviour : BaseSkillBehaviour
{
    public HealTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        var friendlyTarget = _charactersRegistry.GetMinHPFriend(selfCharacter.Team);
        if (friendlyTarget.characterData.Health >= friendlyTarget.characterData.MaxHealth)
        {
            return false;
        }

        var particle = _particleFactory.Create(Skill.ParticleType);
        particle.transform.SetParent(friendlyTarget.transform);
        particle.transform.localPosition = Vector3.zero;

        //TODO calculate heal value
        int heal = Skill.Value;

        friendlyTarget.Heal(heal);

       return base.Use(friendlyTarget, selfCharacter);
         
    }
}
