using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialColor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Color col;

    private void Start()
    {
        Material mat = null;

        if (this.GetComponent<TrailRenderer>())
        {
            mat = this.GetComponent<TrailRenderer>().material;

            mat.SetColor("_TintColor", col);
        }
        else if (this.GetComponent<MeshRenderer>())
            mat = this.GetComponent<MeshRenderer>().material;

        if(mat)
            mat.color = col;
    }
}
