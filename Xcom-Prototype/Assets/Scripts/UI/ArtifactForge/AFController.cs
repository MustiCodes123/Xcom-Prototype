using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AFController : MonoBehaviour
{
    [Inject] PlayerData _playerData;

    [SerializeField] private AFCategoriesModel _categoriesModel;
    [SerializeField] private AFUpgradeModel _upgradeModel;

    [SerializeField] private AFCategoriesView _categoriesView;
    [SerializeField] private AFUpgradeView _upgradeView;

    [SerializeField] private AFScroll _scroll;
    [SerializeField] private AFCircle _circle;

    [SerializeField] private List<AFCategoryButton> _categoriesButtons;
    [SerializeField] private List<AFIngredientSlot> _ingredientSlots;

    [SerializeField] private AFPurchaseButton _purchaseButton;

    private Dictionary<AFSlotsEnum, AFIngredientSlot> _slotsDictionary;

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _categoriesModel.Initialize();
        _categoriesView.DisplayRecipes(_categoriesModel.SortedRecipes);

        SubscribeToEvents();

        _slotsDictionary = new Dictionary<AFSlotsEnum, AFIngredientSlot>();
        foreach (AFIngredientSlot slot in _ingredientSlots)
        {
            _slotsDictionary[slot.SlotEnum] = slot;
        }

        _purchaseButton.Button.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();

        _slotsDictionary.Clear();
    }
    #endregion

    #region Initialization
    private void SubscribeToEvents()
    {
        foreach(AFCategoryButton categoryButton in _categoriesButtons)
        {
            categoryButton.Click += OnCategoryButtonClick;
        }

        foreach (AFIngredientSlot slot in _ingredientSlots)
        {
            slot.UnselectClick += OnUnselectSlot;
        }

        _categoriesView.RecipeItemSelected += OnRecipeItemSelected;
        _upgradeView.IngredientItemSelected += OnIngredientItemSelected;
        _scroll.Unselect += OnScrollUnselect;
        _circle.ItemClick += OnCircleClick;

        _purchaseButton.Click += OnPurchaseClick;
    }

    private void UnsubscribeFromEvents()
    {
        foreach (AFCategoryButton categoryButton in _categoriesButtons)
        {
            categoryButton.Click -= OnCategoryButtonClick;
        }

        _categoriesView.RecipeItemSelected -= OnRecipeItemSelected;
        _upgradeView.IngredientItemSelected -= OnIngredientItemSelected;
        _scroll.Unselect -= OnScrollUnselect;
        _circle.ItemClick -= OnCircleClick;

        _purchaseButton.Click -= OnPurchaseClick;
    }
    #endregion

    #region Callbacks
    private void OnCategoryButtonClick(AFCategories category, AFCategoryButton clickedButton)
    {
        _categoriesView.DisplayCategory(category);

        foreach(AFCategoryButton button in _categoriesButtons)
        {
            if (button == clickedButton)
            {
                button.SetView(button.SelectedView);
                continue;
            }

            button.SetView(button.UnselectedView);
        }
    }

    private void OnRecipeItemSelected(ArtifactRecipeItem artifactRecipe)
    {
        _upgradeView.ClearView();

        _categoriesView.SetActiveScrollView(false);

        _upgradeModel.Initialize(artifactRecipe);

        int currentIngredientsCount = _upgradeModel.AwailableIngredients.Count(i => i.itemName == artifactRecipe.RecipeData.CraftItem.itemName);
        _upgradeView.DisplayAwailableIngredients(_upgradeModel.AwailableIngredients);
        _upgradeView.DisplayScrollInfo(artifactRecipe, currentIngredientsCount);

        _upgradeView.SetActiveScrollView(true);

        _circle.ShowItem(artifactRecipe.TargetItem.itemSprite);

        UpdatePurchaseButton();
    }

    private void OnIngredientItemSelected(CraftItem ingredient)
    {
        AFIngredientSlot freeSlot = GetFirstFreeSlot();

        if (freeSlot != null)
        {
            int requiredCount = _upgradeModel.CurrentRecipe.RecipeData.CraftItemsAmount;
            int availableCount = _upgradeModel.AwailableIngredients.Count(i => i.itemName == ingredient.itemName);

            if (availableCount >= requiredCount)
            {
                ingredient.itemCount = requiredCount;
                freeSlot.SetIngredient(ingredient);
                _upgradeModel.SelectedIngredients.Add(ingredient);
                _upgradeModel.SelectedIngredientsCount += requiredCount;

                AFIngredientItem selectedIngredientItem = _upgradeView.AllIngredients.Find(item => item.Ingredient.itemName == ingredient.itemName);

                if (selectedIngredientItem != null)
                {
                    int remainingCount = availableCount - requiredCount;

                    if (remainingCount > 0)
                    {
                        selectedIngredientItem.DisplayInfo(remainingCount);
                        selectedIngredientItem.ChangeView(selectedIngredientItem.HasIngredientsConfig);
                    }
                    else
                    {
                        selectedIngredientItem.DisplayInfo(0);
                        selectedIngredientItem.ChangeView(selectedIngredientItem.ZeroIngredientsConfig);
                    }
                }
            }
            else
            {
                ingredient.itemCount = availableCount;
                freeSlot.SetIngredient(ingredient);
                _upgradeModel.SelectedIngredients.Add(ingredient);
                _upgradeModel.SelectedIngredientsCount += availableCount;

                AFIngredientItem selectedIngredientItem = _upgradeView.AllIngredients.Find(item => item.Ingredient.itemName == ingredient.itemName);

                if (selectedIngredientItem != null)
                {
                    selectedIngredientItem.DisplayInfo(0);
                    selectedIngredientItem.ChangeView(selectedIngredientItem.ZeroIngredientsConfig);
                }
            }
        }
        else
        {
            Debug.Log("No free slots available.");
        }

        UpdatePurchaseButton();
    }

    private void OnUnselectSlot(CraftItem item)
    {
        if (item != null)
        {
            _upgradeModel.SelectedIngredients.Remove(item);
            _upgradeModel.SelectedIngredientsCount -= item.itemCount;

            AFIngredientItem selectedIngredientItem = _upgradeView.AllIngredients.Find(i => i.Ingredient.itemName == item.itemName);

            if (selectedIngredientItem != null)
            {
                int availableCount = _upgradeModel.AwailableIngredients.Count(i => i.itemName == item.itemName);
                int requiredCount = _upgradeModel.CurrentRecipe.RecipeData.CraftItemsAmount;

                selectedIngredientItem.DisplayInfo(availableCount);
                selectedIngredientItem.ChangeView(selectedIngredientItem.DefaultConfig);
            }
        }

        UpdatePurchaseButton();
    }

    private void OnScrollUnselect()
    {
        _scroll.ScrollGameObject.SetActive(false);
        _upgradeView.SetActiveScrollView(false);

        _categoriesView.ClearView();

        _categoriesModel.Initialize();
        _categoriesView.DisplayRecipes(_categoriesModel.SortedRecipes);

        foreach(AFIngredientSlot slot in _ingredientSlots)
            slot.Unselect();

        _circle.Unselect();

        _categoriesView.SetActiveScrollView(true);

        UpdatePurchaseButton();
    }

    private void OnCircleClick()
    {
        OnScrollUnselect();

        _circle.Unselect();

        UpdatePurchaseButton();
    }

    private async void OnPurchaseClick()
    {
        if (_upgradeModel.CurrentRecipe == null)
        {
            Debug.LogError("Current recipe is null");
            return;
        }

        int price = _upgradeModel.CurrentRecipe.Price;
        GameCurrencies currency = _upgradeModel.CurrentRecipe.Currency;

        bool result = Wallet.Instance.SpendCachedCurrency(currency, (uint)price);

        if (!result)
        {
            Debug.LogError("Purchase operation returned false");
            return;
        }

        BaseItem item = ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(_upgradeModel.CurrentRecipe.TargetItem);
        _playerData.AddItemToInventory(item);

        ArtifactRecipeItem recipeToRemove = _playerData.PlayerInventory.Find(i => i is ArtifactRecipeItem recipe && recipe.RecipeData == _upgradeModel.CurrentRecipe.RecipeData) as ArtifactRecipeItem;
        if (recipeToRemove != null)
        {
            _playerData.RemoveItemFromInventory(recipeToRemove);
        }

        List<CraftItem> ingredientsToRemove = _playerData.PlayerInventory
            .Where(i => i is CraftItem craftItem && craftItem.itemName == _upgradeModel.CurrentRecipe.RecipeData.CraftItem.itemName)
            .Cast<CraftItem>()
            .Take(_upgradeModel.SelectedIngredientsCount)
            .ToList();

        foreach (CraftItem ingredient in ingredientsToRemove)
        {
            _playerData.RemoveItemFromInventory(ingredient);
        }

        foreach (AFIngredientSlot slot in _ingredientSlots)
        {
            if (!slot.IsFree)
            {
                slot.Unselect();
            }
        }

        _upgradeModel.ClearModel();

        OnCircleClick();
    }
    #endregion

    #region Utility Methods
    private AFIngredientSlot GetFirstFreeSlot()
    {
        foreach (AFSlotsEnum slotEnum in Enum.GetValues(typeof(AFSlotsEnum)))
        {
            if (_slotsDictionary[slotEnum].IsFree)
                return _slotsDictionary[slotEnum];
        }

        return null;
    }

    private void UpdatePurchaseButton()
    {
        if (_upgradeModel.CurrentRecipe == null)
        {
            _purchaseButton.Button.gameObject.SetActive(false);
            return;
        }

        int requiredIngredientsCount = _upgradeModel.CurrentRecipe.RecipeData.CraftItemsAmount;
        int selectedIngredientsCount = _upgradeModel.SelectedIngredients.Sum(i => i.itemCount);
        int price = _upgradeModel.CurrentRecipe.Price;

        _purchaseButton.SetPrice(price);
        _purchaseButton.Button.gameObject.SetActive(selectedIngredientsCount >= requiredIngredientsCount);
    }
    #endregion
}