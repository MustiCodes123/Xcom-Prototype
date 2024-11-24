using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] private Transform _iconsParent;
    [SerializeField] private RewardDescriptionPopup _rewardPopup;

    private List<RewardItemView> _currentDisplayedRewards = new List<RewardItemView>();
    private FightWindowPool _fightWindowPool;

    [Inject]
    public void Constructor(FightWindowPool pool)
    {
        _fightWindowPool = pool;
    }
    

    public void DisplayRewards(RewardData rewardData)
    {
        ClearCurrentRewards();

        //foreach (var rewardItem in rewardData.RewardItems)
         for (int i = 0; i < rewardData.RewardItems.Count; i++)
        {
            var rewardItem = rewardData.RewardItems[i];
            RewardItemView rewardItemView = _fightWindowPool.SpawnRewardIcon(_iconsParent);
            rewardItemView.transform.SetSiblingIndex(i);
            rewardItemView.DisplayReward(rewardItem);
            rewardItemView.gameObject.SetActive(true);
            _currentDisplayedRewards.Add(rewardItemView);
        }

    }

    private void ClearCurrentRewards()
    {
        foreach (var reward in _currentDisplayedRewards)
        {
            reward.gameObject.SetActive(false);
            _fightWindowPool.RemoveRewardIcon(reward);
        }

        _currentDisplayedRewards.Clear();
    }

    private void OnDestroy()
    {
        ClearCurrentRewards();
    }
}


