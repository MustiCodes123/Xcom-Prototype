using System;
using UnityEngine;

[Serializable]
public class ArmletUpgradeStats
{
    [field: SerializeField] public float HealthBuff { get; private set; } 
    [field: SerializeField] public float DamageBuff { get; private set; } 
    [field: SerializeField] public float CritDamageBuff { get; private set; }
    [field: SerializeField] public float AttackDamageBuff { get; private set; }
    [field: SerializeField] public float ArmorBuff { get; private set; } 
    [field: SerializeField] public float ItemWeight { get; private set; }
}
