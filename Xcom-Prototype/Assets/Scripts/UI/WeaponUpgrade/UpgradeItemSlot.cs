using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeItemSlot : BaseItemSlot, IDropHandler
{
    [SerializeField] private WeaponUpgradeWindow _weaponUpgradeWindow;
    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        BaseDragableItem dragableItem = eventData.pointerDrag.GetComponent<BaseDragableItem>();

        _weaponUpgradeWindow.SetItemInHorn(dragableItem.Item, this);
    }

    public void SetItemToUpgrade(BaseItem item, Action<BaseItem, BaseItemSlot> setInHorn)
    {
        setInHorn.Invoke(item, this);
    }
}