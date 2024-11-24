using UnityEngine;

namespace Characters.Enemies.Sceletons
{
    public class SkeletonRanger : Skeleton
    {
        [SerializeField] private RangeWeaponView _itemView;
        public override void SetupAnimation(Animator animator)
        {
            if (animator != null)
                animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Bow);
        }

        protected override void SetInventory()
        {
            View.SetRangeWeapon(_itemView);
        }
    }
}