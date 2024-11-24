using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Button buyButton;
    [SerializeField] protected Button unlockButton;
    [SerializeField] protected GameObject unlockGO;
    [SerializeField] protected Image itemImage;

    [SerializeField] protected TextMeshProUGUI itemCountText;
    [SerializeField] protected TextMeshProUGUI itemPriceText;
    [SerializeField] protected TextMeshProUGUI itemNameText;

    private Action OnSlotUnlocked;
    private ShopHundler shopHundler;

    private BaseSkillTemplate itemTemplate;

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
        if (shopHundler.TryBoughtSkill(itemTemplate))
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
        buyButton.gameObject.SetActive(!availableUnlock);
        unlockButton.gameObject.SetActive(availableUnlock);
        unlockGO.SetActive(availableUnlock);
    }

    public void SetItem( BaseSkillTemplate itemTemplate, ShopHundler shopHundler)
    {
        this.itemTemplate = itemTemplate;
        this.shopHundler = shopHundler;

        itemImage.sprite = itemTemplate.Icon;
        itemNameText.text = itemTemplate.Id.ToString();
        itemPriceText.text = itemTemplate.BuyPrice.ToString();
        buyButton.gameObject.SetActive(true);
        unlockGO.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Instance.ShowTooltip(itemTemplate, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }
}   

