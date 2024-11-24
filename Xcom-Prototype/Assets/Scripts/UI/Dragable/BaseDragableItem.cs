using System;
using Data.Resources.AddressableManagement;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class BaseDragableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static Action<bool> ItemDragStateChanged;

    public BaseItem Item;

    public BaseItemSlot CurrentItemSlot;
    [HideInInspector] public Transform parentTransform;

    [SerializeField] protected Image rareImage;
    [SerializeField] protected Image itemImage;
    [SerializeField] protected Image selfImage;
    [SerializeField] protected Image ownerImage;
    [SerializeField] protected Image frame;
    [SerializeField] protected Button cancelButton;
    [SerializeField] protected TextMeshProUGUI itemCountText;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected CharacterHandler _characterHandler;

    [SerializeField] private Image _outline;

    private Button _button;

    private Action<BaseItem, BaseItemSlot> _onItemClicked;

    [Inject]
    public void Construct(CharacterHandler characterHandler)
    {
        _characterHandler = characterHandler;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();

        if (_button != null)
        {
            _button.onClick.AddListener(OnItemClick);
        }
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void OnItemClick()
    {
        if (_onItemClicked != null)
        {
            _onItemClicked?.Invoke(Item, CurrentItemSlot);
        }
        else
        {
            if (Item != null)
            {
                InfoPopup.Instance.ActivateButtons("", "Ok", null, null);

                InfoPopup.Instance.ShowTooltip(Item);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Item != null)
        {
            Tooltip.Instance.ShowTooltip(Item, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }

    public void SetCristal(CristalData cristalData)
    {
        itemImage.sprite = cristalData.Sprite;
        itemCountText.text = cristalData.Name;
        Item.itemSprite = cristalData.Sprite;
        Item.itemName =  cristalData.Name;
        Item.itemDescription = cristalData.Description;
    }

    public void SetChest(ChestViewData chestData)
    {
        itemImage.sprite = chestData.Icon;
        Item.itemSprite = chestData.Icon;
        Item.itemName = chestData.Title;
        Item.itemDescription = chestData.Description;

    }

    public async void SetItem(BaseItem item, BaseItemSlot itemSlot, Action<BaseItem, BaseItemSlot> action = null)
    {
        if (action != null)
        {
            this._onItemClicked = action;
        }

        if (item == null)
        {
            SetEmptyItem();
            return;
        }

        if (_characterHandler != null && _characterHandler.IsItemEquiped(item))
        {
            rareImage.gameObject.SetActive(false);
            frame.gameObject.SetActive(false);
        }
        else
        {
            rareImage.gameObject.SetActive(true);
            frame.gameObject.SetActive(true);
        }

        rareImage.sprite = ItemsDataInfo.Instance.RareBackgrounds[(int)item.Rare];

        if(itemSlot != null)
        {
            CurrentItemSlot = itemSlot;
            SetActiveOutline(false);
        }

        Item = item;

        if (item.ResourceManager != null)
        {
            itemImage.sprite = item.ResourceManager.GetEmptySprite();
        }

        itemImage.sprite = await item.GetItemIcon();

        itemCountText.text = "lvl: " + (item.CurrentLevel).ToString();      
    }


    public void SetEmptyItem()
    {
        itemImage.sprite = null;
        Item = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurrentItemSlot == null || !CurrentItemSlot.IsDragable)
        {
            return;
        }

        parentTransform = rectTransform.parent;
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        selfImage.raycastTarget = false;
        CurrentItemSlot.UpdateInventory();

        ItemDragStateChanged?.Invoke(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CurrentItemSlot.IsDragable)
        {
            return;
        }

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CurrentItemSlot.IsDragable)
        {
            return;
        }

        transform.SetParent(parentTransform);
        rectTransform.localPosition = Vector3.zero;

        selfImage.raycastTarget = true;

        if (CurrentItemSlot != null && CurrentItemSlot.slotType != SlotEnum.None && CurrentItemSlot.slotType == Item.Slot)
        {

            ShowUnequipPopup(Item);
        }
        else
        {
            rareImage.gameObject.SetActive(true);
        }

        ItemDragStateChanged?.Invoke(false);
    }

    public void UpdateInfo()
    {
        itemCountText.text = Item.itemCount.ToString();
    }

    public void UpdateLevelInfo()
    {
        itemCountText.text = $"lvl: {Item.CurrentLevel}";
    }

    public void UpdateInventory()
    {
        if (CurrentItemSlot != null)
            CurrentItemSlot.UpdateInventory();
    }

    public void SetOwnerImage(BaseCharacterModel character, ResourceManager resourceManager)
    {
        ownerImage.gameObject.SetActive(true);
        rareImage.gameObject.SetActive(true);
        ownerImage.sprite = character.Avatar;
        ownerImage.sprite = resourceManager.LoadSprite(character.AvatarId);
    }

    public void DisableItemLvlText()
    {
        itemCountText.text = "";
    }

    public void ShowItemPrise()
    {
        itemCountText.text = Item.itemPrice.ToString();
    }

    public Button EnableCacelButton()
    {
        cancelButton.gameObject.SetActive(true);
        return cancelButton;
    }
    public bool HasOwner()
    {
        return ownerImage.gameObject.activeSelf;
    }

    public void ChangeRareBackground(Sprite image)
    {
        rareImage.sprite = image;
    }

    public void SetActiveOutline(bool value)
    {
        _outline.gameObject.SetActive(value);
    }

    private void ShowUnequipPopup(BaseItem item)
    {
        ItemInfoPopup.Instance.Show(item);
        ItemInfoPopup.Instance.ActivateButtons("Remove", () =>
        {
            if (item != null)
            {
                _characterHandler.UnequipItem(Item);
                CurrentItemSlot.UpdateInventory();
                ItemInfoPopup.Instance.gameObject.SetActive(false);
            }
        },
        () =>
        ItemInfoPopup.Instance.gameObject.SetActive(false));
    }
    
    
}