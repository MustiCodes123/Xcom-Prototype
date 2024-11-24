using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class UIBottomPanel : MonoBehaviour
{
    [Inject] private UIWindowManager _windowManager;

    [SerializeField] private Transform _showPosition;
    [SerializeField] private Transform _hidePosition;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] List<UIBottomPanelButton> _buttons;

    private void Start()
    {
        foreach (var button in _buttons)
        {
            button.OnClick += () => _windowManager.ShowWindow(button.WindowsEnum);
        }
    }

    public void ShowWindow()
    {
        if (transform.position == _showPosition.position)
        {
            return;
        }
        transform.DOKill(true);
        transform.DOMove(_showPosition.position, animationDuration);
    }

    public void HideWindow()
    {
        if (transform.position == _hidePosition.position)
        {
            return;
        }
        transform.DOKill(true);
        transform.DOMove(_hidePosition.position, animationDuration);
    }


    private void OnDestroy()
    {
        foreach (var button in _buttons)
        {
            button.OnClick -= () => _windowManager.ShowWindow(button.WindowsEnum);
        }
    }
}
