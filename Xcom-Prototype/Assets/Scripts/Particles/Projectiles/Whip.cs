using System;
using UnityEngine;

public class Whip : DividedProjectile
{
    [SerializeField] private SphereCollider _collider;

    private WhipAnimationBehaviour _whipAnimation;

    public override void Setup(BaseCharacerView target, BaseCharacerView caster, BaseParticleView particle, BaseSkillModel skill, Action Onhit, BaseDecale decale, BuffsEnum buff = 0)
    {
        base.Setup(target, caster, particle, skill, Onhit, decale);

        _whipAnimation = ParticleView.GetComponent<WhipAnimationBehaviour>();
    }
    public override void SkillUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);

        if (_whipAnimation != null && TargetView != null)
        {
            _whipAnimation.SecureOnTarget(TargetView.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();
        if (damageble != null && damageble is BaseCharacerView target && !Creator.IsMyTeammate(target))
        {
            ParticleView.gameObject.SetActive(true);
            TargetView = target;
            OnDamage();
            _collider.enabled = false;
        }
    }

    public override void UpdateTrail(BaseParticleView trail)
    {
        transform.LookAt(_target);
    }

    public override void OnDespawnedAction()
    {
        _collider.enabled = true;
        gameObject.transform.SetParent(null);
        gameObject.SetActive(false);
        _whipAnimation.ActivateAnimatedWhip();
        ParticleView.OnDespawned();
    }
}