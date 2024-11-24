using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDeBuffTalentBehaviour : BaseSkillBehaviour
{
    public DamageDeBuffTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {
        cooldown = 4;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        var parrticle = _particleFactory.Create(Skill.ParticleType);
        parrticle.SetParent(target.transform);

        var buff = new DamageBuff();
        buff.Duration = Skill.Duration;
        buff.DamageMultiplayer = target.characterData.GetDamage() * (Skill.Value / 100);

        target.SkillServiceProvider.AddBuff(buff);
        return base.Use(target, selfCharacter);
    }
}
