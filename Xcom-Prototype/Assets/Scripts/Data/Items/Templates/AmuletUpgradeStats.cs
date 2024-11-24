using System;
using UnityEngine;

[Serializable]
public class AmuletUpgradeStats
{
    [field: SerializeField] public float CriticalChance { get; private set; }
    [field: SerializeField] public float MovementSpeedBuff { get; private set; }
    [field: SerializeField] public float AttackSpeedBuff { get; private set; }
    [field: SerializeField] public float AttackAccuracy { get; private set; }
    [field: SerializeField] public float DodgeChance { get; private set; }
}
