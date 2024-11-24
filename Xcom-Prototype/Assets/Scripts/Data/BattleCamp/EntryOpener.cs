using System;
using DG.Tweening;
using UnityEngine;

public class EntryOpener : MonoBehaviour
{
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 openRotation;
    [SerializeField] private float openDuration = 1f;

    private Transform _transform;
    private void Awake()
    {
        _transform = transform;
    }

    public void OpenGate()
    {
        Debug.Log("Open_ENTRY");

        var targetPosition = _transform.position + openPosition;
        
        var targetRotation = _transform.rotation.eulerAngles + openRotation;
        
        _transform.DOMove(targetPosition, openDuration);
        
        _transform.DORotate(targetRotation, openDuration);
    }
}