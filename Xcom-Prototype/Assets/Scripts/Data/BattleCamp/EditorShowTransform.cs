using UnityEngine;

public class EditorShowTransform : MonoBehaviour
{
    
    [SerializeField] private Transform transform;
    [SerializeField] private float size = 0.5f; 
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, size);
    }
}