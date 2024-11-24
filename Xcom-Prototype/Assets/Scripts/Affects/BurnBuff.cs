using System.Collections;
using UnityEngine;

public class BurnDebuff : BaseBuff
{
    private MeshParticleView _meshParticle;

    public BurnDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _icon = skill.BuffIcon;
        _damage = skill.BuffPreiodDamage;
        _duration = skill.BuffDuration;
        _damageType = skill.BuffDamageType;
        _particleType = ParticleType.Burn;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        base.OnApply(target);

        if (_particle is MeshParticleView meshParticle)
        {
            meshParticle.SetMesh(Owner.gameObject);
        }
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        base.OnRemove(target);

        if (_particle is MeshParticleView meshParticle)
        {
            meshParticle.SetNeitralMesh();
        }
    }

    protected override void OnTick()
    {
        base.OnTick();

        Owner.TakeDamage(_damage, _damageType, 100, Color.blue);
    }
}