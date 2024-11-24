using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AFRecipeItem : MonoBehaviour
{
    public Action<ArtifactRecipeItem> Click;

    [SerializeField] private Image _artifactIcon;
    [SerializeField] private Image _ingredientIcon;
    [SerializeField] private TMP_Text _ingredientsCountTMP;
    [SerializeField] private TMP_Text _levelTMP;

    private Button _button;

    public ArtifactRecipeItem ArtifactRecipe { get; set; }

    private void OnEnable()
    {
        if (!TryGetComponent(out _button))
            Debug.LogError("Button is null");

        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Initialize(ArtifactRecipeItem artifactRecipe) => ArtifactRecipe = artifactRecipe;

    public void DisplayInfo()
    {
        _artifactIcon.sprite = ArtifactRecipe.itemSprite;
        _ingredientIcon.sprite = ArtifactRecipe.RecipeData.CraftItem.itemSprite;
        _ingredientsCountTMP.text = ArtifactRecipe.RecipeData.CraftItemsAmount.ToString();
    }

    private void OnClick() => Click?.Invoke(ArtifactRecipe);
}
