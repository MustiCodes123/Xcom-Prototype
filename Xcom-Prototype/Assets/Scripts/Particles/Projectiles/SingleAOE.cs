using UnityEngine;
using System;

public class SingleAOE : BaseProjectile
{
    public BuffsEnum Buff;

    private Vector3 _target;

    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff = 0)
    {
        base.Setup(target, caster, particle, skill, Onhit, decale);

        if (caster.AutoBattle || caster.IsBot)
        {
            _target = target.transform.position;
        }
        else
        {
            _target = Decale.Target.position;
        }

        Buff = buff;
    }

    public override void SkillUpdate()
    {
        transform.position = _target;
    }

    public override void OnSpawnAction()
    {
        
    }

    public void OnEnter()
    {
        OnHit.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();

        if (damageble != null && damageble is BaseCharacerView target && !Creator.IsMyTeammate(target) && !target.SkillServiceProvider.IsBuffOnMe(Buff))
        {
            TargetView = target;
            OnEnter();
        }
    }
}