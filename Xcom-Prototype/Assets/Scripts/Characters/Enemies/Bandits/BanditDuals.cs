using System.Collections;
using UnityEngine;

public class BanditDuals: Bandit
{
    public override void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(weaponTypeParameterName, (int)WeaponTypeEnum.Dual);
    }
}