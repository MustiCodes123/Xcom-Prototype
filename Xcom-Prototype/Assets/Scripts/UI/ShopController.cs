using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Random = UnityEngine.Random;
using System.Linq;

public class ShopController
{
    public List<BaseItem> ShopItems = new List<BaseItem>();
    public List<BaseSkillTemplate> ShopSkills = new List<BaseSkillTemplate>();
    public List<PurchaseItem> PurchaseItems = new List<PurchaseItem>();


    private List<BaseItem> BaseShopItems = new List<BaseItem>();
    private List<BaseItem> RareShopItems = new List<BaseItem>();
    private List<BaseItem> CommonShopItems = new List<BaseItem>();
    private List<BaseItem> UnCommonShopItems = new List<BaseItem>();
    private List<BaseItem> DefaultShopItems = new List<BaseItem>();

    public int UnlockedItems;
    public int startUnlockedItems = 4;

    public DateTime nextRefreshTime;

    private PlayerData playerData;
    private ItemsDataInfo itemsDataInfo;
    private SkillsDataInfo skillsDataInfo;
    private ShopDataInfo shopDataInfo;
    private MoneyChangeHandler moneyChangeHandler; 

    private const int shopItems = 10;
    private const int skillItems = 4;

    

    private string saveTimeString = "ShopItemsTime";
    private string saveString = "ShopItems";
    private string saveUnlockedItemsString = "ShopUnlockedSlots";


    public ShopController( PlayerData playerData, ItemsDataInfo itemsDataInfo, SkillsDataInfo skillsDataInfo, ShopDataInfo shopDataInfo, MoneyChangeHandler moneyChangeHandler)
    {
        this.itemsDataInfo = itemsDataInfo;
        this.playerData = playerData;
        this.skillsDataInfo = skillsDataInfo;
        this.shopDataInfo = shopDataInfo;
        this.moneyChangeHandler = moneyChangeHandler;

        UnlockedItems = PlayerPrefs.GetInt(saveUnlockedItemsString, 0);
        //GenerateItems();
        //CheckTime();

        PurchaseItems = shopDataInfo.PurchaseItems.ToList();
        //ShopSkills = GenerateNesSkillItems();
    }

    public void RemoveItem(BaseItem item)
    {
        if (BaseShopItems.Contains(item))
            BaseShopItems.Remove(item);
    }

    public void RefreshShop(bool needSpendMoney = false)
    {
        if(needSpendMoney)
        {
            //TODO: add money spend
            //if (playerData.Money < 100)
            //    return;
            //playerData.Money -= 100;
        }

        GenerateNewItems();
        nextRefreshTime = DateTime.UtcNow.AddHours(1);
        ShopItems = GenerateNewItems();


        PlayerPrefs.SetString(saveTimeString, nextRefreshTime.ToString());
        PlayerPrefs.SetString(saveString, JsonConvert.SerializeObject(ShopItems));
    }

    public bool UnlockNewSlot()
    {
        UnlockedItems++; 
        PlayerPrefs.SetInt(saveUnlockedItemsString, UnlockedItems);

        return true;

    }

    public void CheckTime()
    {
        if (PlayerPrefs.HasKey(saveTimeString))
        {
            nextRefreshTime = DateTime.Parse(PlayerPrefs.GetString(saveTimeString));
            if (nextRefreshTime < DateTime.UtcNow)
            {
                RefreshShop();
            }
            else
            {
                if (PlayerPrefs.HasKey(saveString))
                {
                    ShopItems = JsonConvert.DeserializeObject<List<BaseItem>>(PlayerPrefs.GetString(saveString));
                    var itemTemplates = new List<ItemTemplate>();
                    for (int i = 0; i < ShopItems.Count; i++)
                    {
                        itemTemplates.Add(itemsDataInfo.GetItemTemplate(ShopItems[i].itemID));
                    }
                    ShopItems.Clear();
                    foreach (var template in itemTemplates)
                    {
                        var item = ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(template);
                        ShopItems.Add(item);
                    }
                }
                else
                {
                    ShopItems = GenerateNewItems();
                }
            }
        }
        else
        {
            RefreshShop();
        }
    }   

