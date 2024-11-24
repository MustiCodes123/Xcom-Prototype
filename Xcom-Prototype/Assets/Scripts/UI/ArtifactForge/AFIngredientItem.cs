using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AFIngredientItemViewConfig
{
    [field: SerializeField] public bool IsZeroIngredients { get; private set; }
    [field: SerializeField] public Color IconColor { get; private set; }
}

[RequireComponent(typeof(Button))]
public class AFIngredientItem : MonoBehaviour
{
    public Action<CraftItem> Click;

    public AFIngredientItemViewConfig HasIngredientsConfig;
    public AFIngredientItemViewConfig ZeroIngredientsConfig;
    public AFIngredientItemViewConfig DefaultConfig;

    [SerializeField] private Image _inactiveImage;

    [SerializeField] private Image _ingredientIcon;
    [SerializeField] private TMP_Text _titleTMP;
    [SerializeField] private TMP_Text _countTMP;

    private Button _button;

    public CraftItem Ingredient { get; set; }

    private void OnEnable()
    {
        if (!TryGetComponent(out _button))
            Debug.LogError($"Button is null");

        _button.onClick.AddListener(OnClick);
    }

    public void ChangeView(AFIngredientItemViewConfig config)
    {
        _inactiveImage.gameObject.SetActive(config.IsZeroIngredients);
        _inactiveImage.color = config.IconColor;
        _button.interactable = !config.IsZeroIngredients;
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Initialize(CraftItem ingredient) => Ingredient = ingredient;

    public void DisplayInfo(int count)
    {
        _ingredientIcon.sprite = Ingredient.itemSprite;
        _titleTMP.text = Ingredient.itemName;
        _countTMP.text = count.ToString();
    }

    private void OnClick() => Click?.Invoke(Ingredient);
}