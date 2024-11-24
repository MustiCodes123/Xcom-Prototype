using UnityEngine;
using UnityEngine.AI;

public class MovementAndArmorDebuff : BaseBuff, IBuff
{
    public BaseCharacerView Owner { get; set; }
    public BuffsEnum buffType { get; set; }
    
    public float Duration = 10;
    public float SlowSpeed = 1f;

    private BaseParticleView _particle;
    private BaseParticleView.Factory _particleFactory;
    private Sprite _icon;

    private float _armorReduction;
 
    private float _originnSpeed;
    private NavMeshAgent _navMesh;

    public MovementAndArmorDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        Duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        _armorReduction = skill.Value;
        buffType = BuffsEnum.MovementAndArmorDebuff;
    }

    protected override void OnApply(BaseCharacerView target)
    {
            Owner = target;
            _particle = _particleFactory.Create(ParticleType.ArmorDebuff);
            _particle.SetParent(target.transform);
            _particle.duration = Duration;
            SlowDownTarget(target);
            target.HealthBar.SetBuffOnBar(_icon);
            
            target.characterData.AddResistance(0, 0, -_armorReduction);
    }

    protected override void OnRemove(BaseCharacerView target)
    {

        target.HealthBar.RemoveBuufOnBar(_icon);
        _particle.SetParent(null);

        target.characterData.AddResistance(0, 0, _armorReduction);
        _navMesh.speed = _originnSpeed;
        Owner.HealthBar.RemoveBuufOnBar(_icon);

    }

    private void SlowDownTarget(BaseCharacerView target)
    {
        Owner = target;
        _navMesh = target.NavMeshAgent;
        _originnSpeed = target.characterData.MoveSpeed;
        _navMesh.speed = SlowSpeed;
    }
    public void Tick()
    {
        Duration--;
        if (Duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }
    }
}