using System.Collections;
using UnityEngine;

public class BanditBow : Bandit
{
    [SerializeField] private RangeWeaponView _itemView;
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(weaponTypeParameterName, (int)WeaponTypeEnum.Bow);
    }

    protected override void SetInventory()
    {
        View.SetRangeWeapon(_itemView);
    }
}