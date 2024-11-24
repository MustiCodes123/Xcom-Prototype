using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class QuestWindowView : UIWindowView
{
    [SerializeField] private Transform questsParent;
    [SerializeField] private QuestCard questPrefab;
    [SerializeField] private WalletView _walletView;
    [SerializeField] private AdditionalRewardCard AdditionalRewardPrefab;
    [SerializeField] private AdditionalRewards additionalRewardPanel;
    [SerializeField] private QuestTimer timer;
    
    [Inject] private QuestManager questManager;
    [Inject] private SignalBus signalBus;

    private List<QuestCard> RegularCards = new List<QuestCard>();

    private void Start()
    {
        InitQuests();
    }

    public AdditionalRewardCard CreateAdditionalRewardCard(Quest x, Transform parent)
    {
        AdditionalRewardCard card = Instantiate(AdditionalRewardPrefab, parent);
        card.Init(x, playerData, _walletView, this, _saveManager);

        return card;
    }

    private QuestCard CreateQuestCard(Quest x)
    {
        QuestCard card = Instantiate(questPrefab, questsParent);
        card.Init(x, playerData, _walletView, this, _saveManager);

        return card;
    }

    public override void Show()
    {
        base.Show();
    }

    public void InitQuests()
    {
        Debug.Log($"Called InitQuests");

        foreach (var quest in questManager.RegularQuests)
        {
            if (!quest.IsCompleted && !quest.IsRewardTaken)
            {
                RegularCards.Add(CreateQuestCard(quest));
            }
            else if (quest.IsCompleted && !quest.IsRewardTaken)
            {
                RegularCards.Add(CreateQuestCard(quest));
            }
        }
        
        additionalRewardPanel.ShowAdditionalRewards(questManager);
        additionalRewardPanel.UpdateAdditionalRewardProgress();
        ShowQuests(RegularCards);
    }

    public void ResetQuestsWindow()
    {
        foreach (QuestCard card in RegularCards)
        {
            Destroy(card.gameObject);
        }
        
        RegularCards.Clear();
        additionalRewardPanel.DestroyAdditionalQuests();
        additionalRewardPanel.ShowAdditionalRewards(questManager);
        
        if (AreAllQuestsRewardsTaken())
        {
            timer.ShowWeeklyTimer();
        }
        else
        {
            InitQuests();
        }
    }

    private void ShowQuests(List<QuestCard> questCards)
    {
        for (int i = 0; i < questCards.Count; i++)
        {
            questCards[i].gameObject.SetActive(true);
        }
    }

    public void CountQuest()
    {
        additionalRewardPanel.RegularCompletedQuestCount++;
        signalBus.Fire(new QuestCompleteSignal() { QuestCount = additionalRewardPanel.RegularCompletedQuestCount, QuestType = QuestTypeEnum.Permanent });
        additionalRewardPanel.UpdateAdditionalRewardProgress();
    }

    private bool AreAllQuestsRewardsTaken()
    {
        foreach (var quest in questManager.RegularQuests)
        {
            if (!quest.IsRewardTaken)
            {
                return false;
            }
        }
        return true;
    }
}
