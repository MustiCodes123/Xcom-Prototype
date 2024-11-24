using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class TemporaryQuestWindowView : UIWindowView
{
    [SerializeField] private Transform questsParent;
    [SerializeField] private Transform temporaryRewardParent;
    [SerializeField] private QuestCard questPrefab;
    [SerializeField] private QuestCard temporaryRewardPrefab;
    [SerializeField] private QuestCard temporaryRewardEmptyPrefab;
    [SerializeField] private QuestTimer timer;
    [SerializeField] private WalletView _walletView;
    [SerializeField] private Button dailyQuest;
    [SerializeField] private Button weekQuest;
    [SerializeField] private Button monthQuest;

    [Inject] private QuestManager questManager;
    [Inject] private ItemsDataInfo itemsDataInfo;

    [SerializeField] private Button _currentButton;
    private List<QuestCard> dailyCards = new List<QuestCard>();
    private List<QuestCard> weekCards = new List<QuestCard>();
    private List<QuestCard> monyhlyCards = new List<QuestCard>();
    private List<QuestCard> temporaryRewardQuestsCards = new List<QuestCard>();

    private int _completedDaily;
    private int _completedWeek;
    private int _completedMonth;

    private int _dailyIndex = 0;
    private int _weekIndex = 1;
    private int _monthIndex = 2;

    private void Start()
    {
        InitQuests();
        _currentButton = dailyQuest;
        dailyQuest.onClick.AddListener(OnDailyQuestClick);
        weekQuest.onClick.AddListener(OnWeekQuestClick);
        monthQuest.onClick.AddListener(OnMounthQuestClick);
    }

    public override void Show()
    {
        ResetQuestsWindow();
        base.Show();
        _currentButton.Select();
    }

    public void InitQuests()
    {
        InitTemporaryQuests();

        InitTemporaryRewardQuest();

        OnDailyQuestClick();
        dailyQuest.Select();
    }

    public void ResetQuestsWindow()
    {
        DestroyCards(GetAllQuestCards());

        GetAllQuestCards().Clear();
        dailyCards.Clear();
        weekCards.Clear();
        monyhlyCards.Clear();
        temporaryRewardQuestsCards.Clear();

        _completedDaily = 0;
        _completedWeek = 0;
        _completedMonth = 0;

        InitQuests();
    }

    private void InitTemporaryQuests()
    {
        CreateQuestCard(questManager.DailyQuests, dailyCards, questPrefab, questsParent, QuestTypeEnum.Daily, ref _completedDaily);
        CreateQuestCard(questManager.WeeklyQuests, weekCards, questPrefab, questsParent, QuestTypeEnum.Weekly, ref _completedWeek);
        CreateQuestCard(questManager.MonthQuests, monyhlyCards, questPrefab, questsParent, QuestTypeEnum.Monthly, ref _completedMonth);
    }

    private void InitTemporaryRewardQuest()
    {
        for (int i = 0; i < questManager.TemporaryRewardQuests.Count; i++)
        {
            if (!questManager.TemporaryRewardQuests[i].IsRewardTaken && questManager.TemporaryRewardQuests[i] is TemporaryReward temporaryReward)
            {
                temporaryReward.SetupTeporaryRewardGoal(questManager.GetListByType(temporaryReward.QuestType).Count, GetCompletedQuestsCountByType(temporaryReward.QuestType));
                temporaryRewardQuestsCards.Add(CreateQuestCard(questManager.TemporaryRewardQuests[i], temporaryRewardPrefab, temporaryRewardParent));
            }
            else
            {
                temporaryRewardQuestsCards.Add(CreateQuestCard(questManager.TemporaryRewardQuests[i], temporaryRewardEmptyPrefab, temporaryRewardParent));
            }
        }
    }

    private void CreateQuestCard(List<Quest> quests, List<QuestCard> questCards, QuestCard prefab, Transform parent, QuestTypeEnum questType, ref int completeCount)
    {
        for (int i = 0; i < quests.Count; i++)
        {
            if (!quests[i].IsRewardTaken)
            {
                 questCards.Add(CreateQuestCard(quests[i], prefab, parent));
            }
            else
            {
                completeCount++;
            }

            if (quests[i] is TemporaryQuest temporaryQuest)
            {
                temporaryQuest.SetupTemporaryGoal();
            }
                _signalBus.Fire(new QuestCompleteSignal() { QuestCount = completeCount, QuestType = questType });
        }
    }

    private void OnDailyQuestClick()
    {
        HideAllCards();
        ShowQuests(dailyCards);

        temporaryRewardQuestsCards[_dailyIndex].gameObject.SetActive(true);
        timer.ShowDaylyTimer();
        _currentButton = dailyQuest;
    }

    private void OnMounthQuestClick()
    {
        HideAllCards();
        ShowQuests(monyhlyCards);

        temporaryRewardQuestsCards[_monthIndex].gameObject.SetActive(true);
        timer.ShowMonthlyTimer();
        _currentButton = monthQuest;
    }

    private void OnWeekQuestClick()
    {
        HideAllCards();
        ShowQuests(weekCards);

        temporaryRewardQuestsCards[_weekIndex].gameObject.SetActive(true);
        timer.ShowWeeklyTimer();
        _currentButton = weekQuest;
    }

    private QuestCard CreateQuestCard(Quest x, QuestCard cardPrefab, Transform parent)
    {
        QuestCard card = Instantiate(cardPrefab, parent);
        card.Init(x, playerData, _walletView, this, _saveManager);

        return card;
    }

    private void DestroyCards(List<QuestCard> cards)
    {
        foreach (QuestCard card in cards)
        {
            Destroy(card.gameObject);
        }
    }

    private void ShowQuests(List<QuestCard> questCards)
    {
        for (int i = 0; i < questCards.Count; i++)
        {
            questCards[i].gameObject.SetActive(true);
        }
    }

    private void HideQuests(List<QuestCard> questCards)
    {
        for (int i = 0; i < questCards.Count; i++)
        {
            questCards[i].gameObject.SetActive(false);
        }
    }

    private List<QuestCard> GetAllQuestCards()
    {
        var allCards = new List<QuestCard>();
        allCards.AddRange(dailyCards);
        allCards.AddRange(weekCards);
        allCards.AddRange(monyhlyCards);
        allCards.AddRange(temporaryRewardQuestsCards);
        
        return allCards;
    }

    private void HideAllCards()
    {
        foreach (QuestCard card in GetAllQuestCards())
        {
            card.gameObject.SetActive(false);
        }
    }

    private int GetCompletedQuestsCountByType(QuestTypeEnum questType)
    {
        switch (questType)
        {
            case QuestTypeEnum.Daily:
                return _completedDaily;
            case QuestTypeEnum.Weekly:
                return _completedWeek;
            case QuestTypeEnum.Monthly:
                return _completedMonth;
            case QuestTypeEnum.TeporaryRewardDaily:
                return _completedDaily;
            case QuestTypeEnum.TeporaryRewardWeekly:
                return _completedWeek;
            case QuestTypeEnum.TeporaryRewardMonthly:
                return _completedMonth;
            default: 
                return 0;
        }
    }


    private void OnDestroy()
    {
        dailyQuest.onClick.RemoveAllListeners();
        weekQuest.onClick.RemoveAllListeners();
        monthQuest.onClick.RemoveAllListeners();
    }
}