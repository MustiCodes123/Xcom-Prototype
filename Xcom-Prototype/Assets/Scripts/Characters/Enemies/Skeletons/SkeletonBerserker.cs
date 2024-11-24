using UnityEngine;

namespace Characters.Enemies.Sceletons
{
    public class SkeletonBerserker : Skeleton
    {
        public override void SetupAnimation(Animator animator)
        {
            if (animator != null)
                animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Dual);
        }
    }
}