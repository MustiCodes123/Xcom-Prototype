using System;
using System.Collections;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class CharacterSlotsHolder : MonoBehaviour
{
    private const int MinimalSlotsCount = 4;

    public ItemSlot[] ItemSlots => _itemSlots;
   
    [SerializeField] private ItemSlot[] _itemSlots;

    public void SetupItems(List<BaseItem> itemsTosetup, ResourceManager resourceManager)
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].Setup(resourceManager);
            _itemSlots[i].HideItemSlot();
        }

        for (int i = 0; i < itemsTosetup.Count; i++)
        {
            if (itemsTosetup[i] is WeaponItem item)
            {
                if (item.weaponType == WeaponTypeEnum.Bow)
                {
                    GetSlot(SlotEnum.Bow).EquipItem(itemsTosetup[i].itemName);
                }
                else if(item.weaponType == WeaponTypeEnum.Shield)
                {
                    GetSlot(SlotEnum.Shield).EquipItem(itemsTosetup[i].itemName);
                }
                else if (item.IsEquipedInOffHand)
                {
                    GetSlot(SlotEnum.OffHand).EquipItem(itemsTosetup[i].itemName);                        
                }                    
                else
                {
                    GetSlot(SlotEnum.Weapon).EquipItem(itemsTosetup[i].itemName);
                }
                    
            }
        }
    }

    [ContextMenu("CreateItemsSlots")]
    public void CreateItemSlots()
    {
        for (int i = 0; i < MinimalSlotsCount; i++)
        {
            var slotObject = new GameObject("ItemSlot " + i.ToString());
            var slot = slotObject.AddComponent<ItemSlot>();
            slotObject.transform.parent = gameObject.transform;
        }
    }

    public ItemSlot GetSlot(SlotEnum slotEnum)
    {
        foreach (var slot in _itemSlots)
        {
            if (slot.SlotType == slotEnum)
                return slot;
        }
        Debug.Log("Character not contains " + slotEnum);
        return null;
    }
}
