using UnityEngine;

public class FirstCharacterDataContainer : MonoBehaviour
{
    [field: SerializeField] public SkinnedMeshRenderer[] MeshRenderer { get; private set; }
    [field: SerializeField] public CharacterPreset CharacterPreset { get; private set; }

    [field: SerializeField] public PositionAndRotationData Offset;

    public BaseCharacterModel CharacterModel => CharacterPreset.CreateCharacter();

    private void Awake()
    {
        transform.localPosition += Offset.Position;
        transform.localEulerAngles += Offset.EulerAngles;
    }
}
