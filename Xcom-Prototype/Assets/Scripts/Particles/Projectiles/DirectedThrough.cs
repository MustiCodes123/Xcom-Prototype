using System.Collections;
using UnityEngine;

public class DirectedTrough: Directed
{
    public override void OnDamage()
    {
        OnHit.Invoke();
    }
}