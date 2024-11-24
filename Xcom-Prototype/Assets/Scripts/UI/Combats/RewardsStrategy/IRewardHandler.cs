using UnityEngine;

public interface IRewardHandler
{
    public void ProcessRewards(BaseDragableItem crystalRewardPrefab, BaseDragableItem itemRewardPrefab, Transform root);
}