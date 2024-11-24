using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AFCategoriesView : MonoBehaviour
{
    public Action<ArtifactRecipeItem> RecipeItemSelected;

    [SerializeField] private AFRecipeItem _recipeItemPrefab;

    [SerializeField] private ScrollRect _recipesScrollRect;

    [SerializeField] private RectTransform _ringsCategoryRoot;
    [SerializeField] private RectTransform _amuletsCategoryRoot;
    [SerializeField] private RectTransform _armletsCategoryRoot;
    [SerializeField] private RectTransform _allCategoryRoot;

    [HideInInspector] public List<AFRecipeItem> AllRecipes = new List<AFRecipeItem>();

    #region MonoBehaviour Methods
    private void OnDisable()
    {
        ClearView();
    }
    #endregion

    #region View Methods
    public void DisplayRecipes(Dictionary<AFCategories, List<ArtifactRecipeItem>> recipes)
    {
        foreach (KeyValuePair<AFCategories, List<ArtifactRecipeItem>> kvp in recipes)
        {
            switch (kvp.Key)
            {
                case (AFCategories.Rings):
                    DisplaySingleRecipe(recipes, AFCategories.Rings, _ringsCategoryRoot);
                    break;

                case (AFCategories.Amulets):
                    DisplaySingleRecipe(recipes, AFCategories.Amulets, _amuletsCategoryRoot);
                    break;

                case (AFCategories.Armlets):
                    DisplaySingleRecipe(recipes, AFCategories.Armlets, _armletsCategoryRoot);
                    break;

                case (AFCategories.All):
                    DisplaySingleRecipe(recipes, AFCategories.All, _allCategoryRoot);
                    break;
            }
        }
    }

    public void DisplayCategory(AFCategories category)
    {
        switch (category)
        {
            case (AFCategories.Rings):
                _recipesScrollRect.content = _ringsCategoryRoot;
                HideAllCategories();
                _ringsCategoryRoot.gameObject.SetActive(true);
                break;

            case (AFCategories.Amulets):
                _recipesScrollRect.content = _amuletsCategoryRoot;
                HideAllCategories();
                _amuletsCategoryRoot.gameObject.SetActive(true);
                break;

            case (AFCategories.Armlets):
                _recipesScrollRect.content = _armletsCategoryRoot;
                HideAllCategories();
                _armletsCategoryRoot.gameObject.SetActive(true);
                break;

            case (AFCategories.All):
                _recipesScrollRect.content = _allCategoryRoot;
                HideAllCategories();
                _allCategoryRoot.gameObject.SetActive(true);
                break;
        }
    }

    public void SetActiveScrollView(bool value)
    {
        _recipesScrollRect.gameObject.SetActive(value);
    }

    public void ClearView()
    {
        foreach (AFRecipeItem recipe in AllRecipes)
        {
            recipe.Click -= OnRecipeItemClicked;
            Destroy(recipe.gameObject);
        }

        AllRecipes.Clear();
    }
    #endregion

    #region Utility Methods
    private void DisplaySingleRecipe(Dictionary<AFCategories, List<ArtifactRecipeItem>> recipes, AFCategories key, Transform root)
    {
        if (!recipes.ContainsKey(key))
            return;

        foreach(ArtifactRecipeItem artifactRecipe in recipes[key])
        {
            AFRecipeItem recipeItem = Instantiate(_recipeItemPrefab, root);
            recipeItem.Initialize(artifactRecipe);
            recipeItem.DisplayInfo();
            recipeItem.Click += OnRecipeItemClicked;

            AllRecipes.Add(recipeItem);
        }
    }

    private void HideAllCategories()
    {
        _ringsCategoryRoot.gameObject.SetActive(false);
        _amuletsCategoryRoot.gameObject.SetActive(false);
        _armletsCategoryRoot.gameObject.SetActive(false);
        _allCategoryRoot.gameObject.SetActive(false);
    }
    #endregion

    #region Callbacks
    private void OnRecipeItemClicked(ArtifactRecipeItem artifactRecipe) => RecipeItemSelected?.Invoke(artifactRecipe);
    #endregion
}
