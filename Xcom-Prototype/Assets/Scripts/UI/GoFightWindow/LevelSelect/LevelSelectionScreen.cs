using UnityEngine;

public class LevelSelectionScreen : UIWindowView
{
    [SerializeField] private FightWindowController _fwController;
    [SerializeField] private LevelSelectController _controller;
    [SerializeField] private AnimateUIElements _uiElements;
    [SerializeField] private GameObject _BG;
    override public void Show()
    {
        base.Show();
        _BG.SetActive(true);
        _uiElements.AnimatePanelsIn();
    }

    override public void Hide()
    {
        base.Hide();
        _BG.SetActive(false);
        _uiElements.AnimatePanelsOut();
        _controller.OnDisableScreen();

        if (_tempInfo.FirstWinfow == WindowsEnum.LevelSelectionWindow)
        {
            Show();
            _controller.Initialize(_tempInfo.CurrentStage);
        }
    }

    protected override void OnCloseClicked()
    {
        if (_tempInfo.FirstWinfow == WindowsEnum.LevelSelectionWindow)
            _tempInfo.FirstWinfow = WindowsEnum.None;
        base.OnCloseClicked();
    }
}
