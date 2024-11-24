using UnityEngine;

public class ModeScreen : UIWindowView
{
    [SerializeField] private AnimateUIElements _uiElements;
    [SerializeField] private GameObject _BG;

    public override void Show()
    {
        base.Show();
        _BG.SetActive(true);
        _uiElements.AnimatePanelsIn();
    }

    public override void Hide()
    {
        base.Hide();
        _BG.SetActive(false);
        _uiElements.AnimatePanelsOut();
    }
}
