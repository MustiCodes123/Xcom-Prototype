using UnityEngine;
using Zenject;

public class SphereShape : MonoBehaviour, IShape
{
    public GameObject GameObject => _gameObject;
    
    [Inject] private IShapeCollection _shapeCollection;
    
    [SerializeField] private float _radius = 1f;
    [SerializeField] private Vector3 _offset = Vector3.zero;

    private Bounds _bounds;
    private GameObject _gameObject;

    private void Awake()
    {
        _gameObject = gameObject;
        RecalculateBounds();
    }
    
    private void OnEnable()
    {
        _shapeCollection.Register(this);
    }

    private void Update()
    {
        RecalculateBounds();
    }

    private void RecalculateBounds()
    {
        _bounds = new Bounds(transform.position + _offset, Vector3.one * (_radius * 2f));
    }

    private void OnDisable()
    {
        _shapeCollection.Remove(this);
    }

    public bool IsHit(Ray ray, out float hitDistance)
    {
        if (!GameObject.activeInHierarchy)
        {
            hitDistance = 0;
            return false;
        }
        
        var direction = transform.position + _offset - ray.origin;
        var distance = Vector3.Dot(direction, ray.direction);
        var perpendicular = direction - distance * ray.direction;
        
        hitDistance = distance - Mathf.Sqrt(_radius * _radius - perpendicular.sqrMagnitude);
        return perpendicular.sqrMagnitude <= _radius * _radius;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _offset, _radius);
    }
}