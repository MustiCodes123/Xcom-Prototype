using System;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;

[Serializable]
public struct ItemStats
{
    public RareEnum Rare;
    public ItemSetEnum SetEnum;
    public string itemName;
    public int itemID;
    public string itemDescription;
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public int itemMaxStack;
    public bool isConsumable;
    public SlotEnum Slot;
    public int itemPrice;
    public List<BaseSkillTemplate> skillsDataInfo;
    public List<ItemSkillSet> ItemSkillSets;
    [HideInInspector] public BaseItemsSet ItemsSet;
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemTemplate : ScriptableObject
{
    public RareEnum Rare;
    public ItemSetEnum SetEnum;
    public string itemName;
    public int itemID;
    public string itemDescription;
    public Sprite itemSprite;
    public int itemMaxStack;        
    public bool isConsumable;
    public SlotEnum Slot;   
    public int itemPrice;
    public List<BaseSkillTemplate> skillsDataInfo;
    public List<ItemSkillSet> ItemSkillSets;
    [HideInInspector] public BaseItemsSet ItemsSet;
    public ResourceManager ResourceManager;
    public ItemStats ItemStats;

    public virtual void ResetStats() {}

    [ContextMenu("AddItem")]
    public void AddItemToInventory()
    {
        var item = ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(this);
        PlayerData playerData = FindObjectOfType<BossWindowView>(true).PlayerData;
        playerData.AddItemToInventory(item);
    }
}

