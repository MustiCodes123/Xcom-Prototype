using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBuff : IBuff
{
    public BaseCharacerView Owner { get => owner; set => owner = value; }
    public BuffsEnum buffType { get; set; }

    private BaseCharacerView owner;

    public int AdditionalHP;
    public int Duration;


    public void Apply(BaseCharacerView target)
    {
        Debug.Log("added HP");
        Owner = target;
        Owner.characterData.AddAdditionlHP(AdditionalHP);
        Owner.UpdateHPBars();
        Debug.Log("added HP " + Owner.characterData.Health);
    }

    public void Remove(BaseCharacerView target)
    {
        target.characterData.AddAdditionlHP(-AdditionalHP);
        Owner.UpdateHPBars();
        Debug.Log("removed HP " + Owner.characterData.Health);
    }

    public void Tick()
    {
        Duration--;
        if(Duration<= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }
    }
}
