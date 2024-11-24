using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemShopCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
     protected BaseItem item;
     protected ItemTemplate itemTemplate;

    [SerializeField] protected Button buyButton;
    [SerializeField] protected Button unlockButton;
    [SerializeField] protected GameObject unlockGO;
    [SerializeField] protected Image itemImage;
    [SerializeField] protected Image rareImage;
    [SerializeField] protected TextMeshProUGUI itemCountText;
    [SerializeField] protected TextMeshProUGUI itemPriceText;
    [SerializeField] protected TextMeshProUGUI itemNameText;

    private Action OnSlotUnlocked;
    private ShopHundler shopHundler;

    private void Awake()
    {
        if (buyButton != null)
          buyButton.onClick.AddListener(BuyItem);

        if (unlockButton != null)
            unlockButton.onClick.AddListener(UnlockItem);
    }

    private void UnlockItem()
    {
        OnSlotUnlocked?.Invoke();
    }

    private void BuyItem()
    {
        if (shopHundler.TryBoughtItem(item))
        {
            gameObject.SetActive(false);
        }
    }

    public void SubscribeToClick(Action onSlotUnlocked)
    {
        OnSlotUnlocked = onSlotUnlocked;

    }

    public void SetLocked(bool availableUnlock)
    {
        buyButton.gameObject.SetActive(false);
        unlockButton.gameObject.SetActive(availableUnlock);
        unlockGO.SetActive(true);
    }
    
    public void SetItem(BaseItem item, ShopHundler shopHundler)
    {
        if (unlockGO != null)
            buyButton.gameObject.SetActive(true);

        if(unlockGO != null)
            unlockGO.SetActive(false);

        this.shopHundler = shopHundler;
        this.item = item;
        itemPriceText.text ="Price: " + item.itemPrice.ToString();
        itemImage.sprite = item.itemSprite;

        rareImage.color = ItemsDataInfo.Instance.RareColors[(int)item.Rare];

        if(itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        if (item.itemCount == 1 || item.itemCount == 0)
        {
            itemCountText.text = "";
        }
        else
        {
            itemCountText.text = item.itemCount.ToString();
        }
    }

    public void SetItemTemplate(ItemTemplate item)
    {
        itemTemplate = item;
        gameObject.SetActive(true);
        itemPriceText.text = "Price: " + item.itemPrice.ToString();
        itemImage.sprite = item.itemSprite;
        itemCountText.text = "";
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            Tooltip.Instance.ShowTooltip(item, eventData.position);
        }
        else if(itemTemplate != null)
        {
            Tooltip.Instance.ShowTooltip(itemTemplate, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }

    private void OnDestroy()
    {
        buyButton.onClick.RemoveAllListeners();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InfoPopup.Instance.ActivateButtons("", "Ok", null, null);

        if (item != null)
        {
            InfoPopup.Instance.ShowTooltip(item);
        }
        else if (itemTemplate != null)
        {
            InfoPopup.Instance.ShowTooltip(itemTemplate);
        }
    }
}
