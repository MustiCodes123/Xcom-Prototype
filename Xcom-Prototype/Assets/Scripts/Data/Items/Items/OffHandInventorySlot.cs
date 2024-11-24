using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OffHandInventorySlot : InventoryItemSlot
{
    [SerializeField] private WeaponInventorySlot WeaponSlot;
    [SerializeField] private Image _lockImage;

    public override void SetItem(BaseItem item, bool needCreateNew = true, Action<BaseItem, BaseItemSlot> action = null)
    {
        if (item is WeaponItem weapon) weapon.IsEquipedInOffHand = true;
        if ((WeaponSlot.Item != null && Item is WeaponItem mainWeapon && WeaponSlot.IsTwoHandedWeapon(mainWeapon) == false) 
            || WeaponSlot.Item == null) WeaponSlot.RelieveTwoHand();
        base.SetItem(item, needCreateNew, action);
    }

    public override void AfterDrop(BaseItemSlot Slotfrom, BaseItemSlot SlotTo)
    {
        base.AfterDrop(Slotfrom, SlotTo);
    }

    public override void Reset()
    {
        base.Reset();
    }

    public void UnequipOffHand()
    {
        if (Item != null) CharacterHandler.UnequipItem(Item);
    }

    public override bool IsSuitableSlot(BaseDragableItem dregableItem)
    {
        if (slotType == dregableItem.Item.Slot 
            || (dregableItem.Item is WeaponItem weapon && CanUseDualWeapons() && IsSuitableWeapon(weapon))
            || (dregableItem.Item is WeaponItem shield && shield.weaponType == WeaponTypeEnum.Shield))
        {
            TryUnequipTwoHandedWeapon();
            return true;
        }
        else
            return false;
    }

    public void TryUnequipTwoHandedWeapon()
    {
        if (WeaponSlot.Item is WeaponItem weapon)
        {
            if (WeaponSlot.IsTwoHandedWeapon(weapon))
            {
                CharacterHandler.UnequipItem(WeaponSlot.Item);
            }
        }
    }

    private bool CanUseDualWeapons()
    {
        if (WeaponSlot.Item == null || (WeaponSlot.Item != null && IsSuitableWeapon(WeaponSlot.Item as WeaponItem)))
            return characterSelectHandler.GetCurrentCharacterInfo().CharacterTalents.HasDualSkill();
        return false;
    }

    public bool IsSuitableWeapon(WeaponItem weapon)
    {
        return weapon.weaponType == WeaponTypeEnum.Axe
            || weapon.weaponType == WeaponTypeEnum.Dual
            || weapon.weaponType == WeaponTypeEnum.Dagger
            || weapon.weaponType == WeaponTypeEnum.Mace
            || weapon.weaponType == WeaponTypeEnum.Sword;
    }

    public override bool TwoHandedOnDrop(BaseDragableItem dregableItem)
    {
        if (dregableItem.Item is WeaponItem weapon && WeaponSlot.IsTwoHandedWeapon(weapon))
        {
            return true;
        }
        else
            return false;
    }

    public void SetLock()
    {
        _lockImage.gameObject.SetActive(true);
    }

    public void Unlock()
    {
        _lockImage.gameObject.SetActive(false);
    }
}