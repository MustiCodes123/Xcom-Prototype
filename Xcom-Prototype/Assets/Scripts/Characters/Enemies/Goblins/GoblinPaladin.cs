using System.Collections;
using UnityEngine;
public class GoblinPaladin : Goblin
{
    public override void SetupAnimation(Animator animator)
    {
        if (Animator != null)
            animator.SetInteger("WeaponType", (int)WeaponTypeEnum.Shield);
    }
}
