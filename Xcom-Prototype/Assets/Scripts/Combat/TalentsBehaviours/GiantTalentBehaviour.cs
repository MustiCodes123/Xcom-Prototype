using System.Collections;
using UnityEngine;

public class GiantTalentBehaviour : BaseSkillBehaviour
{

    public GiantTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {

    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        base.Use(target, selfCharacter);
        var giantBuff = new GiantBuff(_particleFactory, Skill);
        selfCharacter.SkillServiceProvider.AddBuff(giantBuff);

        var particle = _particleFactory.Create(Skill.ParticleType);
        particle.SetParent(selfCharacter.transform);
        particle.duration = Skill.Duration;

        return true;
    }

}