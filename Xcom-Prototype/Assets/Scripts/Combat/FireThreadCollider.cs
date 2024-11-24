using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireThreadCollider : MonoBehaviour
{
    public Action<FireThreadCollider> OnCollision;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            OnCollision?.Invoke(this);
        }
    }
}
