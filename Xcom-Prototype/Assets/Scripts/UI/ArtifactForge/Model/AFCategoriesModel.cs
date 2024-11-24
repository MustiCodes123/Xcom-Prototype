using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

#region Enumerations
public enum AFCategories
{
    Rings,
    Amulets,
    Armlets,
    All
}
#endregion

public class AFCategoriesModel : UIWindowView
{
    [Inject] private PlayerData _playerData;

    public Dictionary<AFCategories, List<ArtifactRecipeItem>> SortedRecipes { get; private set; }

    public void Initialize()
    {
        SortRecipes();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SortRecipes()
    {
        SortedRecipes = new Dictionary<AFCategories, List<ArtifactRecipeItem>>();

        List<ArtifactRecipeItem> allRecipes = new List<ArtifactRecipeItem>();

        foreach (BaseItem item in _playerData.PlayerInventory)
        {
            if (item is ArtifactRecipeItem recipeItem)
            {
                allRecipes.Add(recipeItem);
            }
        }

        foreach (ArtifactRecipeItem recipeItem in allRecipes)
        {
            switch (recipeItem.TargetItem.Slot)
            {
                case (SlotEnum.Ring):
                    AddToSortedItems(AFCategories.Rings, recipeItem);
                    break;
                case (SlotEnum.Amulet):
                    AddToSortedItems(AFCategories.Amulets, recipeItem);
                    break;
                case (SlotEnum.Armlet):
                    AddToSortedItems(AFCategories.Armlets, recipeItem);
                    break;
            }

            AddToSortedItems(AFCategories.All, recipeItem);
        }
    }

    private void AddToSortedItems(AFCategories category, ArtifactRecipeItem recipeItem)
    {
        if (!SortedRecipes.ContainsKey(category))
        {
            SortedRecipes[category] = new List<ArtifactRecipeItem>();
        }

        SortedRecipes[category].Add(recipeItem);
    }
}
