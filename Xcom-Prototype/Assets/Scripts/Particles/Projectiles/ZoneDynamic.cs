using System.Collections;
using System;
using UnityEngine;
using DG.Tweening;

public class ZoneDynamic : BaseProjectile
{
    [SerializeField] protected float _extenssionSpeed = 5;

    [SerializeField] protected Vector3 _startScale;
    [SerializeField] protected Vector3 _finishScale;

    private Vector3 _target;
    
    private BuffsEnum Buff;
    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff)
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

        Buff = skill.OnCollisionBuff;
    }

    public override void OnSpawnAction()
    {
        transform.localScale = _startScale;
        transform.DOScale(_finishScale, _extenssionSpeed);
    }

    public override void SkillUpdate()
    {
        transform.position = _target;
        ParticleView.transform.position = _target;
    }
    public override void OnDespawnedAction()
    {
        transform.localScale = _startScale;
        gameObject.SetActive(false);
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