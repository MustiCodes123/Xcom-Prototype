using System.Collections.Generic;
using UnityEngine;

public class LevelRewardHandler : RewardHandlerBase
{
    private List<BaseItem> itemsToReward = new List<BaseItem>();
    private RewardsDropHelper dropHelper;

    public LevelRewardHandler(PlayerData playerData, SaveManager saveManager, TemploaryInfo temploaryInfo, RewardHandlerWindow currentWindow)
        : base(playerData, saveManager, temploaryInfo, currentWindow) { }

    public override void ProcessRewards(BaseDragableItem crystalRewardPrefab, BaseDragableItem itemRewardPrefab, Transform root)
    {
        foreach (var child in root.GetComponentsInChildren<BaseDragableItem>())
        {
            child.gameObject.SetActive(false);
        }

        ProcessExperience();
        ProcessMoney();
        ProcessCrystals(crystalRewardPrefab, root);
        ProcessChests(itemRewardPrefab, root);
        ProcessBrokenChests(itemRewardPrefab, root);

        if (itemsToReward.Count == 0)
        {
            ProcessItems(itemRewardPrefab, root);
        }
        else
        {
            AddItemsToInventory(itemsToReward);
        }
    }

    public List<BaseItem> CalculateRewardItems(BaseDragableItem prefab, Transform root)
    {
        dropHelper = new RewardsDropHelper(prefab, root);
        itemsToReward = dropHelper.CalculateRewardItems(this.TemploaryInfo);

        return itemsToReward;
    }
    private void AddItemsToInventory(List<BaseItem> itemsToAdd)
    {
        dropHelper.ShowItemReward(itemsToAdd);

        foreach (var item in itemsToAdd)
        {
            this.PlayerData.AddItemToInventory(item);
        }
    }
    private void ProcessItems(BaseDragableItem prefab, Transform root)
    {
        RewardsDropHelper rewardsDropHelper = new RewardsDropHelper(prefab, root);
        rewardsDropHelper.TryToDropItems(this.TemploaryInfo, this.PlayerData);
    }

    private void ProcessExperience()
    {
        if(CurrentWindow == RewardHandlerWindow.Win)
            PlayerData.PlayerXP += this.TemploaryInfo.LevelInfo.XP * 2;
        else
            PlayerData.PlayerXP += this.TemploaryInfo.LevelInfo.XP / 2;
    }

    private void ProcessMoney()
    {
        int moneyCount = this.TemploaryInfo.LevelInfo.Gold;

        if (CurrentWindow == RewardHandlerWindow.Lose)
            moneyCount /= 2;

        Wallet.Instance.AddCachedCurrency(GameCurrencies.Gold, moneyCount);
    }

    private void ProcessCrystals(BaseDragableItem prefab, Transform root)
    {
        Debug.Log($"root name {root.gameObject.name}");

        RewardsDropHelper rewardsDropHelper = new RewardsDropHelper(prefab, root);
        rewardsDropHelper.TryAddCristal(this.TemploaryInfo);
    }

    private void ProcessChests(BaseDragableItem prefab, Transform root)
    {
        RewardsDropHelper rewardsDropHelper = new RewardsDropHelper(prefab, root);
        rewardsDropHelper.TryAddChest(this.TemploaryInfo);
    }

    private void ProcessBrokenChests(BaseDragableItem prefab, Transform root)
    {
        RewardsDropHelper rewardsDropHelper = new RewardsDropHelper(prefab, root);
        rewardsDropHelper.TryAddBrokenChest(this.TemploaryInfo);
    }
}