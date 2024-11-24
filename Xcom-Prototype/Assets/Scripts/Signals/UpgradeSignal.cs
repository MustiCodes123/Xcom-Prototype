
public struct UpgradeSignal : ISignal
{
    public bool IsCharacterUpgrade;

    public PlayerData PlayerData;

    public BaseCharacterModel Hero;

    public BaseItem Item;
}