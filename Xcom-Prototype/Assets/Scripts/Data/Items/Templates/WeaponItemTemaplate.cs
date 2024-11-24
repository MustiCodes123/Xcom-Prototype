using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Inventory/WeaponItem")]
public class WeaponItemTemaplate : ItemTemplate
{
    public WeaponTypeEnum weaponType;
    public WeaponUpgradeStats[] WeaponUpgradeStats;

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
