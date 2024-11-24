using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField] private Camera characterCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera _summonCamera;
    [SerializeField] private Transform topDownCameraPos;
    [SerializeField] private Transform portalCameraPos;
    [SerializeField] private Transform battlePreStartPos;
    [SerializeField] private Transform battleMapPos;

    public Camera CharacterCamera => characterCamera;

    public void MoveCameraToPortal(Action action)
    {
        mainCamera.transform.DORotateQuaternion(portalCameraPos.rotation, 0.5f);
        mainCamera.transform.DOMove(portalCameraPos.position, 0.5f).OnComplete(() => { action?.Invoke();});
    }

    public void MoveCameraToTopDown(Action action)
    {
        mainCamera.transform.DORotateQuaternion(topDownCameraPos.rotation, 0.5f);
        mainCamera.transform.DOMove(topDownCameraPos.position, 0.5f).OnComplete(() => { action?.Invoke(); });
    }

    public void MoveCameraToBattlePreStart(Action action)
    {
        mainCamera.transform.DORotateQuaternion(battlePreStartPos.rotation, 0.5f);
        mainCamera.transform.DOMove(battlePreStartPos.position, 0.5f).OnComplete(() => { action?.Invoke(); });
    }
    public void MoveCameraToBattleMap(Action action)
    {
        mainCamera.transform.DORotateQuaternion(battleMapPos.rotation, 0.5f);
        mainCamera.transform.DOMove(battleMapPos.position, 0.5f).OnComplete(() => { action?.Invoke(); });
    }

    public void ActivateCharacterCamera()
    {
        characterCamera.enabled = true;
        mainCamera.enabled = false;
        _summonCamera.enabled = false;
    }

    public void ActivateSummonCamera()
    {
        _summonCamera.enabled = true;
        mainCamera.enabled = false;
        characterCamera.enabled = false;
    }

    public void ActivateMainCamera()
    {
        _summonCamera.enabled = false;
        characterCamera.enabled = false;
        mainCamera.enabled = true;
    }

}
