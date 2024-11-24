using UnityEngine;

[CreateAssetMenu(fileName = "New Ring Item", menuName = "Inventory/RingItem")]
public class RingItemTemplate : ItemTemplate
{
    public RingUpgradeStats[] RingUpgradeStats;

    public override void ResetStats()
    {
        ItemStats stats = new ItemStats
        {
            Rare = this.Rare,
            SetEnum = this.SetEnum,
            itemName = this.itemName,
            itemID = this.itemID,
            itemDescription = this.itemDescription,
            itemSprite = this.itemSprite,
            itemMaxStack = this.itemMaxStack,
            isConsumable = this.isConsumable,
            Slot = this.Slot,
            itemPrice = this.itemPrice,
            skillsDataInfo = this.skillsDataInfo,
            ItemSkillSets = this.ItemSkillSets,
            ItemsSet = this.ItemsSet
        };

        this.ItemStats = stats;
    }
}
