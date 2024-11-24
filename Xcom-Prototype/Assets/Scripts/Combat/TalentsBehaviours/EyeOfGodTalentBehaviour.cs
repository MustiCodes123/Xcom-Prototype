using System.Collections;
using UnityEngine;
public class EyeOfGodTalentBehaviour : BaseSkillBehaviour
{
    private IBuff _debuff;

    public EyeOfGodTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        if (_projectile is WaveThroughWithDecale wave)
        {
            wave.Trail = _particle;
        }

        _particle.transform.position = selfCharacter.transform.position;

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

    public void OnExit()
    {
        _target.SkillServiceProvider.RemoveBuff(_debuff);
    }
}