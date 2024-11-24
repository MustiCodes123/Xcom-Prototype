using System.Collections;
using UnityEngine;

public class LightingBallTalentBehaviour : BaseSkillBehaviour
{
    private BaseProjectile.Factory projectileFactory;

    private Collider[] colliders;

    public LightingBallTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        this.projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;
        var particle = base._particleFactory.Create(Skill.ParticleType);

        var projectile = projectileFactory.Create(Skill.ProjectileType);
        projectile.Setup(target, selfCharacter, particle, Skill, OnHit, _decale);
        _projectile = projectile;
        particle.SetParent(projectile.transform);

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        if (!IsAutobattle(_selfCharacter))
        {
            _target = _projectile.GetTarget();
        }

        var bang = BangBehaveour(_target, Skill.OnCollisionParticle);
        int damage = Skill.Value;
        colliders = Physics.OverlapSphere(_selfCharacter.transform.position, Skill.DamageRange);

        if (colliders.Length != 0)
        {
            foreach (Collider collider in colliders)
            {
                var damageble = collider.GetComponent<IDamageable>();
                if (damageble != null && damageble is BaseCharacerView target && !_selfCharacter.IsMyTeammate(target))
                {
                    damageble.TakeDamage(damage, AttackType.Magical, 100);
                    _projectile.IsActive = false;
                    
                    var burn = new ElectricDebuff(_particleFactory, Skill);
                    burn.damage = damage;
                    target.SkillServiceProvider.AddBuff(burn);
                }
            }
        }
    }
}