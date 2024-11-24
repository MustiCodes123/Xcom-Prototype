public class ArmletItem : BaseItem
{
    public float HealthBuff { get; private set; } 
    public float DamageBuff { get; private set; } 
    public float CritDamageBuff { get; private set; }
    public float AttackDamageBuff { get; private set; }
    public float ArmorBuff { get; private set; } 
    public float ItemWeight { get; private set; }

    public void SetupLevel(ArmletUpgradeStats stats)
    {
        this.HealthBuff = stats.HealthBuff;
        this.DamageBuff = stats.DamageBuff;
        this.CritDamageBuff = stats.CritDamageBuff;
        this.AttackDamageBuff = stats.AttackDamageBuff;
        this.ArmorBuff = stats.ArmorBuff;
        this.ItemWeight = stats.ItemWeight;
    }
}
