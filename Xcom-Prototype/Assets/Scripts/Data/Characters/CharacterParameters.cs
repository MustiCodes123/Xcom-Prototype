using System;

[Serializable]
public struct CharacterParameters
{
    public int Health;
    public int Mana;

    public float MoveSpeed;
    public float LiftingCapacity;

    public int Attack;
    public float AdditionalAttackSpeed;
    public float CriticalDamage;
    public float CriticalChance;
    public float MagicCriticalDamage;
    public float MagicCriticalChance;
    public int Magic;
    public int MagicDefense;

    public int BlockChance;
    public float DodgeAdditional;
    public float MagicalResistance;
    public float PhysicalResistance;
}