using System.Collections;
using UnityEngine;

public class HunterDebuff : BaseBuff
{
    private int _critChanseIncrease;
    private int _originBlockChanse;
    public HunterDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _skill = skill;
        _particleFactory = particleFactory;
        _duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        _critChanseIncrease = skill.Value;
        buffType = BuffsEnum.HunterDebuff;
    }
    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _originBlockChanse = Owner.characterData.BlockChance;
        Owner.characterData.BlockChance -= _skill.Value;
        _particle = _particleFactory.Create(_skill.OnCollisionParticle);
        _particle.SetParent(target.transform);
        _particle.duration = _duration;
        _owner.HealthBar.SetBigBuff(_icon);
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        Owner.characterData.BlockChance = _originBlockChanse;
        _owner.HealthBar.RemoveBigBuff();
        _particle.OnDespawned();
    }
}