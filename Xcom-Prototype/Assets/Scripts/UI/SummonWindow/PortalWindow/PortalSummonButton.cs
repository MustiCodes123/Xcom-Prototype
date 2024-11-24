using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PortalSummonButton : MonoBehaviour
{
    public Action<PortalSummonButton> Click;

    [SerializeField] private Button _button;

    private void OnEnable()
    {
        if(_button == null)
            _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        Click?.Invoke(this);
    }

    public Button GetButton()
    {
        return _button;
    }
}
