using System.Collections.Generic;
using UnityEngine;

public enum ExtensionType
{
    Character,
    Inventory
};

[System.Serializable]
public class InventoryExtensionLevel
{
    [field: SerializeField] public int GoldPrice { get; private set; }
    [field: SerializeField] public int GemPrice { get; private set; }
    [field: SerializeField] public int SlotCount { get; private set; }
}

[CreateAssetMenu(fileName = "SlotsExtensionSettings", menuName = "Settings/SlotsExtensionSettings")]
public class SlotsExtensionSettings : ScriptableObject
{
    [field: SerializeField] public List<InventoryExtensionLevel> CharacterSlotsExtensionLevels { get; private set; }
    [field: SerializeField] public List<InventoryExtensionLevel> InventorySlotsExtensionLevels { get; private set; }
}
