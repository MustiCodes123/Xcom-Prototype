using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWheelTalentBehaviour : BaseSkillBehaviour
{
    public FireWheelTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }
    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);
        _particle.SetParent(_projectile.transform);

        return base.Use(target, selfCharacter);
    }

    private void OnHit()
    {


        _target = _projectile.GetTarget();

        var bang = BangBehaveour(_target, Skill.OnCollisionParticle);
        int damage = Skill.Value;
        
        var damageble = _target.GetComponent<IDamageable>();
        if (damageble != null && damageble is BaseCharacerView target && !_selfCharacter.IsMyTeammate(target))
        {
            damageble.TakeDamage(damage, AttackType.Magical, damage);
            _projectile.IsActive = false;

            var burn = GetBuff(Skill.OnCollisionBuff);
            target.SkillServiceProvider.AddBuff(burn);

        }
    }
}
