using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class WaveThroughWithDecale : DividedProjectile
{
    public override void SkillUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position + startOffset, _target, speed * Time.deltaTime);

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
}