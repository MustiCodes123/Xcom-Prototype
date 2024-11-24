using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AFUpgradeModel : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    public ArtifactRecipeItem CurrentRecipe { get; set; }
    public List<CraftItem> SelectedIngredients { get; set; } = new List<CraftItem>();
    public List<CraftItem> AwailableIngredients { get; private set; }
    public int SelectedIngredientsCount { get; set; }

    public void Initialize(ArtifactRecipeItem artifactRecipe)
    {
        AwailableIngredients = new List<CraftItem>();
        List<CraftItem> allIngredients = new List<CraftItem>();

        CurrentRecipe = artifactRecipe;

        foreach (BaseItem item in _playerData.PlayerInventory)
        {
            if (item is CraftItem ingredient)
            {
                allIngredients.Add(ingredient);
            }
        }

        CraftItem targetIngredient = (CraftItem)ItemsDataInfo.Instance.ConvertTemplateToItem<CraftItem>(artifactRecipe.RecipeData.CraftItem);

        foreach(CraftItem ingredient in allIngredients)
        {
            if (ingredient.itemName == targetIngredient.itemName)
            {
                AwailableIngredients.Add(ingredient);
            }

            else continue;
        }
    }

    public void ClearModel()
    {
        CurrentRecipe = null;
        SelectedIngredients.Clear();
        AwailableIngredients = new List<CraftItem>();
        SelectedIngredientsCount = 0;
    }
}