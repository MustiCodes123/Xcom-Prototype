using UnityEngine;

public enum ArmorType
{
    ChestArmor,
    Gloves,
    Helmet,
    Legs,
    Shield
};

[CreateAssetMenu(fileName = "New Armor Item", menuName = "Inventory/ArmorItem")]
public class ArmorItemTemplate : ItemTemplate
{
    public ArmorType ArmorType;
    public ArmorUpgradeStats[] ArmorUpgradeStats;

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
