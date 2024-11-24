using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorThroughWithDecale: BaseProjectile
{
    public BaseParticleView trail;
    private Vector3 startOffset;
    private Vector3 _target;
    
    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff = 0)
    {
        Duration = skill.Duration;
        base.Setup(target, caster, particle, skill, Onhit, decale);
        if (caster.AutoBattle || caster.IsBot)
        {
            _target = target.transform.position;
        }
        else
        {
            _target = Decale.Target.position;
        }
    }
    public override void SkillUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position + startOffset, _target, speed * Time.deltaTime);

        if (trail != null)
        {
            UpdateTrail(trail);
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

    public void UpdateTrail(BaseParticleView trail)
    {
        trail.transform.LookAt(_target);
    }

    public override void OnDamage()
    {
        OnHit.Invoke();
    }
}
