using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ElectricDebuff : BaseBuff, IBuff
{
    public float slowSpeed = 1f;
    public float duration = 10;
    public float damage = 0;
    public float shieldAppearDelay = 9.2f;

    private float _originnSpeed;

    private NavMeshAgent _navMesh;

    public ElectricDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _duration = skill.BuffDuration;
        _particleFactory = particleFactory;
        _icon = skill.BuffIcon;
        buffType = BuffsEnum.ElectricDebuff;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _navMesh = target.NavMeshAgent;
        _originnSpeed = target.characterData.MoveSpeed;
        _navMesh.speed = slowSpeed;
        _particle = _particleFactory.Create(ParticleType.Electric);
        _particle.SetParent(target.transform);
        _particle.duration = duration;
        _owner.HealthBar.SetBuffOnBar(_icon);
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        _navMesh.speed = _originnSpeed;
        _particle.OnDespawned();
        _owner.HealthBar.RemoveBuufOnBar(_icon);
    }
}