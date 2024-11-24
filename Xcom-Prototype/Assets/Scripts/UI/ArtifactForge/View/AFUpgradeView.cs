using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AFUpgradeView : MonoBehaviour
{
    public Action<CraftItem> IngredientItemSelected;

    [SerializeField] private AFIngredientItem _ingredientItemPrefab;

    [SerializeField] private ScrollRect _ingredientsScrollRect;
    [SerializeField] private Transform _root;

    [SerializeField] private AFScroll _scroll;

    [HideInInspector] public List<AFIngredientItem> AllIngredients = new List<AFIngredientItem>();

    private void OnDisable()
    {
        ClearView();
    }

    public void DisplayScrollInfo(ArtifactRecipeItem artifactRecipe, int currentIngredientsCount)
    {
        _scroll.Initialize();
        _scroll.ShowInfo(artifactRecipe);
        _scroll.UpdateIngredientsCount(currentIngredientsCount, artifactRecipe.RecipeData.CraftItemsAmount);
    }

    public void DisplayAwailableIngredients(List<CraftItem> ingredients)
    {
        List<CraftItem> uniqueIngredients = ingredients.GroupBy(i => i.itemName).Select(g => g.First()).ToList();

        foreach (CraftItem ingredient in uniqueIngredients)
        {
            AFIngredientItem ingredientItem = Instantiate(_ingredientItemPrefab, _root);

            ingredientItem.Initialize(ingredient);
            ingredientItem.DisplayInfo(ingredients.Count(i => i.itemName == ingredient.itemName));

            ingredientItem.Click += OnIngredientItemClicked;

            AllIngredients.Add(ingredientItem);
        }
    }

    public void SetActiveScrollView(bool value)
    {
        _ingredientsScrollRect.gameObject.SetActive(value);
    }

    public void ClearView()
    {
        if (AllIngredients.Count <= 0)
            return;

        foreach (AFIngredientItem ingredient in AllIngredients)
        {
            ingredient.Click -= OnIngredientItemClicked;
            Destroy(ingredient.gameObject);
        }

        AllIngredients.Clear();
    }

    private void OnIngredientItemClicked(CraftItem ingredient) => IngredientItemSelected?.Invoke(ingredient);
}