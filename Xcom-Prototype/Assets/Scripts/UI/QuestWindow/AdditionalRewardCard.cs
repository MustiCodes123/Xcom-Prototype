using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AdditionalRewardCard : MonoBehaviour
{
    public Quest Quest { get; private set; }
    
    [SerializeField] private TextMeshProUGUI currentGoal;
    [SerializeField] private TextMeshProUGUI maxGoal;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private Image pointBG;
    [SerializeField] private Image slotBG;
    [SerializeField] private Sprite activeRewardBG;
    [SerializeField] private Sprite noActiveRewardBG;
    [SerializeField] private BaseDragableItem itemCardView;
    [SerializeField] private Transform itemCardContainer;
    [SerializeField] private Transform _rewardTakenFade;
    [SerializeField] private Button takeButton;
    [SerializeField] private WalletView _wallet;

    private PlayerData _playerData;
    private WalletView _walletView;
    private QuestWindowView _questWindow;
    private BaseDragableItem _itemCard;
    private SaveManager _saveManager;
    private QuestManager _questManager;

    private void Start()
    {
        takeButton.onClick.AddListener(OnTakeButtonClick);
    }

    private void OnTakeButtonClick()
    {
        InfoPopup.Instance.ShowTooltip(Quest);
        InfoPopup.Instance.ActivateButtons("Ok", "", TakeReward, null);
    }

    public void Init(Quest quest, PlayerData playerData, WalletView resourcePanel, QuestWindowView questWindow, SaveManager saveManager)
    {
        Quest = quest;
        _playerData = playerData;
        _questWindow = questWindow;
        _walletView = resourcePanel;
        _saveManager = saveManager;
        _questManager = saveManager.GetQuestManager();

        if (quest.RewardItem != null)
        {
            for (int i = 0; i < quest.RewardItem.Count; i++)
            {
                _itemCard = Instantiate(itemCardView, itemCardContainer);
                _itemCard.SetItem(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(quest.RewardItem[i]), null);
                _itemCard.DisableItemLvlText();
            }
        }
        
        AllQuestsJsonData questData = _questManager.GetSaveData();

        itemCount.text = quest.RewardItem[0].itemPrice.ToString();
        // currentGoal.text = quest.Goals[0].CurrentAmount.ToString();
        currentGoal.text = questData.RegularCompletedAmount.ToString();
        maxGoal.text = quest.Goals[0].RequiredAmount.ToString();

        if (quest.IsCompleted)
        {
            slotBG.sprite = activeRewardBG;
            pointBG.sprite = activeRewardBG;
            currentGoal.text = Math.Min(quest.Goals[0].RequiredAmount, questData.RegularCompletedAmount).ToString();
        }

        takeButton.gameObject.SetActive(quest.IsCompleted);
    }

    public void ActivateCard()
    {
        _rewardTakenFade.gameObject.SetActive(false);
        takeButton.gameObject.SetActive(true);
    }
    public void DeactivateCard()
    {
        _rewardTakenFade.gameObject.SetActive(true);
        takeButton.gameObject.SetActive(false);
    }

    private void TakeReward()
    {
        if (Quest != null)
        {
            Quest.TakeReward(_playerData, _walletView);
            if (_questWindow != null)
            {
                _questWindow.CountQuest();
                _questWindow.ResetQuestsWindow();
            }
            Debug.Log("Take");
            DeactivateCard();
        }
        else
        {
            Debug.LogError("Quest is null when attempting to take reward.");
        }

        _saveManager.SaveGame();
    }

    public void TakeAllRewards(Quest quest)
    {
        if (quest.IsCompleted && !quest.IsRewardTaken)
        {
            quest.TakeReward(_playerData, _walletView);
            DeactivateCard();
        }
    }

    public void ChangeBackGround()
    {
        _itemCard.ChangeRareBackground(activeRewardBG);
    }

}

