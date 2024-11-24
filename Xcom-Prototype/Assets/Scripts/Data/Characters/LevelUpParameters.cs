using System;
using System.Collections.Generic;

[Serializable]
public struct LevelUpParameters 
{
    public int XpToNextLevel;

    public int HealthLevelUpIncrease;
    public int ManaLevelUpIncrease;
    public int AttackLevelUpIncrease;
    public int MagicLevelUpIncrease;

    public float LiftingCapacityLevelUpIncrease;
    public float PhysicalResistanceLevelUpIncrease;
    public float MagicalResistanceLevelUpIncrease;
    public float CriticalPhysicalDamageLevelUpIncrease;
    public float CriticalMagicalDamageLevelUpIncrease;
    public float DodgeAdditionalLevelUpIncrease;
    public float MoveSpeedLevelUpIncrease;

    public Dictionary<RareEnum, int> RareToLevelUpCoef;
}