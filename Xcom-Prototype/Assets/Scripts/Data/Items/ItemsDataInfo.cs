using System;
using System.Collections.Generic;
using System.Linq;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ItemsDataInfo", menuName = "Data/Items/ItemsDataInfo")]
public class ItemsDataInfo : ScriptableObject
{
    public static ItemsDataInfo Instance;

    public List<ItemTemplate> Items = new List<ItemTemplate>();
    public List<WeaponItemTemaplate> Weapons = new List<WeaponItemTemaplate>();
    public List<ArmorItemTemplate> Armors = new List<ArmorItemTemplate>();
    public List<ItemSetTemplate> itemSets = new List<ItemSetTemplate>();
    public List<RingItemTemplate> Rings = new List<RingItemTemplate>();
    public List<AmuletItemTemplate> Amulets = new List<AmuletItemTemplate>();
    public List<ArmletItemTemplate> Armlets = new List<ArmletItemTemplate>();
    public List<CraftItemTemplate> CraftItems = new List<CraftItemTemplate>();
    public List<ArtifactRecipeTemplate> Recipes = new List<ArtifactRecipeTemplate>();

    public int[] InventorExtendetPrice = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
    public int[] InventorSlotsByExtention = { 25, 30, 40, 50, 60, 70, 80, 90, 100 };

    public Color[] RareColors;
    public Sprite[] RareBackgrounds;
    public CristalData[] CristalsInfo;

    public ResourceManager ResourceManager;
    public List<ResourceData> ResourcesData = new List<ResourceData>();

    public CristalData CommonCristalData;
    public CristalData RareCristalData;
    public CristalData EpicCristalData;
    public CristalData LegendaryCristalData;
    public CristalData MythicalCristalData;


    [Inject]
    public ItemsDataInfo()
    {
        Instance = this;
    }

    #region PublicMethods
    public ItemTemplate GetItemTemplate(int itemID)
    {
        ItemTemplate foundItem = FindItemByID(Items, itemID) ??
                                 FindItemByID(Weapons, itemID) ??
                                 FindItemByID(Armors, itemID) ??
                                 FindItemByID(Rings, itemID) ??
                                 FindItemByID(Armlets, itemID) ??
                                 FindItemByID(Amulets, itemID) ??
                                 FindItemByID(CraftItems, itemID) ??
                                 FindItemByID(Recipes, itemID);

        return foundItem;
    }

    public BaseItem ConvertTemplateToItem<T>(ItemTemplate template, int level = 1, bool addSkillSet = true)
    {
        if (level > GameConstants.MaxItemLevel)
            level = GameConstants.MaxItemLevel;
        else if (level < GameConstants.MinItemLevel)
            level = GameConstants.MinItemLevel;

        switch (template)
        {
            case WeaponItemTemaplate weaponTemplate:
                WeaponItem weaponItem = new WeaponItem();
                SetupItem(weaponItem, weaponTemplate, level, addSkillSet);
                weaponItem = CreateWeaponItem(weaponTemplate, weaponItem);

                Debug.Log($">>> Created item with set: {weaponItem.ItemsSet.SetName}");

                return weaponItem;

            case ArmorItemTemplate armorTemplate:
                ArmorItem armorItem = new ArmorItem();
                SetupItem(armorItem, armorTemplate, level, addSkillSet);
                armorItem = CreateArmorItem(armorTemplate, armorItem);

                Debug.Log($">>> Created item with set: {armorItem.ItemsSet.SetName}");

                return armorItem;

            case RingItemTemplate ringTemplate:
                RingItem ringItem = new RingItem();
                SetupItem(ringItem, template, level, addSkillSet);
                ringItem = CreateRingItem(ringTemplate, ringItem);

                Debug.Log($">>> Created item with set: {ringItem.ItemsSet.SetName}");

                return ringItem;

            case AmuletItemTemplate amuletTemplate:
                AmuletItem amuletItem = new AmuletItem();
                SetupItem(amuletItem, amuletTemplate, level, addSkillSet);
                amuletItem = CreateAmuletItem(amuletTemplate, amuletItem);

                Debug.Log($">>> Created item with set: {amuletItem.ItemsSet.SetName}");

                return amuletItem;

            case ArmletItemTemplate armletTemplate:
                ArmletItem armletItem = new ArmletItem();
                SetupItem(armletItem, armletTemplate, level, addSkillSet);
                armletItem = CreateArmletItem(armletTemplate, armletItem);

                Debug.Log($">>> Created item with set: {armletItem.ItemsSet.SetName}");

                return armletItem;

            case CraftItemTemplate craftItemTemplate:
                CraftItem craftItem = new CraftItem();
                SetupItem(craftItem, craftItemTemplate, level, addSkillSet);
                craftItem = CreateCraftItem(craftItem);

                Debug.Log($">>> Created item with set: {craftItem.ItemsSet.SetName}");

                return craftItem;

            case ArtifactRecipeTemplate recipeTemplate:
                ArtifactRecipeItem artifactRecipeItem = new ArtifactRecipeItem();
                SetupItem(artifactRecipeItem, recipeTemplate, level, addSkillSet);
                artifactRecipeItem = CreateRecipeItem(recipeTemplate, artifactRecipeItem);

                Debug.Log($">>> Created item with set: {artifactRecipeItem.ItemsSet.SetName}");

                return artifactRecipeItem;

        }


        return null;
    }

