using UnityEngine;

public class Chest3DModel : MonoBehaviour
{
    [field: SerializeField] public ChestType ChestRarity { get; private set; }
    [field: SerializeField] public ChestIdleAnimation IdleAnimation { get; private set; }
    [field: SerializeField] public ChestsOpenAnimation OpenAnimation { get; private set; }
}
