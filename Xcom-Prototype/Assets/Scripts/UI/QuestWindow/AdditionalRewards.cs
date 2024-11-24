using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AdditionalRewards : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    [SerializeField] private Slider additionalRewardSlider;
    [SerializeField] private TextMeshProUGUI maxAdditionalQuestCount;
    [SerializeField] private TextMeshProUGUI additionalQuestCount;
    [SerializeField] private TextMeshProUGUI questChapterCount;
    [SerializeField] private AdditionalRewardCard additionalRewardPrefab;
    [SerializeField] private QuestWindowView questWindow;
    [SerializeField] private BaseItemSlot[] additionalRewardSlots;

    private int _currentChapter = 1;
    private const int TotalChapters = 7;

    private List<AdditionalRewardCard> _additionalRewards = new List<AdditionalRewardCard>();
    private QuestManager _questManager;
    private int _additionalCompletedQuestCount;

    public int RegularCompletedQuestCount { get; set; }

    public void ShowAdditionalRewards(QuestManager questManager)
    {
        _questManager = questManager;
        AllQuestsJsonData data = _questManager.GetSaveData();

        RegularCompletedQuestCount = data.RegularCompletedAmount;
        _additionalCompletedQuestCount = data.AdditionalCompletedAmount;

        Debug.Log($"RegularCompletedAmount {RegularCompletedQuestCount}");
        Debug.Log($"AdditionalCompletedAmount {_additionalCompletedQuestCount}");

        if (_additionalRewards.Count == 0)
        {
            InitializeAdditionalRewards();
        }

        UpdateRewardCards();
        
        if (_additionalCompletedQuestCount > _questManager.AdditionalQuests.Count)
        {
            TakeAllRewardsAndResetQuests();
        }
    }

    private void InitializeAdditionalRewards()
    {
        for (int i = 0; i < _questManager.AdditionalQuests.Count; i++)
        {
            var quest = _questManager.AdditionalQuests[i];
            var questCard = questWindow.CreateAdditionalRewardCard(quest, additionalRewardSlots[i].transform);
            _additionalRewards.Add(questCard);
        }
    }

    private void UpdateRewardCards()
    {
        for (var i = 0; i < _additionalRewards.Count; i++)
        {
            var quest = _questManager.AdditionalQuests[i];
            var questCard = _additionalRewards[i];

            if (!quest.IsCompleted && !quest.IsRewardTaken)
            {
                // Handle not completed and not taken rewards
            }
            else if (quest.IsCompleted && !quest.IsRewardTaken)
            {
              _additionalRewards[i].ChangeBackGround();
            }
            else if (quest.IsCompleted && quest.IsRewardTaken)
            {
                questCard.DeactivateCard();
            }
        }
    }

    private void TakeAllRewardsAndResetQuests()
    {
        foreach (var questCard in _additionalRewards)
        {
            var quest = _questManager.AdditionalQuests[_additionalRewards.IndexOf(questCard)];
            questCard.TakeAllRewards(quest);
        }

        foreach (var questCard in _additionalRewards)
        {
            var quest = _questManager.AdditionalQuests[_additionalRewards.IndexOf(questCard)];
            quest.ResetQuest();
            questCard.ActivateCard();
        }

        DestroyAdditionalQuests();
        RegularCompletedQuestCount = 0;
        _currentChapter++;
        questChapterCount.text = $"{_currentChapter}/{TotalChapters}";
        additionalRewardSlider.value = 0;
        additionalQuestCount.text = RegularCompletedQuestCount.ToString();
    }

    public void UpdateAdditionalRewardProgress()
    {
        additionalRewardSlider.maxValue = _questManager.AdditionalQuests.Count;
        maxAdditionalQuestCount.text = _questManager.AdditionalQuests.Count.ToString();

        additionalRewardSlider.value = RegularCompletedQuestCount;
        additionalQuestCount.text = RegularCompletedQuestCount.ToString();
    }

    public void DestroyAdditionalQuests()
    {
        foreach (var card in _additionalRewards)
        {
            Destroy(card.gameObject);
        }
        _additionalRewards.Clear();
    }
}
