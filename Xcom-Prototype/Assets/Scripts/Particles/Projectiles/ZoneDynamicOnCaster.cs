using DG.Tweening;
using System;
using UnityEngine;

public class ZoneDynamicOnCaster : BaseProjectile
{
    [SerializeField] protected Vector3 _startScale;
    [SerializeField] protected Vector3 _finishScale;

    private float _extenssionSpeed;
    private Vector3 _target;
    private BuffsEnum Buff;
    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff)
    {
        base.Setup(target, caster, particle, skill, Onhit, decale);
        _extenssionSpeed = skill.Duration;
        _target = caster.transform.position;
    }

    public override void OnSpawnAction()
    {
        transform.localScale = _startScale;
        transform.DOScale(_finishScale, _extenssionSpeed).SetEase(Ease.Linear);
    }

    public override void SkillUpdate()
    {
        transform.position = Creator.transform.position;
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

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();

        if (damageble != null && damageble is BaseCharacerView target && !Creator.IsMyTeammate(target))
        {
            TargetView = target;
            OnEnter();
        }
    }
}