using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class StoneSkinBuff : IBuff
{
    public BaseCharacerView Owner { get => owner; set => owner = value; }
    public BuffsEnum buffType { get; set; }


    public float duration = 10;
    public float shieldAppearDelay = 9.2f;

    private float stoneSkinAnimationSpeed = 2;
    private int armorToIncrease ;

    private BaseCharacerView owner;
    private BaseParticleView.Factory _particleFactory;
    private Sprite _icon;
    private Color _originColor;
    private Material[] _characterMatrials;

    public StoneSkinBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        armorToIncrease = skill.Value;
        _icon = skill.BuffIcon;
    }

    public void Apply(BaseCharacerView target)
    {
        Owner = target;
        _characterMatrials = target.Materials;
        _originColor = _characterMatrials[0].color;
        foreach (Material mat in _characterMatrials)
        {
            if (mat.HasColor("_Color"))
            {
                mat.DOColor(Color.gray, stoneSkinAnimationSpeed);
            }    
               
        }

        owner.characterData.AddResistance(0, 0, armorToIncrease);
        owner.HealthBar.SetBuffOnBar(_icon);
    }

    public void Remove(BaseCharacerView target)
    {
        foreach (Material mat in _characterMatrials)
        {
            if (mat.HasColor("_Color"))
            {
                mat.DOColor(_originColor, stoneSkinAnimationSpeed);
            }
        }

        target.HealthBar.UIShieldIcon.gameObject.SetActive(false);
        owner.characterData.AddResistance(0,0, -armorToIncrease);
        owner.HealthBar.RemoveBuufOnBar(_icon);
    }

    public void Tick()
    {
        duration--;
        if (duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }
    }
}