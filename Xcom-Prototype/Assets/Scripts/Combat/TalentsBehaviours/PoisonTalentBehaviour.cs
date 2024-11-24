using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTalentBehaviour : BaseSkillBehaviour
{

    public PoisonTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {

       var particle = _particleFactory.Create(Skill.ParticleType);
        particle.transform.SetParent(target.transform);
        particle.transform.localPosition = Vector3.zero;

        var debuff = new PoisonDebuff();

        debuff.damage = Skill.Value;
        debuff.duration = Skill.Duration;

        target.SkillServiceProvider.AddBuff(debuff);

        return base.Use(target, selfCharacter);
    }

}
