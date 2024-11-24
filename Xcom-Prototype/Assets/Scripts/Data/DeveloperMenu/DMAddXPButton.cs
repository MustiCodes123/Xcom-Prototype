using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DMAddXPButton : MonoBehaviour
{
    public event Action<int> Click;

    [SerializeField] private Button _button;
    [SerializeField] private int _addXPValue;

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

    private void OnClick() => Click?.Invoke(_addXPValue);
}
