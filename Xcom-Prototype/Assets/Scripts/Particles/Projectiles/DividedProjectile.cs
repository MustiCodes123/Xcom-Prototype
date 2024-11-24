using System;
using UnityEngine;

public class DividedProjectile : BaseProjectile
{
    public BaseParticleView Trail;

    protected Vector3 startOffset;
    protected Vector3 _target;

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
    }

    public virtual void UpdateTrail(BaseParticleView trail)
    {
        trail.transform.LookAt(_target);
    }

    public override void OnDamage()
    {
        OnHit.Invoke();
    }
}