using UnityEngine;

public enum RewardHandlerWindow
{
    Lose,
    Win
}

public abstract class RewardHandlerBase : IRewardHandler
{
    protected PlayerData PlayerData;
    protected SaveManager SaveManager;
    protected TemploaryInfo TemploaryInfo;
    protected ItemsDataInfo ItemsDataInfo;
    protected RewardHandlerWindow CurrentWindow;

    public RewardHandlerBase(PlayerData playerData, SaveManager saveManager, TemploaryInfo temploaryInfo, RewardHandlerWindow currentWindow)
    {
        this.PlayerData = playerData;
        this.SaveManager = saveManager;
        this.TemploaryInfo = temploaryInfo;
        CurrentWindow = currentWindow;

        this.ItemsDataInfo = ItemsDataInfo.Instance;
    }

    public abstract void ProcessRewards(BaseDragableItem crystalRewardPrefab, BaseDragableItem itemRewardPrefab, Transform root);

    protected virtual void SaveProgress()
    {
        SaveManager.SaveGame();
    }
}