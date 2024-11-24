
public class UpgradeItemGoal : QuestGoal
{
    public bool IsAmountByLvl;
    public SlotEnum ItemType;
    public WeaponTypeEnum WeaponType;

    public override void Process(ISignal signal)
    {
        if (signal is UpgradeSignal)
        {
            UpgradeSignal upgradeItemSignal = (UpgradeSignal)signal;
            if (!upgradeItemSignal.IsCharacterUpgrade && CurrentAmount < upgradeItemSignal.Item.CurrentLevel + 1 && IsAmountByLvl)
            {
                if (upgradeItemSignal.Item is WeaponItem weapon && weapon.weaponType == WeaponType)
                {
                    CurrentAmount = upgradeItemSignal.Item.CurrentLevel + 1;
                }
                else if (upgradeItemSignal.Item.Slot == ItemType)
                { 
                    CurrentAmount = upgradeItemSignal.Item.CurrentLevel + 1;
                }
            }

            if (!upgradeItemSignal.IsCharacterUpgrade && !IsAmountByLvl)
            {
                if (upgradeItemSignal.Item is WeaponItem weapon && weapon.weaponType == WeaponType)
                {
                    CurrentAmount++;
                }
                else if (upgradeItemSignal.Item.Slot == ItemType)
                {
                    CurrentAmount++;
                }
                else if (ItemType == SlotEnum.Any)
                {
                    CurrentAmount++;
                }
            }
        }
    }
}