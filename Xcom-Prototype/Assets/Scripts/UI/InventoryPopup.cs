using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryPopup : MonoBehaviour
{
    [SerializeField] private Transform inventoryConteiner;
    [SerializeField] private Button closeButton;

    private List<BaseItemSlot> itemSlots = new List<BaseItemSlot>();

    [Inject] private BaseItemSlot itemPrefab;
    [Inject] private PlayerData playerData;



    public void ShoiwPossibleItemsForCharacter(BaseCharacterModel model, SlotEnum slotEnum)
    {
        int itemsCreated = 0;
        for (int i = 0; i < playerData.PlayerInventory.Count; i++)
        {
            var item = playerData.PlayerInventory[i];

            if(item.Slot == slotEnum)
            {
                if(itemSlots.Count > itemsCreated)
                {
                    itemSlots[itemsCreated].SetItem(item);
                }
                else
                {
                    BaseItemSlot itemSlot = Instantiate(itemPrefab, inventoryConteiner);
                    itemSlot.SetItem(item);
                    itemSlots.Add(itemSlot);
                }
                itemsCreated++;
            }
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

}
