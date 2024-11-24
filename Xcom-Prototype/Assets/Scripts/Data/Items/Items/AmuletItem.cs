public class AmuletItem : BaseItem
{
    public float CriticalChance { get; private set; }
    public float MovementSpeedBuff { get; private set; }
    public float AttackSpeedBuff { get; private set; }
    public float AttackAccuracy {  get; private set; }    
    public float DodgeChance { get; private set; }

    public void SetupLevel(AmuletUpgradeStats stats)
    {
        this.CriticalChance = stats.CriticalChance;
        this.MovementSpeedBuff = stats.MovementSpeedBuff;
        this.AttackSpeedBuff = stats.AttackSpeedBuff;
        this.AttackAccuracy = stats.AttackAccuracy;
        this.DodgeChance = stats.DodgeChance;
    }
}
