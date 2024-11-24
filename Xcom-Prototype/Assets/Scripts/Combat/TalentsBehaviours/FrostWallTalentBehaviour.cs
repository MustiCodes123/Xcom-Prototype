using System.Collections;
using UnityEngine;

public class FrostWallTalentBehaviour : BaseSkillBehaviour
{
    private Collider[] _colliders;

    private int bangParticleSize = 2;
    public FrostWallTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
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
        var bang = BangBehaveour(_projectile.transform.position, Skill.OnCollisionParticle);
        bang.transform.localScale = Vector3.one * bangParticleSize;
        int damage = Skill.Value;
        _colliders = Physics.OverlapSphere(_projectile.transform.position, Skill.DamageRange);

        if (_colliders.Length != 0)
        {
            foreach (Collider collider in _colliders)
            {
                var damageble = collider.GetComponent<IDamageable>();
                if (damageble != null && damageble is BaseCharacerView target && !_selfCharacter.IsMyTeammate(target))
                {
                    damageble.TakeDamage(damage, Skill.DamageType);
                    _projectile.IsActive = false;


                    var burn = GetBuff(Skill.OnCollisionBuff);
                    target.SkillServiceProvider.AddBuff(burn);

                }
            }
        }
    }
}