using System.Collections;
using UnityEngine;

public class BanditMage : Bandit
{
    [SerializeField] private RangeWeaponView _rangeWeaponView;
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(weaponTypeParameterName, (int)WeaponTypeEnum.Staff);
    }

    protected override void SetInventory()
    {
        View.SetRangeWeapon(_rangeWeaponView);
    }
}