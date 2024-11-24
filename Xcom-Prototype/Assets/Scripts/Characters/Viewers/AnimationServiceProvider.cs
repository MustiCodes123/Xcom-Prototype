using UnityEngine;

public class AnimationServiceProvider : MonoBehaviour
{
    public BaseCharacerView CharacterView { get; set; }

    /// <summary>
    /// Invokes from animation clips
    /// </summary>
    public void Shoot()
    {
        CharacterView.CombatServiceProvider.Shoot();
    }

    /// <summary>
    /// Invokes from animation clips
    /// </summary>
    public void MeleeAttack()
    {
        CharacterView.CombatServiceProvider.MeleeAttack();
    }

    /// <summary>
    /// Invokes from animation clips
    /// </summary>
    public void ShieldHammerAttack()
    {
        CharacterView.CombatServiceProvider.ShieldHammerAttack();
    }
    /// <summary>
    /// Invokes from animation clips
    /// </summary>
    public void CritMeleeAttack()
    {
        CharacterView.CombatServiceProvider.CritMeleeAttack();
    }
}
