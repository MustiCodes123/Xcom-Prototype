using System.Collections;
using UnityEngine;
using System;

public class Traps : BaseProjectile
{
    public bool IsActivated;

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

        IsActivated = false;
        transform.LookAt(_lookAt);
    }

    public override void SkillUpdate()
    {

    }

    public override void OnDamage()
    {
        IsActivated = true;
        base.OnDamage();
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
}