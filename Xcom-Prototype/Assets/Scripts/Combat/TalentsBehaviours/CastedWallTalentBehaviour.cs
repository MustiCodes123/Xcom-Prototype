using System.Collections;
using UnityEngine;

public class CastedWallTalentBehaviour : BaseSkillBehaviour
{
    public CastedWallTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _particle.SetParent(_projectile.transform);
        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        if (!IsAutobattle(_selfCharacter))
        {
            _target = _projectile.GetTarget();
        }
        if (_target != null && !_target.SkillServiceProvider.IsBuffOnMe(Skill.OnCollisionBuff))
        {
            var deBuff = GetBuff(Skill.OnCollisionBuff);
            _target.SkillServiceProvider.AddBuff(deBuff);
        }
    }
}