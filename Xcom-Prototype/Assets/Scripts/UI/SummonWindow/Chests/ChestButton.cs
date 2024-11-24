using System;
using UnityEngine;
using UnityEngine.UI;

#region Enumerations
public enum ChestType
{
    Wooden,
    Ancient,
    King,
    Hero,
    God,
    BrokenWooden,
    BrokenAncient,
    BrokenKing,
    BrokenHero,
    BrokenGod,
    Exceptional
};
#endregion

#region Data Classes
[System.Serializable]
public class ChestData
{
    [field: SerializeField] public ChestType ChestType { get; private set; }
    [field: SerializeField] public GameCurrencies CurrencyType { get; private set; }
    [field: SerializeField] public uint Price { get; private set; }
}
#endregion

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(ChestButtonView))]
public class ChestButton : MonoBehaviour
{
    public Action<ChestButton> Click;
    public Action<uint> OnPriceChanged;

    private Button _button;

    [field: SerializeField] public ChestButtonView ButtonView { get; private set; }
    [field: SerializeField] public ChestData Data { get; private set; }
    [field: SerializeField] public Chest3DModel Chest3DModel { get; private set; }
    [field: SerializeField] public bool IsBroken { get; private set; }

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        ButtonView = GetComponent<ChestButtonView>();

        _button.onClick.AddListener(OnClick);

        PlayerInventory.Instance.OnChestAmountChanged += OnChestAmountChanged;
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
        PlayerInventory.Instance.OnChestAmountChanged -= OnChestAmountChanged;
        Chest3DModel.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if(Data.ChestType.ToString().StartsWith("Broken"))
        {
            IsBroken = true;
        }
        else
        {
            IsBroken = false;
        }
    }

    private void OnClick()
    {
        Click?.Invoke(this);
        OnPriceChanged?.Invoke(Data.Price);
    }

    private void OnChestAmountChanged(ChestType chestsRarity, int chestsAmount)
    {
        if (Data.ChestType == chestsRarity)
            ButtonView.DisplayAmount(chestsAmount);
    }
}
