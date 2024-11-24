using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopCategoryButtonViewConfig
{
    [field: SerializeField] public Sprite ButtonSprite { get; private set; }
    [field: SerializeField] public TMP_FontAsset Font { get; private set; }
    [field: SerializeField] public bool IsSelected { get; private set; }
}

[RequireComponent(typeof(Button))]
public abstract class ShopSelectableButton : MonoBehaviour
{
    public ShopCategoryButtonViewConfig SelectedStateConfig;
    public ShopCategoryButtonViewConfig UnselectedStateConfig;

    [SerializeField] protected Button Button;
    [SerializeField] protected Image ButtonImage;
    [SerializeField] protected TMP_Text TextTMP;
    [SerializeField] protected GameObject Highlight;

    private IShopCategoryButtonState _currentState;

    public abstract void ChangeView(ShopCategoryButtonViewConfig viewConfig);

    public void SetState(IShopCategoryButtonState newState)
    {
        _currentState = newState;
        newState.ApplyState(this);
    }
}