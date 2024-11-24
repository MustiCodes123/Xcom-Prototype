using UnityEngine;

namespace Characters.Enemies.Sceletons
{
    public class SkeletonPaladin : Skeleton
    {
        public override void SetupAnimation(Animator animator)
        {
            if (animator != null)
                animator.SetInteger(WeaponType, (int)WeaponTypeEnum.TwoHandedSword);
        }
    }
}