using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicArmorTalentBehaviour : BaseSkillBehaviour
{
    public MagicArmorTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        var characterTarget = _charactersRegistry.GetRandomCharacter(selfCharacter.Team);

        var particle = _particleFactory.Create(Skill.ParticleType);
        particle.transform.position = characterTarget.transform.position;

        characterTarget.AddArmor(Skill.Value);

        return base.Use(target, selfCharacter);
    }
    
}
