using UnityEngine;

[CreateAssetMenu(fileName = "ChestViewData", menuName = "Data/ChestViewData")]
public class ChestViewData : ScriptableObject
{
    [field: SerializeField] public ChestType Rarity { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
}