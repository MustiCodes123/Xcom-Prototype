using System;
using UnityEngine;

[Serializable]
public class RingUpgradeStats
{
    [field: SerializeField] public float MagicResistance { get; private set; }
    [field: SerializeField] public float ManaBuff { get; private set; }
    [field: SerializeField] public float MagicDamageBuff { get; private set; }
    [field: SerializeField] public float ManaRegenerationRateBuff { get; private set; }
    [field: SerializeField] public float ManaCostReduction { get; private set; }
}
