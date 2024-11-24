using System.Collections;
using UnityEngine;
using System;

public class ArmorDebuff : IBuff
{
    public BaseCharacerView Owner { get => owner; set => owner = value; }
    public BuffsEnum buffType { get; set; }

    public int damage = 0;
    public float _duration = 10;

    private BaseCharacerView owner;
    private BaseParticleView _particle;
    private BaseParticleView.Factory _particleFactory;
    private Sprite _icon;

    private float _resistanceReduse;


    public ArmorDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        _resistanceReduse = skill.Value;
        buffType = BuffsEnum.ArmorDebuff;
    }

    public void Apply(BaseCharacerView target)
    {
        Owner = target;
        _particle = _particleFactory.Create(ParticleType.ArmorDebuff);
        _particle.SetParent(target.transform);
        _particle.duration = _duration;
        owner.HealthBar.SetBuffOnBar(_icon);
        owner.characterData.AddResistance(0, 0, -_resistanceReduse);
    }

    public void Remove(BaseCharacerView target)
    {
        owner.HealthBar.RemoveBuufOnBar(_icon);
        _particle.OnDespawned();
        owner.characterData.AddResistance(0, 0, _resistanceReduse);
    }

    public void Tick()
    {
        _duration--;
        if (_duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }
    }
}