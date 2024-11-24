using UnityEngine;

[CreateAssetMenu(fileName = "DropChances", menuName = "Data/SummonWindow/DropChances")]
public class ChestRarityDropChances : ScriptableObject
{
    [field: Header("Default Chests Set Drop Chances")]
    [field: SerializeField] public ItemSetDropChances[] WoodenChestSetDropChances { get; private set; }
    [field: SerializeField] public ItemSetDropChances[] AncientChestSetDropChances { get; private set; }
    [field: SerializeField] public ItemSetDropChances[] KingChestSetDropChances { get; private set; }
    [field: SerializeField] public ItemSetDropChances[] HeroChestSetDropChances { get; private set; }
    [field: SerializeField] public ItemSetDropChances[] GodChestSetDropChances { get; private set; }

    [field: Space(10)]
    [field: Header("Exceptional Chests Sets")]
    [field: SerializeField] public string[] ExceptionalChestSetPlayFabIDs { get; private set; }

    [field: Space(10)]
    [field: Header("Broken Chests Rarity Drop Chances")]
    [field: SerializeField] public RarityDropChances BrokenWoodenChestDropChances { get; private set; }
    [field: SerializeField] public RarityDropChances BrokenAncientChestDropChances { get; private set; }
    [field: SerializeField] public RarityDropChances BrokenKingChestDropChances { get; private set; }
    [field: SerializeField] public RarityDropChances BrokenHeroChestDropChances { get; private set; }
    [field: SerializeField] public RarityDropChances BrokenGodChestDropChances { get; private set; }


    [field: Space(10)]
    [field: Header("Broken Chests Item Type Drop Chances")]
    [field: SerializeField] public ItemTypeDropChances[] BrokenWoodenChestItemsDropChances { get; private set; }
    [field: SerializeField] public ItemTypeDropChances[] BrokenAncientChestItemsDropChances { get; private set; }
    [field: SerializeField] public ItemTypeDropChances[] BrokenKingChestItemsDropChances { get; private set; }
    [field: SerializeField] public ItemTypeDropChances[] BrokenHeroChestItemsDropChances { get; private set; }
    [field: SerializeField] public ItemTypeDropChances[] BrokenGodChestItemsDropChances { get; private set; }

    [System.Serializable]
    public class RarityDropChances
    {
        [field: Range(0, 100)]
        [field: SerializeField]
        public float CommonDropChance { get; private set; }

        [field: Range(0, 100)]
        [field: SerializeField]
        public float RareDropChance { get; private set; }

        [field: Range(0, 100)]
        [field: SerializeField]
        public float EpicDropChance { get; private set; }

        [field: Range(0, 100)]
        [field: SerializeField]
        public float LegendaryDropChance { get; private set; }

        [field: Range(0, 100)]
        [field: SerializeField]
        public float MythicalDropChance { get; private set; }
    }

    [System.Serializable]
    public class ItemTypeDropChances
    {
        [field: SerializeField]
        public SlotEnum ItemType { get; private set; }

        [field: Range(0, 100)]
        [field: SerializeField]
        public float DropChance { get; private set; }
    }

    [System.Serializable]
    public class ItemSetDropChances
    {
        [field: SerializeField]
        public ItemsSetRarity SetRarity { get; private set; }

        [field: SerializeField]
        [field: Range(0, 100)]
        public float DropChance { get; private set; }
    }
}