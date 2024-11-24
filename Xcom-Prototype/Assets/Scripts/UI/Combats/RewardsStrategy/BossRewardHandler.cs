using System.Collections.Generic;
using UnityEngine;

public class BossRewardHandler : RewardHandlerBase
{
    private BossData _bossData;

    public BossRewardHandler(PlayerData playerData, SaveManager saveManager, TemploaryInfo temploaryInfo, RewardHandlerWindow currentWindow)
        : base(playerData, saveManager, temploaryInfo, currentWindow) 
    {
        _bossData = this.TemploaryInfo.CurrentBoss;
    }

    public override void ProcessRewards(BaseDragableItem crystalRewardPrefab, BaseDragableItem itemRewardPrefab, Transform root)
    {
        ProcessResources(crystalRewardPrefab, root);
        ProcessItems(itemRewardPrefab, root);
        ProcessExperience();
    }

    private void ProcessResources(BaseDragableItem crystalRewardPrefab, Transform root)
    {
        List<Resource> resources = _bossData.Rewards[(int)_bossData.Difficulty].ResourcesReward.Resources;
        foreach (var resource in resources)
        {
            AddResource(resource, crystalRewardPrefab, root);
        }
    }

    private void ProcessItems(BaseDragableItem itemRewardPrefab, Transform root)
    {
        List<ItemToDrop> items = _bossData.Rewards[(int)_bossData.Difficulty].RewardItems;
        if (items.Count > 0)
            AddItems(items, itemRewardPrefab, root);
    }

    private void ProcessExperience()
    {
        if(CurrentWindow == RewardHandlerWindow.Win)
            PlayerData.PlayerXP += _bossData.ExperiencePerBattle[(int)_bossData.Difficulty] * 2;
        else
            PlayerData.PlayerXP += _bossData.ExperiencePerBattle[(int)_bossData.Difficulty] / 2;
    }

    private void AddItems(List<ItemToDrop> items, BaseDragableItem prefab, Transform root)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int random = Random.Range(0, 100);
            if (random < items[i].DropChance)
            {
                var itemTemplate = items[i].Item;
                var itemCard = GameObject.Instantiate(prefab, root);
                var item = this.ItemsDataInfo.ConvertTemplateToItem<BaseItem>(itemTemplate);
                itemCard.SetItem(item, null);
                this.PlayerData.AddItemToInventory(item);
            }
        }
    }

    private async void AddResource(Resource resource, BaseDragableItem crystalRewardPrefab, Transform root)
    {
        CristalData crystalData = new();

        switch (resource.Type)
        {
            case (ResourceType.Gold):
                if(CurrentWindow == RewardHandlerWindow.Win)
                    Wallet.Instance.AddCachedCurrency(GameCurrencies.Gold, resource.Count);
                else
                    Wallet.Instance.AddCachedCurrency(GameCurrencies.Gold, resource.Count / 2);
                break;
            case (ResourceType.Gems):
                Wallet.Instance.AddCachedCurrency(GameCurrencies.Gem, resource.Count);
                break;
            case (ResourceType.Energy):
                Wallet.Instance.AddCachedCurrency(GameCurrencies.Energy, resource.Count);
                break;
            case (ResourceType.Keys):
                Wallet.Instance.AddCachedCurrency(GameCurrencies.Key, resource.Count);
                break;
            case (ResourceType.CommonSummonCrystal):
                await PlayerInventory.Instance.AddCristalToInventory(SummonCristalsEnum.Common, resource.Count);
                crystalData = this.ItemsDataInfo.CommonCristalData;
                break;
            case (ResourceType.RareSummonCrystal):
                await PlayerInventory.Instance.AddCristalToInventory(SummonCristalsEnum.Rare, resource.Count);
                crystalData = this.ItemsDataInfo.RareCristalData;
                break;
            case (ResourceType.EpicSummonCrystal):
                await PlayerInventory.Instance.AddCristalToInventory(SummonCristalsEnum.Epic, resource.Count);
                crystalData = this.ItemsDataInfo.EpicCristalData;
                break;
            case (ResourceType.LegendarySummonCrystal):
                await PlayerInventory.Instance.AddCristalToInventory(SummonCristalsEnum.Legendary, resource.Count);
                crystalData = this.ItemsDataInfo.LegendaryCristalData;
                break;
            case (ResourceType.MythicalSummonCrystal):
                await PlayerInventory.Instance.AddCristalToInventory(SummonCristalsEnum.Mythical, resource.Count);
                crystalData = this.ItemsDataInfo.MythicalCristalData;
                break;

            default:
                Debug.LogError($"Unknown resource type <color=yellow>{resource.Type}</color>");
                return;
        }

        ResourceTypeData resourceTypeData = resource.GetResourceTypeData();

        switch (resourceTypeData)
        {
            case ResourceTypeData.Currency:
                break;

            case ResourceTypeData.Crystal:
                BaseDragableItem slot = GameObject.Instantiate(crystalRewardPrefab, root);
                slot.SetCristal(crystalData);
                break;

            default:
                Debug.LogError($"Unknown resourceTypeData <color=yellow>{resourceTypeData}</color>");
                return;
        }
    }
}