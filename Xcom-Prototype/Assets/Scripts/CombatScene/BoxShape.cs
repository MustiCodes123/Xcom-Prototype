using UnityEngine;
using Zenject;

public class BoxShape : MonoBehaviour, IShape
{
    public GameObject GameObject => _gameObject;

    [Inject] private IShapeCollection _shapeCollection;

    [SerializeField] private Vector3 _size = new Vector3(1f, 4f, 1f);
    [SerializeField] private Vector3 _offset = Vector3.zero;

    private Bounds _bounds;
    private GameObject _gameObject;
    private bool _isInitialized;

    public void Initialize(IShapeCollection shapeCollection)
    {
        _shapeCollection = shapeCollection;
        _isInitialized = true;
        _shapeCollection.Register(this);
    }

    private void Awake()
    {
        _gameObject = gameObject;
        RecalculateBounds();
    }

    private void Update()
    {
        RecalculateBounds();
    }

    private void RecalculateBounds()
    {
        _bounds = new Bounds(transform.position + _offset, _size);
    }

    public bool IsHit(Ray ray, out float hitDistance)
    {
        if (!gameObject.activeInHierarchy)
        {
            hitDistance = 0;
            return false;
        }

        bool hit = _bounds.IntersectRay(ray, out hitDistance);
        return hit;
    }

    private void OnDisable()
    {
        if (_isInitialized && _shapeCollection != null)
        {
            _shapeCollection.Remove(this);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + _offset, _size);
    }
    
#endif
}