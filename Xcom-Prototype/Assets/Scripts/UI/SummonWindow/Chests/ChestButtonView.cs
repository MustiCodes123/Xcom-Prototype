using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ViewStateConfig
{
    [field: SerializeField] public Sprite BackgroundSprite { get; private set; }
    [field: SerializeField] public TMP_FontAsset Font { get; private set; }
}

public class ChestButtonView : MonoBehaviour
{
    public ViewStateConfig SelectedConfig;
    public ViewStateConfig UnselectedConfig;

    [SerializeField] private TMP_Text _titleTMP;
    [SerializeField] private TMP_Text _amountTMP;
    [SerializeField] private Image _background;

    private IChestButtonState _currentState;

    public void ChangeView(ViewStateConfig config)
    {
        _background.sprite = config.BackgroundSprite;
        _titleTMP.font = config.Font;
    }

    public void SetState(IChestButtonState newState)
    {
        _currentState = newState;
        _currentState.ApplyState(this);
    }

    public void DisplayAmount(int amount)
    {
        _amountTMP.text = amount.ToString();
    }
}
