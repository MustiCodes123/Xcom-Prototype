using DG.Tweening;
using UnityEngine;

public class FrostDebuff : IBuff
{
    public BaseCharacerView Owner { get => owner; set => owner = value; }
    public BuffsEnum buffType { get; set; }

    public int damage = 0;
    public float _duration = 10;

    private BaseCharacerView owner;
    private BaseParticleView _particle;
    private BaseParticleView.Factory _particleFactory;
    private Sprite _icon;
    private IBehaviourState _stunState;
    private IBehaviourState _originState;

    public FrostDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        buffType = BuffsEnum.FrostBuff;
    }

    public void Apply(BaseCharacerView target)
    {
        if (target.IsDead)
        {
            return;
        }

        Owner = target;
        _stunState = new StunState(target);
        _originState = target.CurrentState;
        Owner.SetState(_stunState);
        Owner.transform.DOMove(Owner.transform.position, _duration);
        Owner.Animator.enabled = false;
        _particle = _particleFactory.Create(ParticleType.Freeze);
        _particle.SetParent(target.transform);
        _particle.duration = _duration;
        owner.HealthBar.SetBuffOnBar(_icon);
    }

    public void Remove(BaseCharacerView target)
    {
        if (!target.IsDead)
        {
            Owner.Animator.enabled = true;
            Owner.SetState(_originState);
            owner.HealthBar.RemoveBuufOnBar(_icon);
        }
        else
        {
            var deathState = new DeathState(target);
            
            Owner.Animator.enabled = true;
            Owner.SetState(deathState);
            owner.HealthBar.RemoveBuufOnBar(_icon);
        }
        _particle.SetParent(null);
    }

    public void Tick()
    {
        _duration--;

        if (Owner == null)
        {
            return;
        }

        if (_duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }

        if (Owner.IsDead)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }
        Owner.SetAnimation(AnimStates.Idle);
      
    }
}