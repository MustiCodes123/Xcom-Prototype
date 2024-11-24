using System.Collections;
using UnityEngine;
using System;

public class Zone : BaseProjectile
{
    private BuffsEnum Buff;

    private Vector3 _casterPosition;

    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff = 0)
    {
        base.Setup(target, caster, particle, skill, Onhit, decale);
        _casterPosition = caster.transform.position;
        Buff = skill.OnCollisionBuff;
        transform.position = _casterPosition;
        transform.SetParent(caster.transform);
        ParticleView = particle;
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