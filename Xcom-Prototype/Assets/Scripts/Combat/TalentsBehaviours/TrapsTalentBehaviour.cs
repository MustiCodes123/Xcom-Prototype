using System.Collections.Generic;
using UnityEngine;

public class TrapsTalentBehaviour : BaseSkillBehaviour
{
    private Collider[] colliders;
    private Traps _trap;
    private List<BaseParticleView> _particles = new List<BaseParticleView>();
    private List<BaseProjectile> _projectiles = new List<BaseProjectile>();
    private float _farDamageFactor = 0.5f;
    private int _trapsCount = 3;

    public TrapsTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);
        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _projectile = GetActivatedTrap();
        _target = _projectile.GetTarget();
        _target.TakeDamage(Skill.Value, Skill.DamageType);

        var bang = BangBehaveour(_target, Skill.OnCollisionParticle);
        colliders = Physics.OverlapSphere(_selfCharacter.transform.position, Skill.DamageRange);

        if (colliders.Length != 0)
        {
            foreach (Collider collider in colliders)
            {
                var damageble = collider.GetComponent<IDamageable>();
                if (damageble != null && damageble is BaseCharacerView target && !_selfCharacter.IsMyTeammate(target))
                {
                    int farDamage = (int)(Skill.Value * _farDamageFactor);
                    damageble.TakeDamage(farDamage, AttackType.Magical, 100);
                    _projectile.IsActive = false;

                    if (!target.SkillServiceProvider.IsBuffOnMe(Skill.OnCollisionBuff))
                    {
                        var burn = GetBuff(Skill.OnCollisionBuff);
                        target.SkillServiceProvider.AddBuff(burn);
                    }
                }
            }
        }
    }

    protected override void CreateParticleAndProjectile(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;

        if (_decale is TrapsDecale traps && Skill.ProjectileType == ProjectileType.TrapsProjectile)
        {
            for (int i = 0; i < traps.Targets.Length; i++)
            {
                var particle = _particleFactory.Create(Skill.ParticleType);
                _particles.Add(particle);
                var projectile = _projectileFactory.Create(Skill.ProjectileType);
                _projectiles.Add(projectile);
                projectile.Setup(target, selfCharacter, particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
                Traps trapsProjectile = projectile as Traps;
                projectile.transform.position = traps.Targets[i].transform.position;
                particle.SetParent(projectile.transform);
            }
        }
        else
        {
            var decale = CreateDecale(target.transform);
            _decale = decale;

            if (_decale is TrapsDecale && Skill.ProjectileType == ProjectileType.TrapsProjectile)
            {
                _decale.Target.position = _target.transform.position;

                CreateParticleAndProjectile(_target, selfCharacter);
            }
            else
            {
                base.CreateParticleAndProjectile(_target, _selfCharacter);
                _particle.SetParent(_projectile.transform);
                _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
            }
            decale.OnDispawned();
            _decale = null;
        }
    }

    private Traps GetActivatedTrap()
    {
        foreach (var projectile in _projectiles)
        {
            if (projectile is Traps trap && trap.IsActivated == true)
            {
                _projectiles.Remove(projectile);
                return trap;
            }  
        }
        return null;
    }
}