using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDebuff : IBuff
{
    public BaseCharacerView Owner { get => owner; set => owner = value; }
    public BuffsEnum buffType { get; set; }

    private BaseCharacerView owner;
    public int damage = 0;
    public float duration = 0;

    public void Apply(BaseCharacerView target)
    {
        Owner = target;
        Debug.Log("POison Apply");
    }

    public void Remove(BaseCharacerView target)
    {
        Debug.Log("POison Remove");
    }

    public void Tick()
    {
        Debug.Log("POison Tick" + damage);
        duration--;
        if (duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }    
          
        Owner.TakeDamage(damage, AttackType.Magical, 100, Color.blue);
    }
}
