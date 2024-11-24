using UnityEngine;

public class InstantStrikeTalentBehaviour : BaseSkillBehaviour
{
    private BaseProjectile.Factory projectileFactory;

    public InstantStrikeTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        this.projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        var particle = base._particleFactory.Create(Skill.ParticleType);

        var projectile = projectileFactory.Create(Skill.ProjectileType);
        projectile.Setup(target, selfCharacter, particle, Skill, OnHit, _decale);
        _projectile = projectile;

        if (_projectile is VectorThroughWithDecale vector)
        {
            vector.trail = particle;
        }

        particle.transform.position = selfCharacter.transform.position;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();
        int damage = Skill.Value;
        _target.TakeDamage(damage, AttackType.Magical);
    }

    public BaseParticleView IceTrailBehaveour(BaseCharacerView target)
    {
        return null;
    }
}