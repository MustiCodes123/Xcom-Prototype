using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickLightningVFX_TestScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool canMoveToObject = false;
    [SerializeField] float delayStartTime = 0.5f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] List<Transform> objToTeleport = new List<Transform>();

    Vector3 origPos = Vector3.negativeInfinity;
    int curId = 0;

    bool activated = false;

    private void OnEnable()
    {


        if (!activated)
        {
            origPos = this.transform.position;
            activated = true;
        }
        else
        {
            this.transform.position = origPos;
            curId = 0;
        }

        Invoke("StartTeleporting", delayStartTime);
    }

    private void StartTeleporting()
    {
        if (objToTeleport.Count == 0)
            return;

        canMoveToObject = true;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsObject();   
    }

    private void MoveTowardsObject()
    {
        if (!canMoveToObject)
            return;

        float curSpeed = Time.deltaTime * moveSpeed;

        Vector3 thisPos = this.transform.position;
        Vector3 nextPos = objToTeleport[curId].transform.position;

        nextPos.y = thisPos.y;

        this.transform.position = Vector3.MoveTowards(thisPos,nextPos,curSpeed);

        if (Vector3.Distance(thisPos, nextPos) <= 0.001f)
        {
            curId++;

            if (curId>=objToTeleport.Count)
            {
                canMoveToObject = false;

                this.transform.position = origPos;

            }
        }
    }
}
