using UnityEngine;

public class DemonMage : BaseDemon
{
    [SerializeField] private RangeWeaponView _itemView;
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Bow);
    }

    public override void SetInventory()
    {
        View.SetRangeWeapon(_itemView);
    }
}
