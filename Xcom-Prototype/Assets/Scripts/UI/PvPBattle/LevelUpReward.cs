using System.Collections.Generic;
using System;

[Serializable]
public class LevelUpReward
{
    public List<ItemTemplate> RewardItem = new List<ItemTemplate>();
    public Reward ResourcesReward;
}

[Serializable]
public class BossReward
{
    public List<ItemToDrop> RewardItems = new List<ItemToDrop>();
    public Reward ResourcesReward;
}

