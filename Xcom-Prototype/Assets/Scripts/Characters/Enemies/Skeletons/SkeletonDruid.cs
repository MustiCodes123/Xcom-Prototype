using UnityEngine;

namespace Characters.Enemies.Sceletons
{
    public class SkeletonDruid : Skeleton
    {
        public override void SetupAnimation(Animator animator)
        {
            if (animator != null)
                animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Spear);
        }
    }
}