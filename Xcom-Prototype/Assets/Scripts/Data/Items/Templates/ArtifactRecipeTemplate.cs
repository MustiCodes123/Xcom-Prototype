using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactRecipeTemplate", menuName = "Inventory/ArtifactRecipeTemplate")]
public class ArtifactRecipeTemplate : ItemTemplate
{
    [field: SerializeField] public RecipeData RecipeData { get; private set; }
    [field: SerializeField] public ItemTemplate TargetItemTemplate { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public GameCurrencies Currency { get; private set; }
}

[System.Serializable]
public class RecipeData
{
    [SerializeField] public CraftItemTemplate CraftItem;
    [SerializeField] public int CraftItemsAmount;
}