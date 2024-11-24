using System.Collections;
using UnityEngine;
using System;

public class WallWithDestroy : BaseProjectile
{
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

        Duration = skill.Duration;
        transform.position = _target;
        transform.LookAt(_lookAt);

        StartCoroutine(DespawnAfterDelay(Duration));
    }

    public override void SkillUpdate()
    {

    }

    public override void OnDespawnedAction()
    {
        OnHit.Invoke();
        base.OnDespawnedAction();
    }
}