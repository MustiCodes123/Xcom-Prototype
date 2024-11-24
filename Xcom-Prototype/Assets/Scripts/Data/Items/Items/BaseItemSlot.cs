using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class BaseItemSlot : MonoBehaviour, IDropHandler
{
    public Action<BaseItem, BaseItemSlot> OnClickAction;
    public Action OnEquip;
    public BaseItem Item { get; set; }

    public bool IsDragable = true;
    public int SlotID;
    public int ContainerID;
    
    public SlotEnum slotType;

    [SerializeField] private Image _outlineImage;
    [SerializeField] private Image _background;
    [SerializeField] private UpgradeSlotPrice _priceBlock;

    protected BaseDragableItem CurrentDragableItem;
    protected CharacterHandler CharacterHandler;    

    private BaseDragableItem _dragableItemPrefab;
    private Tooltip _tooltip;

    [Inject]
    public void Construct(BaseDragableItem dragableItemPrefab, CharacterHandler characterHandler, Tooltip tooltip)
    {
        this._dragableItemPrefab = dragableItemPrefab; 
        this.CharacterHandler = characterHandler;
        this._tooltip = tooltip;
        IsDragable = true;
    }

    public virtual void SetItem(BaseItem item, bool needCreateNew = true, Action<BaseItem, BaseItemSlot> action = null)
    {
        if (_outlineImage) _outlineImage.gameObject.SetActive(false);

        if (item == null || CurrentDragableItem != null) return;
        
        if (action != null) OnClickAction = action;

        Item = item;

        if (_outlineImage)
        {
            _outlineImage.gameObject.SetActive(true);
        }

        if (needCreateNew)
        {
            CurrentDragableItem = Instantiate(_dragableItemPrefab, transform);
            CurrentDragableItem.Construct(CharacterHandler);
            CurrentDragableItem.SetItem(item, this, OnClickAction);
            CurrentDragableItem.transform.SetAsLastSibling();
        }

        OnEquip?.Invoke();
    }

    public bool IsFreeSlot() => CurrentDragableItem == null;

    public BaseDragableItem GetItem() => CurrentDragableItem;

    public virtual void Reset()
    {
        Item = null;

        if (_outlineImage) _outlineImage.gameObject.SetActive(false);

        if (CurrentDragableItem) Destroy(CurrentDragableItem.gameObject);

        CurrentDragableItem = null;

        _tooltip.HideTooltip();

        if (_priceBlock != null)
        {
            _priceBlock.gameObject.SetActive(false);
        }
    }

    public virtual void ClearSlot()
    {
        Item = null;
        CurrentDragableItem = null;
        if (_outlineImage) _outlineImage.gameObject.SetActive(false);
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        BaseDragableItem dragableItem = eventData.pointerDrag.GetComponent<BaseDragableItem>();
                       
        if (dragableItem != null && dragableItem.Item != null )
        {
            var fromSlot = dragableItem.CurrentItemSlot;

            if (IsSuitableSlot(dragableItem))
            {
                if (CurrentDragableItem != null)
                {
                    CharacterHandler.UnequipItem(CurrentDragableItem.Item);
                }

                ShowEquipPopup(dragableItem, fromSlot);
                dragableItem.CurrentItemSlot.UpdateInventory();
            }
            else if (CurrentDragableItem != null && CurrentDragableItem.Item.Slot == dragableItem.Item.Slot && CurrentDragableItem.Item.isConsumable)
            {
                int count = CurrentDragableItem.Item.itemCount + dragableItem.Item.itemCount;

                if (count > CurrentDragableItem.Item.itemMaxStack)
                {
                    CurrentDragableItem.Item.itemCount = CurrentDragableItem.Item.itemMaxStack;
                    dragableItem.Item.itemCount = count - CurrentDragableItem.Item.itemMaxStack;
                    CurrentDragableItem.UpdateInfo();
                    dragableItem.UpdateInfo();
                    AfterDrop(fromSlot, this);
                }
                else
                {
                    dragableItem.CurrentItemSlot.ClearSlot();
                    CurrentDragableItem.Item.itemCount = count;
                    Destroy(dragableItem.gameObject);
                    AfterDrop(fromSlot, this);
                }
            }
        } 
    }

    public virtual void AfterDrop(BaseItemSlot slotFrom, BaseItemSlot slotTo)
    {
        InventoryItemSlot inventoryItemSlot = slotFrom as InventoryItemSlot;
        if (inventoryItemSlot != null)
        {
            CharacterHandler.UnequipItem(CurrentDragableItem.Item);

            if (inventoryItemSlot is WeaponInventorySlot weaponSlot)
            {
                weaponSlot.RelieveTwoHand();
            }
            else if (inventoryItemSlot is OffHandInventorySlot shield) 
            {
                shield.UnequipOffHand();
            }
        }

        _tooltip.HideTooltip();

        OnEquip?.Invoke();
    }

    public void ShowPrice(Sprite priceImage, GameCurrencies type, uint count)
    {
        if (_priceBlock != null)
        {
            _priceBlock.gameObject.SetActive(true);
            _priceBlock.SetPrice(priceImage, type, count);
        }
    }

    public void HidePrice()
    {
        if (_priceBlock != null)
        {
            _priceBlock.gameObject.SetActive(false);
        }
    }

    public virtual bool IsSuitableSlot(BaseDragableItem dregableItem)
    {
        if (slotType == dregableItem.Item.Slot)
        {
            return true;
        }
        else
            return false;
    }

    public virtual bool TwoHandedOnDrop(BaseDragableItem dregableItem)
    {
        return false;
    }

    public virtual void UpdateInventory()
    {
    }

    public void UpdateLevelCount()
    {
        CurrentDragableItem.UpdateLevelInfo();
    }

    private void ShowEquipPopup(BaseDragableItem dragableItem, BaseItemSlot fromSlot)
    {
        ItemInfoPopup.Instance.Show(dragableItem.Item);
        ItemInfoPopup.Instance.ActivateButtons("Equip", () =>
        {
            OnEquip?.Invoke();
            if (dragableItem.Item != null)
            {
                dragableItem.CurrentItemSlot.ClearSlot();
                dragableItem.CurrentItemSlot = this;
                SetItem(dragableItem.Item, false);
                CurrentDragableItem = dragableItem;
                CurrentDragableItem.SetActiveOutline(false);
                dragableItem.parentTransform = transform;
                AfterDrop(fromSlot, this);
                ItemInfoPopup.Instance.gameObject.SetActive(false);
            }
        },
        () =>
        {
            ItemInfoPopup.Instance.gameObject.SetActive(false);
        });
    }

    public BaseDragableItem GetDragableItem() => CurrentDragableItem;
}
