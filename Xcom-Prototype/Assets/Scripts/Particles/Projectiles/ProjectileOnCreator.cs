using System;
using UnityEngine;

public class ProjectileOnCreator : DividedProjectile
{
    private float _casterSpeed;
    
    private float _originalSpeed;

    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill,
        Action Onhit, BaseDecale decale, BuffsEnum buff = BuffsEnum.BurnBuff)
    {
        base.Setup(target, caster, particle, skill, Onhit, decale, buff);
        _casterSpeed = skill.DamageRange;
        _originalSpeed = Creator.NavMeshAgent.speed;
        Creator.NavMeshAgent.speed = _casterSpeed;
    }
    
    public override void SkillUpdate()
    {
        if (Trail != null)
        {
            UpdateTrail(Trail);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();

        if (damageble != null && damageble is BaseCharacerView target && !Creator.IsMyTeammate(target))
        {
            TargetView = target;
            OnDamage();
        }
    }

    public override void OnDamage()
    {
        OnHit.Invoke();
    }

    public override void OnDespawned()
    {
        Creator.NavMeshAgent.speed = _originalSpeed;
        base.OnDespawned();
    }
}