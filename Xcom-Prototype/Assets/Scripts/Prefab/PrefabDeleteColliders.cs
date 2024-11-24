using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDeleteColliders : MonoBehaviour
{
    private void Awake()
    {
        DestroyAllColliders(transform);
    }

    private void DestroyAllColliders(Transform currentTransform)
    {
        // Destroy colliders attached to the current transform
        Collider[] colliders = currentTransform.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            Destroy(collider);
        }

        // Recursively call the function for each child
        foreach (Transform child in currentTransform)
        {
            DestroyAllColliders(child);
        }

        Destroy(this);
    }
}
