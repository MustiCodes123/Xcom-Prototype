using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class UIWindowManager : MonoBehaviour
{
    public Dictionary<WindowsEnum, UIWindowView> Windows = new Dictionary<WindowsEnum, UIWindowView>();
    public WindowsEnum FirstWindow = WindowsEnum.None;

    [Inject] private UIBottomPanel _uIBottomPanel;
    [Inject] private PlayerData _playerData;
    [Inject] private SignalBus _signalBus;
    [Inject] private QuestManager _questManager;
    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private UniRxDisposable _uniRxDisposable;


    private UIWindowView _activeWindow;
    private UIWindowView _previousWindow;

    private void Awake()
    {
        UIWindowView[] windowsArray = GetComponentsInChildren<UIWindowView>(true);
        foreach (var window in windowsArray)
        {
            Windows.Add(window.windowType, window);
            window.Hide();
        }
    }

    private void Start()
    {
        LoadFirstWindow();
    }

    public void ShowPreviousWindow()
    {
        _activeWindow.HideImmediate();
        if (_previousWindow != null)
        {
            _previousWindow.Show();
            _activeWindow = _previousWindow;
        }
    }

    public void ShowWindow(WindowsEnum windowType)
    {
        if (!Windows.ContainsKey(windowType))
        {
            Debug.LogError("Window with name " + windowType + " is not registered in UIWindowManager");
            return;
        }
        UIWindowView windowToActivate = Windows[windowType];
        if (_activeWindow != null && windowToActivate == _activeWindow && _activeWindow.isActiveAndEnabled)
        {
            return;
        }

        if (_activeWindow != null)
        {
            _previousWindow = _activeWindow;
            _activeWindow.Hide();
        }

        if (_uIBottomPanel != null)
            _uIBottomPanel.HideWindow();

        windowToActivate.Show();
        _activeWindow = windowToActivate;
        InfoPopup.Instance.Hide();
    }

    public void ShowWindowWithDelay(WindowsEnum windowType, int delay)
    {
        _uniRxDisposable.OpenWindowTimerDisposable.Clear();

        Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
        {
            ShowWindow(windowType);

        }).AddTo(_uniRxDisposable.OpenWindowTimerDisposable);
    }

    public void HideWindow(WindowsEnum windowName)
    {
        if (!Windows.ContainsKey(windowName))
        {
            Debug.LogError("Window with name " + windowName + " is not registered in UIWindowManager");
            return;
        }
        UIWindowView windowToDeactivate = Windows[windowName];
        windowToDeactivate.Hide();
        _activeWindow = null;
        _uIBottomPanel.ShowWindow();

        InfoPopup.Instance.Hide();
    }

    public void HideAll()
    {
        foreach (var window in Windows.Values)
        {
            window.Hide();
        }
        _activeWindow = null;
        _uIBottomPanel.ShowWindow();

        InfoPopup.Instance.Hide();
    }

    public bool IsAnyWindowActive()
    {
        return _activeWindow != null && _activeWindow.gameObject.activeInHierarchy;
    }

    public SignalBus GetSignalBus()
    {
        return _signalBus;
    }

    private void LoadFirstWindow()
    {
        FirstWindow = _temploaryInfo.FirstWinfow;

        if (_playerData.PlayerGroup.GetCharactersFromBothGroup().Count == 0)
        {
            ShowWindow(WindowsEnum.FirstCharacterWindow);
            _questManager.ResetAllQuests();
        }

        if (FirstWindow != WindowsEnum.None && Windows.ContainsKey(FirstWindow))
        {
            Windows[FirstWindow].ShowWOAnimation();
            FirstWindow = WindowsEnum.None;
        }
    }
}
