using UnityEngine;

public class FireMissile : BaseProjectile
{
    public override void SkillUpdate()
    {
       
    }

    private void OnEnter()
    {
        OnHit.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();

        if (damageable is BaseCharacerView target && !Creator.IsMyTeammate(target))
        {
            TargetView = target;
            OnEnter();
           base.OnDespawned();
        }
    }
}