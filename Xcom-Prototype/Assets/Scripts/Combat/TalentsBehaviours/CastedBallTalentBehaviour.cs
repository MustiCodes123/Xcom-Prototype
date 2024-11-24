using UnityEngine;

public class CastedBallTalentBehaviour : BaseSkillBehaviour
{
    private Collider[] colliders;
    public CastedBallTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);
        _particle.SetParent(_projectile.transform);

        return base.Use(target, selfCharacter);
    }

    private void OnHit()
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

                    var burn = GetBuff(Skill.OnCollisionBuff);
                    target.SkillServiceProvider.AddBuff(burn);

                }
            }
        }
    }
}
