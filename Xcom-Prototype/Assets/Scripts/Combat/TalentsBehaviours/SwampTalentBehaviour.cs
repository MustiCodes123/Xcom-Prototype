using System.Collections.Generic;

public class SwampTalentBehaviour : BaseSkillBehaviour
{
    private IBuff _debuff;

    public SwampTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
        _particle.SetParent(_projectile.transform);

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