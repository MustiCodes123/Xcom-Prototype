using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class BaseWeaponInfo
{
    public string Name;
    public int Rank;
    public int Cost;
   

    public int Luck;
    public int Skill;
    public int Attack;
    public int Defense;
    public int Magic;
    public int MagicDefense;

    public BaseWeaponInfo()
    {
        Name = "Weapon";
        Cost = 0;
        Luck = 0;
        Skill = 0;
        Attack = 0;
        Defense = 0;
        Magic = 0;
        MagicDefense = 0;
    }

    public BaseWeaponInfo(BaseWeaponInfo weapon)
    {
        Name = weapon.Name;
        Cost = weapon.Cost;
        Luck = weapon.Luck;
        Skill = weapon.Skill;
        Attack = weapon.Attack;
        Defense = weapon.Defense;
        Magic = weapon.Magic;
        MagicDefense = weapon.MagicDefense;
    }

  
}
