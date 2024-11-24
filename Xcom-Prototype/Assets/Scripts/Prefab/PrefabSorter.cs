using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrefabSorter : MonoBehaviour
{

    [Header("Settings")]
    [Tooltip("To sort or not")]
    [SerializeField] bool sort = false;
    [Tooltip("How many items to show at same row")]
    [SerializeField] int rawItemAmount = 5;
    [Tooltip("What distance should items keep")]
    [SerializeField] float distance = 2f;


    private void Awake()
    {
        SortPrefabs();
    }

    private void SortPrefabs()
    {
        if (!sort)
            return;

        int childCount = this.transform.childCount;

        float curX = 0;
        float curRaw = 0;
        int curItemAmount = 0;

        for (int i = 0; i < childCount; i++)
        {
            this.transform.GetChild(i).transform.localPosition = new Vector3(curX,0,curRaw);

            curX += distance;
            curItemAmount++;

            if(curItemAmount >= rawItemAmount)
            {
                curX = 0;
                curRaw += distance;
                curItemAmount = 0;
            }
        }

        Destroy(this);
    }

}
