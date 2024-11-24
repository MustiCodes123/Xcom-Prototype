using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemShopView : UIWindowView
{
    [SerializeField] private ItemShopCard itemShopCard;

    [SerializeField] private Transform itemShopCardContainer;
    [SerializeField] private Button resetItemsButton;
    [SerializeField] private TextMeshProUGUI nextTimeUpdate;

    [SerializeField] private SkillShopItem skillCard;
    [SerializeField] private Transform skillsParent;


    [SerializeField] private PurchaseCard purchaseCard;
    [SerializeField] private Transform purchaseParent;

    [SerializeField] private GameObject skillShopGO;
    [SerializeField] private GameObject itemShopGO;
    [SerializeField] private GameObject purchaseGO;

    private List<ItemShopCard> allItemShopCards = new List<ItemShopCard>();
    private List<PurchaseCard> allPurchaseCards = new List<PurchaseCard>();

    private MoneyChangeHandler moneyChangeHandler;
    private SaveManager saveManager;
    private ShopHundler shopHundler;
    private ShopController shopController;
    private Tooltip tooltip;
    private SkillsDataInfo skillsDataInfo;

    [Inject]
    public void Construct(PlayerData playerData, ShopController shopController, MoneyChangeHandler moneyChangeHandler, SaveManager saveManager, ShopHundler shopHundler, Tooltip Tooltip, SkillsDataInfo skillsDataInfo)
    {
        this.shopController = shopController;
        this.playerData = playerData;
        this.moneyChangeHandler = moneyChangeHandler;
        this.saveManager = saveManager;
        this.shopHundler = shopHundler;
        this.tooltip = Tooltip;
        this.skillsDataInfo = skillsDataInfo;
    }


    protected override void Awake()
    {
        base.Awake();
        CreateShop();
        CreateShopSkills();
        CreatePurchase();
        resetItemsButton.onClick.AddListener(ResetShopItems);
    }

    private void Update()
    {
        var timeSpan = (shopController.nextRefreshTime - DateTime.UtcNow);

        if(timeSpan.TotalMilliseconds <= 0)
        {
            shopController.RefreshShop(false);
        }
        nextTimeUpdate.text = "refresh after: " + string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    private void ResetShopItems()
    {
        shopController.RefreshShop();
    }

    override public void Show()
    {
        transform.DOKill(true);
        gameObject.SetActive(true);

        shopController.CheckTime();
        ShowItems();
    }

    override public void Hide()
    {
        transform.DOKill(true);
        saveManager.SaveGame();
        gameObject.SetActive(false);
    }

    private void CreateShop()
    {
        var items = shopController.ShopItems;

        for (int i = 0; i < allItemShopCards.Count; i++)
        {
            allItemShopCards[i].gameObject.SetActive(false);
        }

        bool isFirstLockedItem = true;

        for (int i = 0; i < items.Count; i++)
        {
            if (allItemShopCards.Count <= i)
            {
                var newItemShopCard = Instantiate(itemShopCard, itemShopCardContainer);
                newItemShopCard.SetItem(items[i], shopHundler);
                newItemShopCard.gameObject.SetActive(true);
                allItemShopCards.Add(newItemShopCard);
            }
            else
            {
                allItemShopCards[i].gameObject.SetActive(true);
                allItemShopCards[i].SetItem(items[i], shopHundler);
            }

            if ((shopController.UnlockedItems + shopController.startUnlockedItems) <= i)
            {
                allItemShopCards[i].SetLocked(isFirstLockedItem);
                allItemShopCards[i].SubscribeToClick(() =>
                {
                    shopController.UnlockNewSlot();
                    CreateShop();
                });
                isFirstLockedItem = false;
            }
        }
    }

    private void CreateShopSkills()
    {
        var skills = shopController.ShopSkills;

        for (int i = 0; i < skills.Count; i++)
        {
            var newSkillCard = Instantiate(skillCard, skillsParent);
            newSkillCard.SetItem(skills[i], shopHundler);
            newSkillCard.SetLocked(false);
            newSkillCard.SubscribeToClick(() =>
            {

            });
            newSkillCard.gameObject.SetActive(true);
        }
    }

    private void CreatePurchase()
    {
        var purchaseCards = shopController.PurchaseItems;
        
        for (int i = 0; i < purchaseCards.Count; i++)
        {
            var newPurchaseCard = Instantiate(purchaseCard, purchaseParent);
            newPurchaseCard.SetItem(purchaseCards[i], OnItemBought);
            newPurchaseCard.gameObject.SetActive(true);
            allPurchaseCards.Add(newPurchaseCard);
        }
    }

    private void OnItemBought(PurchaseItem item)
    {
        Debug.Log("Shop view OnItemBought");
        shopController.ProcessPurchaseItem(item);
    }

    public void ShowItems()
    {
        skillShopGO.SetActive(false);
        itemShopGO.SetActive(true);
        purchaseGO.SetActive(false);
        _signalBus.Fire(new ShopWindowOpenSignal() { ShopTab = ShopTabEnum.Items });
    }

    public void ShowSkills()
    {
        skillShopGO.SetActive(true);
        itemShopGO.SetActive(false);
        purchaseGO.SetActive(false);
        _signalBus.Fire(new ShopWindowOpenSignal() { ShopTab = ShopTabEnum.Skills });
    }

    public void ShowPurchase()
    {
        skillShopGO.SetActive(false);
        itemShopGO.SetActive(false);
        purchaseGO.SetActive(true);
        _signalBus.Fire(new ShopWindowOpenSignal() { ShopTab = ShopTabEnum.Exchange });
    }
}
