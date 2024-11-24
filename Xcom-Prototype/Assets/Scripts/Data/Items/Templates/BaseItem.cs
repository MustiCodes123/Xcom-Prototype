using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Data.Resources.AddressableManagement;

[Serializable]
public class BaseItem : IDMData, IStorageItemData
{
    public RareEnum Rare;
    public string itemName;
    public int itemID;
    public ItemSetEnum SetEnum;
    public string itemDescription;

    [JsonIgnore] public Sprite itemSprite;
    [JsonIgnore] public GameObject itemPrefab;

    public int itemCount;
    public int itemValue;
    public int itemMaxStack;
    public bool isConsumable;
    public SlotEnum Slot;
    public int itemPrice;
    [Range(1, 15)] public int CurrentLevel;
    public List<BaseSkillModel> skillModels = new();
    public BaseItemsSet ItemsSet;

    public ItemStats Stats;

    public ResourceManager ResourceManager;

    [OnDeserializing]
    protected void OnDeserialize(StreamingContext context)
    {
        if (ItemsDataInfo.Instance != null)
        {
            var template = ItemsDataInfo.Instance.GetItemTemplate(itemID);
            itemSprite = template.itemSprite;
            Stats = template.ItemStats;
        }
    }

    public void SetStatsFromTemplate<T>(T template, int level) where T : ItemTemplate
    {
        this.itemName = template.itemName;
        this.itemID = template.itemID;
        this.itemDescription = template.itemDescription;
        this.itemSprite = template.itemSprite;
        this.Rare = template.Rare;
        this.itemMaxStack = template.itemMaxStack;
        this.isConsumable = template.isConsumable;
        this.Slot = template.Slot;
        this.CurrentLevel = Mathf.Clamp(level, 0, GameConstants.MaxItemLevel);
        this.itemPrice = template.itemPrice;
        this.SetEnum = template.SetEnum;
        this.ItemsSet = template.ItemsSet;
        this.ResourceManager = template.ResourceManager;
    }

    public async Task<Sprite> GetItemIcon()
    {
        if (ResourceManager != null && itemSprite == null)
        {
            return await ResourceManager.LoadItemSpriteAsync(this.itemID.ToString());
        }
        else
        {
            return itemSprite;
        }
    }
}