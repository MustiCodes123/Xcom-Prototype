using UnityEngine;

public class DemonBarbarian : BaseDemon
{
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(WeaponType, (int)WeaponTypeEnum.TwoHandedSword);
    }
}