    public void GenerateItems()
    {
        ItemsDataInfo.Instance.Weapons.ForEach(item =>
        {
            BaseShopItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
        });
        ItemsDataInfo.Instance.Armors.ForEach(item =>
        {
            BaseShopItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
        });
        ItemsDataInfo.Instance.Items.ForEach(item =>
        {
            BaseShopItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
        });
        ItemsDataInfo.Instance.Rings.ForEach(item =>
        {
            BaseShopItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
        });
        ItemsDataInfo.Instance.Amulets.ForEach(item =>
        {
            BaseShopItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
        });

        CommonShopItems = BaseShopItems.FindAll(item => item.Rare == RareEnum.Common);
        RareShopItems = BaseShopItems.FindAll(item => item.Rare == RareEnum.Rare);
    }


    public List<BaseItem> GenerateNewItems()
    {
        List<BaseItem> baseItems = new List<BaseItem>();

        int rareShance = Random.Range(180, 200);
        int commonShance = Random.Range(140, 180);
        int unCommonShance = Random.Range(90, 140);

        var defaultItems = DefaultShopItems;
        var commonItems = CommonShopItems;
        var unCommonItems = UnCommonShopItems;
        var rareItems = RareShopItems;

        for (int i = 0; i < shopItems; i++)
        {
            int randomItem = Random.Range(0, 200);
            if (randomItem >= rareShance)
            {
                int randomRareItem = Random.Range(0, rareItems.Count);
                baseItems.Add(rareItems[randomRareItem]);
            }
            else if (randomItem >= commonShance)
            {
                int randomCommonItem = Random.Range(0, commonItems.Count);
                baseItems.Add(commonItems[randomCommonItem]);
            }
            else if (randomItem >= unCommonShance)
            {
                int randomUnCommonItem = Random.Range(0, unCommonItems.Count);
                baseItems.Add(unCommonItems[randomUnCommonItem]);
            }
            else
            {
                int randomDefaultItem = Random.Range(0, defaultItems.Count);
                baseItems.Add(defaultItems[randomDefaultItem]);
            }
        }

        return baseItems;
    }

    public List<BaseSkillTemplate> GenerateNesSkillItems()
    {
        List<BaseSkillTemplate> skills = new List<BaseSkillTemplate>();

        for (int i = 0; i < skillItems; i++)
        {
            skills.Add(skillsDataInfo.BaseSkillModels[Random.Range(0, skillsDataInfo.BaseSkillModels.Length)]);
        }

        return skills;
    }


    public bool BuyInventorySlot()
    {
        if (playerData.Money >= itemsDataInfo.InventorExtendetPrice[playerData.InventoryExtention] && itemsDataInfo.InventorExtendetPrice.Length - 1 > playerData.InventoryExtention)
        {
            playerData.Money -= itemsDataInfo.InventorExtendetPrice[playerData.InventoryExtention];
            playerData.CurrentInventorySize = itemsDataInfo.InventorSlotsByExtention[playerData.InventoryExtention];
            playerData.InventoryExtention++;
            return true;
        }
        else
        {
            Debug.Log("Not enough money");
            return false;
        }
    }


    public bool TryBoughtItem(BaseItem item)
    {
        if (playerData.Money >= item.itemPrice && playerData.HasFreeInventorySlots())
        {
            playerData.Money -= item.itemPrice;
            RemoveItem(item);
            playerData.AddItemToInventory(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryBoughtSkill(BaseSkillTemplate skill)
    {
        if (playerData.Money >= skill.BuyPrice)
        {
            playerData.Money -= skill.BuyPrice;
            playerData.PlayerAvailablesSkills.Add(skill.GetSkill());
            return true;
        }
        else
        {
            Debug.Log("Not enough money");
            return false;
        }
    }

    public void ProcessPurchaseItem(PurchaseItem item)
    {
        switch (item.purchaseType)
        {
            case PurchaseType.Gem:
                playerData.Gems += item.ContainsCount;
                break;
            case PurchaseType.Energy:
                playerData.Energy += item.ContainsCount;
                break;
            case PurchaseType.Gold:
                playerData.Money += item.ContainsCount;
                break;
            case PurchaseType.EpicCristal:
                playerData.EpicSummonCristal += item.ContainsCount;
                break;
            case PurchaseType.LegendaryCristal:
                playerData.LegendarySummonCristal += item.ContainsCount;
                break;
            case PurchaseType.CommonCristal:
                playerData.CommonSummonCristal += item.ContainsCount;
                break;
            case PurchaseType.RareCristal:
                playerData.RareSummonCristal += item.ContainsCount;
                break;
            default:
                break;
        }

        moneyChangeHandler.BoughtPurchaseItem(item);
    }
}
