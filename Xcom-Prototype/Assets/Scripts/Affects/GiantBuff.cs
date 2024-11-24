using System.Collections;
using UnityEngine;
using DG.Tweening;

public class GiantBuff : IBuff
{
    public BaseCharacerView Owner { get => owner; set => owner = value; }
    public BuffsEnum buffType { get; set; }


    public int damage = 0;
    public float duration = 10;

    private float giantAnimationSpeed = 1;
    private int statsIncrease = 10;

    private BaseCharacerView owner;
    private BaseParticleView.Factory _particleFactory;
    private Vector3 skilloOdificator = new Vector3(2, 2, 2);
    private Vector3 _originScale;
    private Sprite _icon;

    public GiantBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _icon = skill.BuffIcon;
    }

    public void Apply(BaseCharacerView target)
    {
        Owner = target;
        _originScale = Owner.transform.localScale;
        Owner.transform.DOScale(skilloOdificator, giantAnimationSpeed);

        target.characterData.IncreaseAllStats(statsIncrease);
        owner.HealthBar.SetBuffOnBar(_icon);
    }

    public void Remove(BaseCharacerView target)
    {
        Owner.transform.DOScale(_originScale, giantAnimationSpeed);
        target.characterData.IncreaseAllStats(-statsIncrease);
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