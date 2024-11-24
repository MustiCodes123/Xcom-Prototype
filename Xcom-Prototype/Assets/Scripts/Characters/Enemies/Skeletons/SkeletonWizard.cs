using UnityEngine;

namespace Characters.Enemies.Sceletons
{
    public class SkeletonWizard : Skeleton
    {
        [SerializeField] private RangeWeaponView _itemView;

        public override void SetupAnimation(Animator animator)
        {
            if (animator != null)
                animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Wand);
        }
        
        
        protected override void SetInventory()
        {
            View.SetRangeWeapon(_itemView);
        }
    }
}