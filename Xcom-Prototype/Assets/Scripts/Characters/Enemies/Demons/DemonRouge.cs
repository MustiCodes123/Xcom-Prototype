using UnityEngine;

public class DemonRouge : BaseDemon
{
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Dual);
    }
}