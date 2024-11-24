using UnityEngine;
using UnityEngine.UI;


public class ShopCategoryButtonView : ShopSelectableButton
{
    private ShopPresenter _presenter;

    [field: SerializeField] public WindowType WindowType { get; private set; }
    [field: SerializeField] public int CategoryID { get; private set; }


    public void Initialize(ShopPresenter presenter)
    {
        _presenter = presenter;

        CategoryID = transform.GetSiblingIndex();

        Button.onClick.AddListener(OnButtonClick);
    }

    public override void ChangeView(ShopCategoryButtonViewConfig viewConfig)
    {
        ButtonImage.sprite = viewConfig.ButtonSprite;
        TextTMP.font = viewConfig.Font;
        Highlight.SetActive(viewConfig.IsSelected);
    }

    public void OnButtonClick() => _presenter.OnCategorySelected(CategoryID, WindowType);
}