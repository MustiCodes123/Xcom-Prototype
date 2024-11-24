using UnityEngine;

public class PvPRewardHandler : RewardHandlerBase
{
    private PvPPlayerService _pvpPlayerService;
    private PvPBattleData _pvpBattleData;

    public PvPRewardHandler(PlayerData playerData, SaveManager saveManager, TemploaryInfo temploaryInfo, PvPPlayerService pvpPlayerService, PvPBattleData pvpBattleData, RewardHandlerWindow currentWindow)
        : base(playerData, saveManager, temploaryInfo, currentWindow) 
    {
        _pvpPlayerService = pvpPlayerService;
        _pvpBattleData = pvpBattleData;
    }

    public override void ProcessRewards(BaseDragableItem crystalRewardPrefab, BaseDragableItem itemRewardPrefab, Transform root)
    {
        ProcessExperience();
        ProcessItems(itemRewardPrefab, root);
        ProcessCrystals(crystalRewardPrefab, root);
        ProcessChests(itemRewardPrefab, root);
        ProcessBrokenChests(itemRewardPrefab, root);
        ProcessMoney();
    }

    private void ProcessExperience()
    {
        int experience = _pvpBattleData.ExperiencePerBattle;

        if (CurrentWindow == RewardHandlerWindow.Lose)
            experience /= 2;
        else
            experience *= 2;

        this.PlayerData.PlayerXP += experience;
        _pvpPlayerService.AddExperience(experience, _pvpBattleData, this.PlayerData);
    }

    private void ProcessItems(BaseDragableItem prefab, Transform root)
    {
        RewardsDropHelper rewardsDropHelper = new RewardsDropHelper(prefab, root);
        rewardsDropHelper.TryToDropItems(this.TemploaryInfo, this.PlayerData);
    }

    private void ProcessCrystals(BaseDragableItem prefab, Transform root)
    {
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

    private void ProcessMoney()
    {
        int rewardMultiplier = _pvpBattleData.CurrentRewardMultiplier(this.TemploaryInfo.FakeLeader);
        int moneyCount = 0;

        for (int i = 0; i < _pvpBattleData.Reward.Resources.Count; i++)
        {
            if (_pvpBattleData.Reward.Contains(ResourceType.Gold, out var resource))
                moneyCount = resource.Count * rewardMultiplier;
        }

        if (CurrentWindow == RewardHandlerWindow.Lose)
            moneyCount /= 2;
        else
            moneyCount *= 2;

        Wallet.Instance.AddCachedCurrency(GameCurrencies.Gold, moneyCount);
    }
}