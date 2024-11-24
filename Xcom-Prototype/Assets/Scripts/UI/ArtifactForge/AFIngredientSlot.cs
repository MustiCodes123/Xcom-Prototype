using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AFSlotsEnum
{
    First,
    Second,
    Third,
    Fourth,
    Fifth
};

public class AFIngredientSlot : MonoBehaviour
{
    public Action<CraftItem> UnselectClick;

    [SerializeField] private Image _ingredientIcon;
    [SerializeField] private TMP_Text _countTMP;
    [SerializeField] private Button _unselectButton;

    [field: SerializeField] public AFSlotsEnum SlotEnum { get; private set; }
    public CraftItem Ingredient { get; private set; }
    public bool IsFree { get; private set; } = true;

    public void SetIngredient(CraftItem ingredient)
    {
        Ingredient = ingredient;

        _ingredientIcon.sprite = ingredient.itemSprite;
        _ingredientIcon.gameObject.SetActive(true);

        _countTMP.text = $"x{ingredient.itemCount}";
        _countTMP.gameObject.SetActive(true);

        _unselectButton.gameObject.SetActive(true);
        _unselectButton.onClick.AddListener(OnUnselectClick);

        IsFree = false;
    }

    public void Unselect() => OnUnselectClick();

    private void OnUnselectClick()
    {
        _ingredientIcon.sprite = null;
        _ingredientIcon.gameObject.SetActive(false);
        _countTMP.text = "";
        _countTMP.gameObject.SetActive(false);
        _unselectButton.gameObject.SetActive(false);
        _unselectButton.onClick.RemoveAllListeners();

        UnselectClick?.Invoke(Ingredient);

        Ingredient = null;
        IsFree = true;
    }
}
