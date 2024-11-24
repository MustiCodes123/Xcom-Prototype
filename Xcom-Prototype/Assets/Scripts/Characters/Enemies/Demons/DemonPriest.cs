using UnityEngine;

public class DemonPriest : BaseDemon
{
    [SerializeField] private RangeWeaponView _rangeWeaponView;
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Staff);
    }

    public override void SetInventory()
    {
        View.SetRangeWeapon(_rangeWeaponView);
    }
}