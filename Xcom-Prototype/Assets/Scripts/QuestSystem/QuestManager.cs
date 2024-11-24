using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class QuestManager : MonoBehaviour
{
    private PlayerData playerData;
    private SignalBus signalBus;
    private SaveManager saveManager;

    public List<Quest> AllQuests = new List<Quest>();
    public List<Quest> DailyQuests = new List<Quest>();
    public List<Quest> WeeklyQuests = new List<Quest>();
    public List<Quest> MonthQuests = new List<Quest>();
    public List<Quest> RegularQuests = new List<Quest>();
    public List<Quest> AdditionalQuests = new List<Quest>();
    public List<Quest> TemporaryRewardQuests = new List<Quest>();

    [Inject]
    public void Init(SignalBus signalBus, PlayerData playerData, SaveManager saveManager)
    {
        this.playerData = playerData;
        this.signalBus = signalBus;
        this.saveManager = saveManager;

        saveManager.SetQuestManager(this);

        AddAllQuests();

        signalBus.Subscribe<CharacterSelectSignal>(OnCharacterSelected);
        signalBus.Subscribe<CharacterEquipSignal>(OnCharacterEquiped);
        signalBus.Subscribe<CharacterLevelpSignal>(OnCharacterLevelUp);
        signalBus.Subscribe<ItemBuySignal>(OnItemBuySignal);
        signalBus.Subscribe<LevelFinishSignal>(OnLevelFinished);
        signalBus.Subscribe<QuestCompleteSignal>(OnQuestComplete);
        signalBus.Subscribe<SummonHeroSignal>(OnSummonHero);
        signalBus.Subscribe<UpgradeSignal>(OnUpgrade);
        signalBus.Subscribe<UseResourceSignal>(OnSpendResource);
    }

    public void AddAllQuests()
    {
        AllQuests.AddRange(DailyQuests);
        AllQuests.AddRange(WeeklyQuests);
        AllQuests.AddRange(MonthQuests);
        AllQuests.AddRange(RegularQuests);
        AllQuests.AddRange(AdditionalQuests);
        AllQuests.AddRange(TemporaryRewardQuests);
    }

    public void AddAdditionalQuests()
    {
        AllQuests.AddRange(AdditionalQuests);
    }

    private void OnLevelFinished(LevelFinishSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnItemBuySignal(ItemBuySignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnCharacterLevelUp(CharacterLevelpSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnCharacterEquiped(CharacterEquipSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnCharacterSelected(CharacterSelectSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnQuestComplete(QuestCompleteSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnSummonHero(SummonHeroSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnUpgrade(UpgradeSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    private void OnSpendResource(UseResourceSignal signal)
    {
        AllQuests.ForEach(x => x.Process(signal));
    }

    public void Evaluate()
    {
        foreach (var quest in AllQuests)
        {
            quest.Evaluate();
        }
    }

    public AllQuestsJsonData GetSaveData()
    {
        AllQuestsJsonData allQuestsData = new AllQuestsJsonData
        {
            Data = AllQuests.Select(q => q.GetSaveData()).ToArray(),
            RegularCompletedAmount = RegularQuests.Where(q => q.IsCompleted && q.IsRewardTaken).Count(),
            AdditionalCompletedAmount = AdditionalQuests.Where(q => q.IsCompleted && q.IsRewardTaken).Count()
        };

        return allQuestsData;
    }

    public void LoadQuests(AllQuestsJsonData allQuestsData)
    {
        if (allQuestsData == null || allQuestsData.Data == null)
        {
            Debug.LogWarning("No quest data to load.");
            return;
        }

        int index = 0;

        foreach (var questData in allQuestsData.Data)
        {
            if (index < DailyQuests.Count)
            {
                DailyQuests[index].LoadFromData(questData);
            }
            else if (index < DailyQuests.Count + WeeklyQuests.Count)
            {
                WeeklyQuests[index - DailyQuests.Count].LoadFromData(questData);
            }
            else if (index < DailyQuests.Count + WeeklyQuests.Count + MonthQuests.Count)
            {
                MonthQuests[index - DailyQuests.Count - WeeklyQuests.Count].LoadFromData(questData);
            }
            else if (index < DailyQuests.Count + WeeklyQuests.Count + MonthQuests.Count + RegularQuests.Count)
            {
                RegularQuests[index - DailyQuests.Count - WeeklyQuests.Count - MonthQuests.Count].LoadFromData(questData);
            }
            else if (index < DailyQuests.Count + WeeklyQuests.Count + MonthQuests.Count + RegularQuests.Count + AdditionalQuests.Count)
            {
                AdditionalQuests[index - DailyQuests.Count - WeeklyQuests.Count - MonthQuests.Count - RegularQuests.Count].LoadFromData(questData);
            }
            else if (index < DailyQuests.Count + WeeklyQuests.Count + MonthQuests.Count + RegularQuests.Count + AdditionalQuests.Count + TemporaryRewardQuests.Count)
            {
                TemporaryRewardQuests[index - DailyQuests.Count - WeeklyQuests.Count - MonthQuests.Count - RegularQuests.Count - AdditionalQuests.Count].LoadFromData(questData);
            }

            index++;
        }
    }
    public List<Quest> GetListByType(QuestTypeEnum questType)
    {
        switch (questType)
        {
            case QuestTypeEnum.Daily:
                return DailyQuests;
            case QuestTypeEnum.Weekly:
                return WeeklyQuests;
            case QuestTypeEnum.Monthly: 
                return MonthQuests;
            case QuestTypeEnum.Permanent:
                return RegularQuests;
            case QuestTypeEnum.AdditionalReward:
                return AdditionalQuests;
            case QuestTypeEnum.TeporaryRewardDaily:
                return DailyQuests;
            case QuestTypeEnum.TeporaryRewardWeekly:
                return WeeklyQuests;
            case QuestTypeEnum.TeporaryRewardMonthly:
                return MonthQuests;
            default
                : return null;
        }
    }

    [ContextMenu("Reset AllQuests")]
    public void ResetAllQuests()
    {
        ResetQuestList(DailyQuests);
        ResetQuestList(WeeklyQuests);
        ResetQuestList(MonthQuests);
        ResetQuestList(RegularQuests);
        ResetQuestList(AdditionalQuests);
    }

    [ContextMenu("Reset DailyQuests")]
    private void ResetDailyQuests()
    {
        ResetQuestList(DailyQuests);
    }

    [ContextMenu("Reset WeeklyQuests")]
    private void ResetWeeklyQuests()
    {
        ResetQuestList(WeeklyQuests);
    }

    [ContextMenu("Reset MonthlyQuests")]
    private void ResetMonthlyQuests()
    {
        ResetQuestList(MonthQuests);
    }

    [ContextMenu("Reset RegularQuests")]
    private void ResetRegularQuests()
    {
        ResetQuestList(RegularQuests);
    }

    [ContextMenu("Reset AdditionalRewards")]
    private void ResetAdditionalReards()
    {
        ResetQuestList(AdditionalQuests);
    }

    [ContextMenu("Reset AdditionalTemproraryRewards")]
    private void ResetAdditionalTemproraryReards()
    {
        ResetQuestList(TemporaryRewardQuests);
    }

    private void ResetQuestList(List<Quest> questList)
    {
        foreach (var quest in questList)
        {
            quest.ResetQuest();
        }
    }
}

public class AllQuestsJsonData
{
    public QuestJsonData[] Data { get; set; }
    public int RegularCompletedAmount { get; set; }
    public int AdditionalCompletedAmount { get; set; }
}