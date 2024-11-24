using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBuff : IBuff
{
    public BaseCharacerView Owner { get => owner; set => owner = value; }
    public BuffsEnum buffType { get; set; }

    private BaseCharacerView owner;

    public int DamageMultiplayer;
    public int Duration;

    public void Apply(BaseCharacerView target)
    {
        Owner = target;

        Owner.characterData.AdditionalaAttack += DamageMultiplayer;

    }

    public void Remove(BaseCharacerView target)
    {
        Owner.characterData.AdditionalaAttack -= DamageMultiplayer;
    }

    public void Tick()
    {

        Duration--;
        if(Duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }

    }
}
