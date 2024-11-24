using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Globalization;
using Zenject;
using System.Linq;

[Serializable]
public class QuestSkipPriceData
{
    [field: SerializeField] public int Price { get; private set; }
    public GameCurrencies Currency { get => GameCurrencies.Gem; }
}

public class Quest : MonoBehaviour
{
    [SerializeField] protected QuestTypeEnum _questType;

    public string Name;
    public string Description;
    public Sprite QuestIcon;
    [JsonIgnore] public string DefaultDescription;
    public int GoldReward;
    public int GemReward;
    public int ExpReward;
    public CristalData[] CristalRewards;
    public List<ItemTemplate> RewardItem;
    public List<QuestGoal> Goals;
    public bool IsCompleted;
    public bool IsRewardTaken;
    public DateTime QuestGenerationDate;
    public QuestSkipPriceData QuestSkipPriceData = new QuestSkipPriceData();

    public QuestTypeEnum QuestType => _questType;

    protected PlayerData _playerData;

    public virtual void Evaluate()
    {
        int completedGoals = 0;
        foreach (QuestGoal goal in Goals)
        {
            goal.Evaluate();

            if (goal.IsReached)
            {
                completedGoals++;
            }

        }

        IsCompleted = completedGoals >= Goals.Count;
    }

    public virtual void Process(ISignal signal)
    {
        for (int i = 0; i < Goals.Count; i++)
        {
            if (!Goals[i].IsReached)
            {
                Goals[i].Process(signal);
            }
        }

        Evaluate();
    }

    public async void TakeReward(PlayerData playerData, WalletView walletView)
    {
        _playerData = playerData;

        if (IsCompleted && !IsRewardTaken)
        {
            IsRewardTaken = true;

            foreach (ItemTemplate item in RewardItem)
            {
                if (item.Slot == SlotEnum.Weapon || item.Slot == SlotEnum.Chest || item.Slot == SlotEnum.Shield || item.Slot == SlotEnum.Head || item.Slot == SlotEnum.Legs || item.Slot == SlotEnum.Gloves || item.Slot == SlotEnum.Ring || item.Slot == SlotEnum.Amulet)
                {
                    _playerData.AddItemToInventory(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
                }

                if (item.Slot == SlotEnum.Money)
                {
                    _playerData.Money += item.itemPrice;
                    walletView.UpdateCurrency(GameCurrencies.Gold, item.itemPrice);
                }

                if (item.Slot == SlotEnum.GoldGem)
                {
                    _playerData.Gems += item.itemPrice;
                    walletView.UpdateCurrency(GameCurrencies.Gem, item.itemPrice);
                }

                foreach (var crystal in CristalRewards)
                {
                    await PlayerInventory.Instance.AddCristalToInventory(crystal.CristalEnum);
                }

                if (item.Slot == SlotEnum.Character && item is CharacterItemTemplate characterReward)
                {
                    _playerData.PlayerGroup.AddCharacterToNotAsignedGroup(characterReward.characterPreset.CreateCharacter());
                }
            }
        }
    }

    public void SetSavedData(string data)
    {
        string[] dataSplit = data.Split(';');
        string[] goals = dataSplit[0].Split('z');

        for (int i = 0; i < Goals.Count && i < goals.Length - 1; i++)
        {
            Goals[i].ParseString(goals[i]);
        }

        IsCompleted = JsonConvert.DeserializeObject<bool>(goals[goals.Length - 1]);
        IsRewardTaken = JsonConvert.DeserializeObject<bool>(dataSplit[1]);
    }

    public void LoadFromData(QuestJsonData data)
    {
        IsCompleted = data.IsCompleted;
        IsRewardTaken = data.IsRewardTaken;

        foreach (var goal in Goals)
        {
            if (data.GoalsData.TryGetValue(goal.GetStringValue(), out bool isReached))
            {
                goal.IsReached = isReached;
                if (isReached)
                {
                    goal.CurrentAmount = goal.RequiredAmount;
                }
            }
        }
    }

    public QuestJsonData GetSaveData()
    {
        Dictionary<string, bool> goalsData = new Dictionary<string, bool>();
        foreach (QuestGoal goal in Goals)
        {
            goalsData[goal.GetStringValue()] = goal.IsReached;
        }

        return new QuestJsonData
        {
            QuestName = Name,
            GoalsData = goalsData,
            IsCompleted = IsCompleted,
            IsRewardTaken = IsRewardTaken
        };
    }
    public virtual void ResetQuest()
    {
        foreach (QuestGoal goal in Goals)
        {
            goal.CurrentAmount = 0;
            goal.IsReached = false;
        }

        IsCompleted = false;
        IsRewardTaken = false;
    }

    protected int GetQuestTimeByType(QuestTypeEnum questType)
    {
        var week = ISOWeek.GetWeekOfYear(DateTime.Now);

        switch (questType)
        {
            case QuestTypeEnum.Daily:
                return DateTime.Now.Day;
            case QuestTypeEnum.Weekly:
                return week;
            case QuestTypeEnum.Monthly:
                return DateTime.Now.Month;
            case QuestTypeEnum.TeporaryRewardDaily:
                return DateTime.Now.Day;
            case QuestTypeEnum.TeporaryRewardWeekly:
                return week;
            case QuestTypeEnum.TeporaryRewardMonthly:
                return DateTime.Now.Month;
            default:
                return 0;
        }
    }
}

public class QuestJsonData
{
    public string QuestName { get; set; }
    public Dictionary<string, bool> GoalsData { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRewardTaken { get; set; }
}

