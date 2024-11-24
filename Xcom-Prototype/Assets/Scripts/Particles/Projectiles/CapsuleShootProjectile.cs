using UnityEngine;

public class CapsuleShootProjectile : BaseProjectile
{
    public override void SkillUpdate()
    {
        var newPosition = transform.position + transform.forward * speed * Time.deltaTime;
        
        transform.position = newPosition;
    }

    private void OnEnter()
    {
        OnHit.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponent<IDamageable>();

        if (damageble is BaseCharacerView target && !Creator.IsMyTeammate(target))
        {
            TargetView = target;
            OnEnter();
        }
    }
}