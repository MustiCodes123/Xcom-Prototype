using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AFScroll : MonoBehaviour
{
    public Action Unselect;

    [SerializeField] private TMP_Text _artifactNameTMP;
    [SerializeField] private TMP_Text _descriptionTMP;
    [SerializeField] private TMP_Text _ingredientsCountTMP;

    [SerializeField] private Image _artifactIcon;
    [SerializeField] private Image _ingredientIcon;

    [SerializeField] private Button _unselectButton;

    [field : SerializeField] public GameObject ScrollGameObject { get; private set; }

    public void Initialize()
    {
        _unselectButton.onClick.AddListener(OnUnselectClick);
    }

    public void ShowInfo(ArtifactRecipeItem artifactRecipe)
    {
        _artifactNameTMP.text = artifactRecipe.itemName;
        _descriptionTMP.text = artifactRecipe.itemDescription;
        _ingredientsCountTMP.text = artifactRecipe.RecipeData.CraftItemsAmount.ToString();

        _artifactIcon.sprite = artifactRecipe.itemSprite;
        _ingredientIcon.sprite = artifactRecipe.RecipeData.CraftItem.itemSprite;

        ScrollGameObject.SetActive(true);
    }

    public void UpdateIngredientsCount(int currentCount, int requiredCount)
    {
        _ingredientsCountTMP.text = $"{currentCount}/{requiredCount}";

        if (currentCount >= requiredCount)
        {
            _ingredientsCountTMP.color = Color.green;
        }
        else
        {
            _ingredientsCountTMP.color = Color.red;
        }
    }

    private void OnUnselectClick() => Unselect?.Invoke();
}