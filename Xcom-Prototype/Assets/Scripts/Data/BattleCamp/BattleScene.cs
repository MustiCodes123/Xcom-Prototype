using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    private const float MinDistanceToFloor = 3.5f;

    public Vector3 SpawnCameraPosition => _spawnCameraPosition;
    public Quaternion SpawnCameraRotation => Quaternion.Euler(_spawnCameraAngle);
    public Vector3 StartCameraPosition => _startCameraPosition;
    public Transform CameraTarget => _cameraBoundsCenter;
    public List<Transform> StartBattlePositions => _startBattlePositions;
    public List<Transform> SpawnCharacterPositions => _spawnCharacterPositions;
    public List<Transform> StartEnemiesPositions => _spawnEnemiesPositions;
    public List<Transform> CameraWaypoints => _cameraWaypoints;

    [SerializeField] private List<Transform> _startBattlePositions = new();
    [SerializeField] private List<Transform> _spawnCharacterPositions = new();
    [SerializeField] private List<Transform> _spawnEnemiesPositions = new();
    [SerializeField] private List<Transform> _cameraWaypoints = new();

    [SerializeField] private Vector3 _spawnCameraPosition;
    [SerializeField] private Vector3 _spawnCameraAngle;

    [SerializeField] private Vector3 _startCameraPosition;

    [SerializeField] private Transform _cameraBoundsCenter;
    [SerializeField] private Vector3 _cameraBoundsSize = new Vector3(20, 10, 20);

    [SerializeField] private EntryOpener[] _entryOpeners;

    private void Awake()
    {
        Debugging();
    }

    public void OpenGates()
    {
        foreach (var entryOpener in _entryOpeners)
        {
            if (entryOpener != null)
                entryOpener.OpenGate();
        }
    }

    public Bounds GetSceneBounds()
    {
        Bounds bounds = new Bounds();
        bounds.center = _cameraBoundsCenter.position + (Vector3.up * _cameraBoundsSize.y) / 2 + Vector3.up * MinDistanceToFloor;
        bounds.size = _cameraBoundsSize;
        return bounds;
    }

    private void Debugging()
    {
        if (_startBattlePositions.Count == 0 || _spawnCharacterPositions.Count == 0 || _spawnEnemiesPositions.Count == 0)
            Debug.Log("Some fields of " + gameObject.name + " " + "is not initialized");
    }

    private void DrawPositions(List<Transform> positions, Color color)
    {
        Gizmos.color = color;
        foreach (var transform in positions)
        {
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawPositions(_startBattlePositions, Color.green);
        DrawPositions(_spawnCharacterPositions, Color.blue);
        DrawPositions(_spawnEnemiesPositions, Color.red);

        Gizmos.DrawCube(_cameraBoundsCenter.position + (Vector3.up * _cameraBoundsSize.y) / 2 + Vector3.up * MinDistanceToFloor,
            _cameraBoundsSize);
        Gizmos.DrawSphere(_startCameraPosition, 1f);
        Gizmos.DrawCube(_spawnCameraPosition, Vector3.one * 2);

        // Draw camera waypoints
        DrawPositions(_cameraWaypoints, Color.yellow);
    }
}