    public ItemTemplate GetRandomItemTemplateOfType(DropItemType itemType, RareEnum rarity)
    {
        switch (itemType)
        {
            //Weapons
            case DropItemType.Axe:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Axe);
            case DropItemType.Bow:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Bow);
            case DropItemType.Dagger:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Dagger);
            case DropItemType.Mace:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Mace);
            case DropItemType.Spear:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Spear);
            case DropItemType.Sword:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Sword);
            case DropItemType.Wand:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Wand);
            case DropItemType.THAxe:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.TwoHandedAxe);
            case DropItemType.THMace:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.TwoHandedMace);
            case DropItemType.THSword:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.TwoHandedSword);
            case DropItemType.Shield:
                return GetRandomWeaponTemplate(rarity, WeaponTypeEnum.Shield);

            //Armor
            case DropItemType.ChestArmor:
                return GetRandomArmorTemplate(rarity, ArmorType.ChestArmor);
            case DropItemType.Gloves:
                return GetRandomArmorTemplate(rarity, ArmorType.Gloves);
            case DropItemType.Helmet:
                return GetRandomArmorTemplate(rarity, ArmorType.Helmet);
            case DropItemType.Legs:
                return GetRandomArmorTemplate(rarity, ArmorType.Legs);

            case DropItemType.Craft:
                return GetRandomCraftTemplate(rarity);

            default:
                Debug.LogError($"Unknown item type {itemType}");
                return null;
        }
    }

    public ResourceData GetResourceData(Resource resource)
    {
        for (int i = 0; i < ResourcesData.Count; i++)
        {
            if (ResourcesData[i].ResourceType == resource.Type)
                return ResourcesData[i];
        }
        return null;
    }

    public ResourceData GetResourceData(ResourceType type)
    {
        for (int i = 0; i < ResourcesData.Count; i++)
        {
            if (ResourcesData[i].ResourceType == type)
                return ResourcesData[i];
        }
        return null;
    }
    #endregion

    #region ItemsCreation
    private ArmorItem CreateArmorItem(ArmorItemTemplate armorTemplate, ArmorItem armorItem)
    {
        if (armorTemplate.ArmorUpgradeStats != null && armorTemplate.ArmorUpgradeStats.Length > armorItem.CurrentLevel + 1)
        {
            armorItem.SetupLevel(armorTemplate.ArmorUpgradeStats[armorItem.CurrentLevel], armorTemplate.ArmorUpgradeStats[armorItem.CurrentLevel + 1]);
        }
        else if (armorTemplate.ArmorUpgradeStats != null && armorTemplate.ArmorUpgradeStats.Length > armorItem.CurrentLevel)
        {
            armorItem.SetupLevel(armorTemplate.ArmorUpgradeStats[armorItem.CurrentLevel], null);
        }

        return armorItem;
    }

    private WeaponItem CreateWeaponItem(WeaponItemTemaplate weaponTemplate, WeaponItem weaponItem)
    {
        if (weaponTemplate.WeaponUpgradeStats != null && weaponTemplate.WeaponUpgradeStats.Length > weaponItem.CurrentLevel + 1)
        {
            weaponItem.SetupLevel(weaponTemplate.WeaponUpgradeStats[weaponItem.CurrentLevel], weaponTemplate.WeaponUpgradeStats[weaponItem.CurrentLevel + 1]);
        }
        else if (weaponTemplate.WeaponUpgradeStats != null && weaponTemplate.WeaponUpgradeStats.Length > weaponItem.CurrentLevel)
        {
            weaponItem.SetupLevel(weaponTemplate.WeaponUpgradeStats[weaponItem.CurrentLevel], null);
        }

        weaponItem.weaponType = weaponTemplate.weaponType;
        return weaponItem;
    }

    private RingItem CreateRingItem(RingItemTemplate ringTemplate, RingItem ringItem)
    {
        if (ringTemplate.RingUpgradeStats != null && ringTemplate.RingUpgradeStats.Length > ringItem.CurrentLevel)
        {
            ringItem.SetupLevel(ringTemplate.RingUpgradeStats[ringItem.CurrentLevel]);
        }

        return ringItem;
    }

    private AmuletItem CreateAmuletItem(AmuletItemTemplate amuletTemplate, AmuletItem amuletItem)
    {
        if (amuletTemplate.AmuletUpgradeStats != null && amuletTemplate.AmuletUpgradeStats.Length > amuletItem.CurrentLevel)
        {
            amuletItem.SetupLevel(amuletTemplate.AmuletUpgradeStats[amuletItem.CurrentLevel]);
        }

        return amuletItem;
    }

    private ArmletItem CreateArmletItem(ArmletItemTemplate armletTemplate, ArmletItem armletItem)
    {
        if (armletTemplate.ArmletUpgradeStats != null && armletTemplate.ArmletUpgradeStats.Length > armletItem.CurrentLevel)
        {
            armletItem.SetupLevel(armletTemplate.ArmletUpgradeStats[armletItem.CurrentLevel]);
        }

        return armletItem;
    }

    private CraftItem CreateCraftItem(CraftItem craftItem)
    {
        return craftItem;
    }

    private ArtifactRecipeItem CreateRecipeItem(ArtifactRecipeTemplate recipeTemplate, ArtifactRecipeItem recipeItem)
    {
        recipeItem.RecipeData = recipeTemplate.RecipeData;
        recipeItem.TargetItem = recipeTemplate.TargetItemTemplate;
        recipeItem.Price = recipeTemplate.Price;
        recipeItem.Currency = recipeTemplate.Currency;

        return recipeItem;
    }

    private void SetupItem<I, T>(I item, T template, int level, bool addRandomSkill)
        where I : BaseItem
        where T : ItemTemplate
    {
        if (level > GameConstants.MaxItemLevel)
            level = GameConstants.MaxItemLevel;
        else if (level < GameConstants.MinItemLevel)
            level = GameConstants.MinItemLevel;

        item.SetStatsFromTemplate(template, level);
        item.Slot = template.Slot;

        List<BaseItemsSet> awailableSets = SetItemsContainer.Instance.GetAllSetsContainingItem(template);

        if(awailableSets != null && awailableSets.Count > 0)
        {
            Debug.Log($">>> Count: {awailableSets.Count}");
            item.ItemsSet = awailableSets[UnityEngine.Random.Range(0, awailableSets.Count)];

            BaseSkillModel randomSkill = item.ItemsSet.SkillSet.GetRandomSkill(template);

            if(randomSkill != null && addRandomSkill)
            {
                item.skillModels = new List<BaseSkillModel>
                {
                    randomSkill
                };
            }
        }

        //***To add random skill from skillSet in ItemTemplate (for test) ***

        /*foreach (var skillSet in template.ItemSkillSets)
        {
            BaseSkillModel skill = skillSet.GetRandomSkill(template);
            item.skillModels.Add(skill);
        }
        */

        if (template.itemMaxStack == 1 || template.itemMaxStack == 0)
            item.itemCount = 1;
    }

    private ItemTemplate GetRandomWeaponTemplate(RareEnum rarity, WeaponTypeEnum weaponType)
    {
        ItemTemplate[] matchingWeapons = Weapons.Where(w => w.weaponType == weaponType && w.Rare == rarity).ToArray();

        if (matchingWeapons.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, matchingWeapons.Length);
            return matchingWeapons[randomIndex];
        }

        Debug.LogError($"Not found {weaponType}");
        return null;
    }

    private ItemTemplate GetRandomArmorTemplate(RareEnum rarity, ArmorType armorType)
    {
        ItemTemplate[] matchingArmors = Armors.Where(a => a.ArmorType == armorType && a.Rare == rarity).ToArray();

        if (matchingArmors.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, matchingArmors.Length);
            return matchingArmors[randomIndex];
        }

        Debug.LogError($"Not found {armorType}");
        return null;
    }

    private ItemTemplate GetRandomCraftTemplate(RareEnum rarity)
    {
        ItemTemplate[] matchingCraftItems = CraftItems.Where(c => c.Rare == rarity).ToArray();

        if (matchingCraftItems.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, matchingCraftItems.Length);
            return matchingCraftItems[randomIndex];
        }

        Debug.LogError($"Not found craft item with rarity {rarity}");
        return null;
    }
    #endregion

    #region ContextMenu

    [ContextMenu("Generate Uniq Ids")]
    private void GenerateUniqIDs()
    {
        int id = 0;

        id = GenerateIDForItemsLists(Weapons, id);
        id = GenerateIDForItemsLists(Armors, id);
        id = GenerateIDForItemsLists(Amulets, id);
        id = GenerateIDForItemsLists(Armlets, id);
        id = GenerateIDForItemsLists(Rings, id);
        id = GenerateIDForItemsLists(CraftItems, id);
        id = GenerateIDForItemsLists(Recipes, id);

        Debug.Log($">>>Generated ID for items. Last ID = {id}");
    }

    [ContextMenu("Reset Stats")]
    private void ResetStats()
    {
        ResetStatsForList(Armors);
        ResetStatsForList(Weapons);
        ResetStatsForList(Amulets);
        ResetStatsForList(Armlets);
        ResetStatsForList(Rings);
    }

    [ContextMenu("Find item by name")]
    private void DebugItemName()
    {
        string itemName = "Common Hood 2"; //Put here name of item that you want to find. For devs
        ItemTemplate foundItem = FindItemByName(Items, itemName) ??
                                 FindItemByName(Weapons, itemName) ??
                                 FindItemByName(Armors, itemName) ??
                                 FindItemByName(Rings, itemName) ??
                                 FindItemByName(Armlets, itemName) ??
                                 FindItemByName(Amulets, itemName) ??
                                 FindItemByName(CraftItems, itemName) ??
                                 FindItemByName(Recipes, itemName);

        Debug.Log($"Found item name is: <color=green>{foundItem.name}</color>");
    }

    private void ResetStatsForList<T>(List<T> items) where T : ItemTemplate
    {
        foreach (var item in items)
        {
            item.ResetStats();
        }
    }

    private int GenerateIDForItemsLists<T>(List<T> items, int ID) where T : ItemTemplate
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = ID;
            ID++;

            //EditorUtility.SetDirty(items[i]);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }

        return ID;
    }

    [ContextMenu("RenameIcons")]
    private void RenameAllIconsFiles()
    {
        foreach (var item in Recipes)
        {
            //var path = AssetDatabase.GetAssetPath(item.itemSprite);
            //var newName = "ItemIcon" + item.itemID.ToString();
            //item.itemSprite = null;
            //item.ResourceManager = ResourceManager;
            //AssetDatabase.RenameAsset(path, newName);
            //EditorUtility.SetDirty(item.itemSprite);
            //EditorUtility.SetDirty(item);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }
    }
    #endregion

    #region Search
    private ItemTemplate FindItemByID<T>(List<T> itemList, int itemID) where T : ItemTemplate
    {
        return itemList.FirstOrDefault(item => item.itemID == itemID);
    }

    private ItemTemplate FindItemByName<T>(List<T> itemList, string itemName) where T : ItemTemplate
    {
        return itemList.FirstOrDefault(item => item.itemName == itemName);
    }

    #endregion
}