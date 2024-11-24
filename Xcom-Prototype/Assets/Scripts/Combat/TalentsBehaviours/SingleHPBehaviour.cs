using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHPBehaviour : BaseSkillBehaviour
{
    public SingleHPBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {

        cooldown = 5;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {

    

        var friendlyTarget = _charactersRegistry.GetRandomCharacter(selfCharacter.Team);

        var view = friendlyTarget.CharacterView;

        if(view != null)
        {
            var buff = new HPBuff();
            buff.Duration = Skill.Duration;
            buff.AdditionalHP = Skill.Value;

            var particle = _particleFactory.Create(Skill.ParticleType);
            particle.SetParent(view.transform);

            view.SkillServiceProvider.AddBuff(buff);
        }

        return base.Use(target, selfCharacter);
    }

}
