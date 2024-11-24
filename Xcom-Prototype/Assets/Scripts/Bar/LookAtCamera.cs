using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cameraTransform;
    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        transform.LookAt(cameraTransform);
        
    }
}
