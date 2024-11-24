using System.Collections;
using UnityEngine;
public class Directed : BaseProjectile
{
    public override void SkillUpdate()
    {
        if (IsActive)
        {
            transform.position += -Vector3.forward * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<IEnemy>();
        var damageble = other.GetComponent<IDamageable>();

        if (enemy != null && damageble != null)
        {
            OnDamage();
        }
    }
}