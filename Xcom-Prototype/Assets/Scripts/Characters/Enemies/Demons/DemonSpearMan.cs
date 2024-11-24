using UnityEngine;

public class DemonSpearMan : BaseDemon
{
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Spear);
    }
}