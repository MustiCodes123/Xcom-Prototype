using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeaponView : DropItemView
{
    public void SetWeapon(GameObject weaponObject)
    {
        weaponObject.transform.SetParent(_itemViewBox.transform);
    }
}
