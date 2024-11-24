using System.Collections;
using UnityEngine;

public class BaseBuff : IBuff
{
    public BuffsEnum buffType { get; set; }
    public BaseCharacerView Owner { get => _owner; set => _owner = value; }
    protected BaseCharacerView _owner;
    protected BaseSkillModel _skill;
    protected BaseParticleView _particle;
    protected BaseParticleView.Factory _particleFactory;
    protected Sprite _icon;

    protected int _damage;
    protected AttackType _damageType;
    protected float _duration;
    protected ParticleType _particleType;

    public void Apply(BaseCharacerView target)
    {
        OnApply(target);
    }

    public virtual void Remove(BaseCharacerView target)
    {
        OnRemove(target);
    }

    public virtual void Tick()
    {
        OnTick();
    }

    protected virtual void OnApply(BaseCharacerView target)
    {
        Owner = target;

        _particle = _particleFactory.Create(_particleType);
        _particle.SetParent(target.transform);

        _owner.HealthBar.SetBuffOnBar(_icon);
    }

    protected virtual void OnRemove(BaseCharacerView target)
    {
        _owner.HealthBar.RemoveBuufOnBar(_icon);
        _particle.OnDespawned();
    }

    protected virtual void OnTick()
    {
        _duration--;
        if (_duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }
    }
}