using System;
using UnityEngine;

public enum WindowType
{
    GamePass,
    Pack,
    Bank,
    GemShop,
    Limited,
    Daily
};

public class OfferButtonView : ShopSelectableButton
{
    public WindowType WindowType;

    private ShopPresenter _presenter;

    [field: SerializeField] public int ID { get; private set; }

    public void Initialize(ShopPresenter presenter, WindowType offerType, Action<int, WindowType> click)
    {
        _presenter = presenter;

        ID = transform.GetSiblingIndex();

        this.WindowType = offerType;

        Button.onClick.AddListener(() => click?.Invoke(ID, this.WindowType));
    }

    public override void ChangeView(ShopCategoryButtonViewConfig viewConfig)
    {
        TextTMP.font = viewConfig.Font;
        Highlight.SetActive(viewConfig.IsSelected);
    }
}