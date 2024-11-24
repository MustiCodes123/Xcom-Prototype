using System.Collections;
using UnityEngine;

public class CastedAuraTalentBehaviour : BaseSkillBehaviour
{
    private IBuff _debuff;

    public CastedAuraTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        _projectile.transform.SetParent(selfCharacter.transform);
        _particle.transform.SetParent(_projectile.transform);

        _projectile.transform.localPosition = Vector3.zero;
        _particle.transform.localPosition = Vector3.zero;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();

        if (_target != null)
        {
            var deBuff = GetBuff(Skill.OnCollisionBuff);
            _debuff = deBuff;
            _target.SkillServiceProvider.AddBuff(deBuff);
        }
    }
}