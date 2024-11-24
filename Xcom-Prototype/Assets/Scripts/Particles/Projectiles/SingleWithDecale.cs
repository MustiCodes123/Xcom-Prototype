using System;
using UnityEngine;

public class SingleWithDecale : BaseProjectile
{
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
    }
    public override void SkillUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);
        transform.LookAt(_target);
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();

        if (damageble != null && damageble is BaseCharacerView targetCharacter)
        {
            if (!Creator.IsMyTeammate(targetCharacter))
            {
                Enemy = damageble as BaseCharacerView;
                TargetView = Enemy;
                OnDamage();
            }
        }
    }
}
