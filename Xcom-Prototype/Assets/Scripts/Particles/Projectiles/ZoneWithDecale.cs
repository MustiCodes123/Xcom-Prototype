using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class ZoneWithDecale : BaseProjectile
{
    public BuffsEnum Buff;

    private Vector3 _target;
    private Vector3 _lookAt;

    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff)
    {
        base.Setup(target, caster, particle, skill, Onhit, decale);

        if (caster.AutoBattle || caster.IsBot)
        {
            _target = target.transform.position;
            _lookAt = target.transform.position;
        }
        else
        {
            _target = Decale.Target.position;
            _lookAt = Decale.LookAt.position;
        }
        Buff = skill.OnCollisionBuff;

        transform.position = _target;
        transform.LookAt(_lookAt);
    }

    public override void SkillUpdate()
    {

    }

    public void OnEnter()
    {
        OnHit.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();

        if (damageble != null && damageble is BaseCharacerView target && !Creator.IsMyTeammate(target) && !target.SkillServiceProvider.IsBuffOnMe(Buff))
        {
            TargetView = target;
            OnEnter();
        }
    }
}