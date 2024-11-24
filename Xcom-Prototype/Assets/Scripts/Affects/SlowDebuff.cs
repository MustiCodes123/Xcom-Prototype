using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class SlowDebuff : BaseBuff, IBuff
{
    public bool inTrigger = false;
    public float slowSpeed = 1f;

    private NavMeshAgent _navMesh;
    private Color _originColor;
    private Material[] _characterMatrials;

    private float _originnSpeed;
    private float blueSkinAnimationSpeed = 1;

    
    public SlowDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        buffType = BuffsEnum.SlowDebuff;
        _damage = skill.BuffPreiodDamage;
        _skill = skill;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _navMesh = target.NavMeshAgent;
        _originnSpeed = target.characterData.MoveSpeed;
        _navMesh.speed = slowSpeed;
        _particle = _particleFactory.Create(ParticleType.MagneticCircles);
        _particle.SetParent(target.transform);
        Owner.HealthBar.SetBuffOnBar(_icon);
    }
    

    protected override void OnRemove(BaseCharacerView target)
    {
        _navMesh.speed = _originnSpeed;
        Owner.HealthBar.RemoveBuufOnBar(_icon);

        _particle.OnDespawned();
    }

    public void ChangeMaterial(Color color, BaseCharacerView target)
    {
        _characterMatrials = target.Materials;
        _originColor = _characterMatrials[0].color;

        foreach (Material mat in _characterMatrials)
        {
            mat.DOColor(color, blueSkinAnimationSpeed);
        }
    }
}

