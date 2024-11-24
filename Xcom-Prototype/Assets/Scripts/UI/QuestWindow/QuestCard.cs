using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDescription;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI skipPriceView;

    [SerializeField] private Button takeButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Slider progressBar;

    [SerializeField] private BaseDragableItem itemCardView;
    [SerializeField] private Transform itemCardContainer;
    [SerializeField] private List<BaseDragableItem> rewardItems = new List<BaseDragableItem>();

    private Quest quest;
    private PlayerData playerData;
    private WalletView _walletView;
    private QuestWindowView questWindow;
    private TemporaryQuestWindowView tempQuestWindow;
    private SaveManager _saveManager;

    private void OnEnable()
    {
        if (quest != null && playerData != null && _walletView != null && questWindow != null && _saveManager != null)
        {
            Init(quest, playerData, _walletView, questWindow, _saveManager);
        }

        takeButton.onClick.AddListener(OnTakeButtonClick);
        skipButton.onClick.AddListener(OnSkipButtonClick);
    }

    private void OnDisable()
    {
        takeButton.onClick.RemoveListener(OnTakeButtonClick);
        skipButton.onClick.RemoveListener(OnSkipButtonClick);
    }

    public void Init(Quest quest, PlayerData playerData, WalletView resourcePanel, UIWindowView questWindow, SaveManager saveManager)
    {
        this.quest = quest;
        this.playerData = playerData;
        _walletView = resourcePanel;
        _saveManager = saveManager;

        if (questWindow is  QuestWindowView permanentQusetWindow)
        {
            this.questWindow = permanentQusetWindow;
        }
        if (questWindow is TemporaryQuestWindowView temporaryQuestWindow)
        {
            this.tempQuestWindow = temporaryQuestWindow;
        }

        questDescription.text = quest.Description;
        if (quest.RewardItem != null)
        {
            for (int i = 0; i < quest.RewardItem.Count; i++)
            {
                if (quest.RewardItem.Count > rewardItems.Count)
                {
                    BaseDragableItem itemCard = Instantiate(itemCardView, itemCardContainer);
                    itemCard.SetItem(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(quest.RewardItem[i]), null);
                    rewardItems.Add(itemCard);
                    if (itemCard.Item.Slot != SlotEnum.Weapon && itemCard.Item.Slot != SlotEnum.Chest && itemCard.Item.Slot != SlotEnum.Gloves && itemCard.Item.Slot != SlotEnum.Legs)
                    {
                        itemCard.ShowItemPrise();
                    }
                }
            }
        }

        if(quest.CristalRewards != null)
        {
            for (int i = 0; i < quest.CristalRewards.Length; i++)
            {
                if (quest.CristalRewards.Length > (itemCardContainer.childCount - rewardItems.Count))
                {
                    BaseDragableItem itemCard = Instantiate(itemCardView, itemCardContainer);
                    itemCard.SetCristal(quest.CristalRewards[i]);
                }
            }
        }

        takeButton.gameObject.SetActive(quest.IsCompleted);
        skipButton.gameObject.SetActive(!quest.IsCompleted);
        countDescription.text = quest.Goals[0].CurrentAmount + "/" + quest.Goals[0].RequiredAmount+ "  ";
        skipPriceView.text = quest.QuestSkipPriceData.Price.ToString();

        SetQuestprogressBar();
    }

    private void OnTakeButtonClick()
    {
        if (quest != null)
        {
            quest.TakeReward(playerData, _walletView);

            if (questWindow != null)
            {
                questWindow.CountQuest();
                questWindow.ResetQuestsWindow();
            }
            if (tempQuestWindow != null)
            {
                tempQuestWindow.ResetQuestsWindow();
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Quest is null when attempting to take reward.");
        }

        _saveManager.SaveGame();
    }
    
    private void OnSkipButtonClick()
    {
        bool result = Wallet.Instance.SpendCachedCurrency(quest.QuestSkipPriceData.Currency, (uint)quest.QuestSkipPriceData.Price);

        if(result)
        {
            quest.IsCompleted = true;
            OnTakeButtonClick();

            if (questWindow != null)
            {
                questWindow.CountQuest();
                questWindow.ResetQuestsWindow();
            }
            if (tempQuestWindow != null)
            {
                tempQuestWindow.ResetQuestsWindow();
            }
        }
        else
        {
            Debug.Log("Shop popup");
        }
    }

    private void SetQuestprogressBar()
    {
       progressBar.maxValue = this.quest.Goals[0].RequiredAmount;
       progressBar.value = this.quest.Goals[0].CurrentAmount;
    }
}
