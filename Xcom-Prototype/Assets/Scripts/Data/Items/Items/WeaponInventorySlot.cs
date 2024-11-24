using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponInventorySlot : InventoryItemSlot
{
    [SerializeField] private OffHandInventorySlot OffHand;

    public override void SetItem(BaseItem item, bool needCreateNew = true, Action<BaseItem, BaseItemSlot> action = null)
    {
        if (item is WeaponItem weapon)
        {
            weapon.IsEquipedInOffHand = false;
            if (!IsTwoHandedWeapon(weapon)) RelieveTwoHand();
        }
        if (item == null) RelieveTwoHand();
        base.SetItem(item, needCreateNew, action);
    }

    public override void AfterDrop(BaseItemSlot Slotfrom, BaseItemSlot SlotTo)
    {
        base.AfterDrop(Slotfrom, SlotTo);
        DisableSlotWithTwoHanded();
    }

    public override void Reset()
    {
        base.Reset();
        RelieveTwoHand();
    }

    public override bool IsSuitableSlot(BaseDragableItem dregableItem)
    {
        var weapon = dregableItem.Item as WeaponItem;
        if (slotType == dregableItem.Item.Slot)
        {
            if (IsTwoHandedWeapon(weapon)) OffHand.UnequipOffHand();
            return true;
        }
        return false;
    }

    public bool IsTwoHandedWeapon(WeaponItem weapon)
    {
        return weapon.weaponType == WeaponTypeEnum.TwoHandedAxe
            || weapon.weaponType == WeaponTypeEnum.TwoHandedSword
            || weapon.weaponType == WeaponTypeEnum.TwoHandedMace
            || weapon.weaponType == WeaponTypeEnum.Bow
            || weapon.weaponType == WeaponTypeEnum.Crossbow
            || weapon.weaponType == WeaponTypeEnum.Spear
            || weapon.weaponType == WeaponTypeEnum.Staff
            || weapon.weaponType == WeaponTypeEnum.Wand;
    }

    public void RelieveTwoHand()
    {
        if (OffHand != null)
        {
            OffHand.IsDragable = true;
            OffHand.Unlock();
            OffHand.enabled = true;
        }
    }

    public void DisableSlotWithTwoHanded()
    {
        if (Item != null && Item is WeaponItem weapon && IsTwoHandedWeapon(weapon))
        {
            OffHand.IsDragable = false;
            OffHand.SetLock();
            OffHand.enabled = false;
        }
    }

    public void CheckShieldAndTwoHanded()
    {
        DisableSlotWithTwoHanded();
    }

    public override bool TwoHandedOnDrop(BaseDragableItem dregableItem)
    {
        if (dregableItem.Item is WeaponItem weapon && IsTwoHandedWeapon(weapon))
        {
            return true;
        }
        else
            return false;
    }

    public OffHandInventorySlot GetOffHand() => OffHand;
}