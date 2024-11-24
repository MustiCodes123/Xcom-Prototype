using UnityEngine;

public class MeteorRainTalentBehaviour : BaseSkillBehaviour
{
    public MeteorRainTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;

        var particle = _particleFactory.Create(Skill.ParticleType);
        var projectile = _projectileFactory.Create(Skill.ProjectileType);
        projectile.Setup(target, selfCharacter, particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
        _projectile = projectile;

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