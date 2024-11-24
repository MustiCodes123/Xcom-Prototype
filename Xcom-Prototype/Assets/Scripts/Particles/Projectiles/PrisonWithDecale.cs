using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

public class PrisonWithDecale : BaseProjectile
{
    private List<BaseCharacerView> stoppedTargets = new List<BaseCharacerView>();

    private BuffsEnum Buff;
    private Vector3 _target;
    private Vector3 _lookAt;

    private float _originSpeed;

    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff)
    {
        base.Setup(target, caster, particle, skill, Onhit, decale);

        if (caster.AutoBattle && caster.IsBot)
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

    public override void OnDespawnedAction()
    {
        foreach (BaseCharacerView target in stoppedTargets)
        {
            target.NavMeshAgent.speed = _originSpeed;
        }
        
        stoppedTargets.Clear();
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

    private void OnTriggerExit(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();

        if (damageble != null && damageble is BaseCharacerView target && !Creator.IsMyTeammate(target) && !target.SkillServiceProvider.IsBuffOnMe(BuffsEnum.StunBuff))
        {
            stoppedTargets.Add(target);
            TargetView = target;
            _originSpeed = target.NavMeshAgent.speed;
            target.NavMeshAgent.speed = 0;
        }
    }
}