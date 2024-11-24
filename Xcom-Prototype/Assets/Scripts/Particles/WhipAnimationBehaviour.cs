using System.Collections;
using UnityEngine;

public class WhipAnimationBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _animatedWhip;
    [SerializeField] private Transform _physicWhip;
    [SerializeField] private Transform _edgeEnd;

    private Vector3 _whipEdgePositionOffset = new Vector3(0, 1, 0);

    public void SecureOnTarget(Vector3 target)
    {
        ActivatePhysicsWhip();
        _edgeEnd.position = target + _whipEdgePositionOffset;
    }

    public void ActivateAnimatedWhip()
    {
        _animatedWhip.gameObject.SetActive(true);
        _physicWhip.gameObject.SetActive(false);
    }
    public void ActivatePhysicsWhip()
    {
        _animatedWhip.gameObject.SetActive(false);
        _physicWhip.gameObject.SetActive(true);
    }
}