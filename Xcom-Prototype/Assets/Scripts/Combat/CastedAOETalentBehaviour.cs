using System.Collections;
using UnityEngine;

public class CastedAOETalentBehaviour : BaseSkillBehaviour
{
    public CastedAOETalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;

        CreateParticleAndProjectile(target, selfCharacter);
        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
        _particle.SetParent(_projectile.transform);
        _particle.transform.localPosition = Vector3.zero;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();

        if (_target != null)
        {
            _target.TakeDamage(Skill.Value, Skill.DamageType);
            var deBuff = GetBuff(Skill.OnCollisionBuff);
            _target.SkillServiceProvider.AddBuff(deBuff);
        }
    }
}