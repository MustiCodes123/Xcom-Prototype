using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardItem
{
    [field: SerializeField] public Sprite Icon { get; set; }
    [field: SerializeField] public Object Item { get; set; }
    [field: SerializeField] public string Title { get; set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public int Amount { get; set; }  // Only for display Gold and XP rewards. For other items Amount not used and must be 0.
}

[CreateAssetMenu(fileName = "RewardData", menuName = "Rewards/NewRewardData")]
public class RewardData : ScriptableObject
{
    [field: SerializeField] public List<RewardItem> RewardItems { get; set; }
}