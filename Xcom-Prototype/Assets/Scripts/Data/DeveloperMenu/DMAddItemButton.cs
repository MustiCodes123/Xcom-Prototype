using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DMAddItemButton : MonoBehaviour
{
    public event Action<IDMData> Click;

    [SerializeField] private Button _button;
    [SerializeField] private DMItem _dmItem;

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

    private void OnClick() => Click?.Invoke(_dmItem.Data);
}
