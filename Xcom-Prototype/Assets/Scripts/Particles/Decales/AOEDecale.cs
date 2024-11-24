using UnityEngine;
using System;

public class AOEDecale : BaseDecale
{
    [SerializeField] private Transform damageCircle;

    public override void DecaleUpdate()
    {
        if (Joystick != null)
        {
            float joysticX = Joystick.Horizontal;
            float joysticy = Joystick.Vertical;
            var angle = Mathf.Atan2(joysticX, joysticy) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, angle + offsetY, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationDelta);

            Vector3 vector = new Vector3((Math.Abs(joysticX) + Math.Abs(joysticy)) * maxRange, 0, 0);
            damageCircle.localPosition = Vector3.Lerp(damageCircle.localPosition, vector, 0.1f);
            _caster.LookAtTarget(Target);
        }
    }
}