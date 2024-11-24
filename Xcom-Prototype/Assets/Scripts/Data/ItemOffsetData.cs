using UnityEngine;

[System.Serializable]
public struct ItemOffsetData
{
    [SerializeField] private string _ItemAddressableName;
    public string ItemAddressableName { get => _ItemAddressableName; }

    [SerializeField] private PositionAndRotationData _Offset;
    public PositionAndRotationData Offset { get => _Offset; }
}