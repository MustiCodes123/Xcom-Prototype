using UnityEngine;

public class RingItem : BaseItem
{
    public float MagicResistance { get; private set; }
    public float ManaBuff { get; private set; }
    public float MagicDamageBuff { get; private set; }
    public float ManaRegenerationRateBuff { get; private set; }
    public float ManaCostReduction { get; private set; }

    public void SetupLevel(RingUpgradeStats stats)
    {
        this.MagicResistance = stats.MagicResistance;
        this.ManaBuff = stats.ManaBuff;
        this.MagicDamageBuff = stats.MagicDamageBuff;
        this.ManaRegenerationRateBuff = stats.ManaRegenerationRateBuff;
        this.ManaCostReduction = stats.ManaCostReduction;
    }
}
