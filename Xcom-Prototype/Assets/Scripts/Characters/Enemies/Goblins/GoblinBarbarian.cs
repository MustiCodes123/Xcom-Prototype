using System;
using System.Collections;
using UnityEngine;

public class GoblinBarbarian : Goblin
{
    public override void SetupAnimation(Animator animator)
    {
        if (Animator != null)
            animator.SetInteger("WeaponType", (int)WeaponTypeEnum.TwoHandedAxe);
    }
}
