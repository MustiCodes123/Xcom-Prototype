using UnityEngine;

[System.Serializable]
public struct PositionAndRotationData
{
    [SerializeField] private Vector3 _Position;
    public Vector3 Position { get => _Position; }

    [SerializeField] private Vector3 _EulerAngles;
    public Vector3 EulerAngles { get => _EulerAngles; }
}