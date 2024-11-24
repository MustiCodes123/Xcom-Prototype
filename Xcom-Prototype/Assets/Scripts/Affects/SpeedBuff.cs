using UnityEngine.AI;
using UnityEngine;

public class SpeedBuff : BaseBuff, IBuff
{
    private NavMeshAgent _navMesh;

    private float _originnSpeed;

    public SpeedBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _particleType = skill.ParticleType;
        _duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        buffType = BuffsEnum.SpeedBuff;
        _skill = skill;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _navMesh = target.NavMeshAgent;
        _originnSpeed = target.characterData.MoveSpeed;
        _navMesh.speed = _skill.Value;

        _particle = _particleFactory.Create(_particleType);

        if (_particle is MeshParticleView meshParticle)
        {
            meshParticle.SetMesh(Owner.gameObject);
        }

        Owner.HealthBar.SetBuffOnBar(_icon);
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        _navMesh.speed = _originnSpeed;
        _particle.OnDespawned();

        if (_particle is MeshParticleView meshParticle)
        {
            meshParticle.SetNeitralMesh();
        }

        Owner.HealthBar.RemoveBuufOnBar(_icon);
    }
}