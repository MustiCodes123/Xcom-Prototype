using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBuffTalentBehaviour : BaseSkillBehaviour
{
    public DamageBuffTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {
        cooldown = 4;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        var friendlyTarget = _charactersRegistry.GetRandomCharacter(selfCharacter.Team, selfCharacter);
        if (friendlyTarget != null)
        {
            var parrticle = _particleFactory.Create(Skill.ParticleType);
            parrticle.SetParent(friendlyTarget.transform);

            var buff = new DamageBuff();
            buff.Duration = Skill.Duration;
            buff.DamageMultiplayer = friendlyTarget.characterData.GetDamage() * (Skill.Value / 100);

            friendlyTarget.SkillServiceProvider.AddBuff(buff);
        }

        return base.Use(target, selfCharacter);
    }

}
