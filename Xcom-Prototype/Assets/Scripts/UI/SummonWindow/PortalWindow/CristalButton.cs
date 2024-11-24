using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CristalButton : MonoBehaviour
{
    public Action<CristalButton, CristalData> Click;

    [SerializeField] private Button _button;

    [field: SerializeField] public CristalData ButtonCristalData { get; private set; }

    private bool _isSelected;

    private void OnEnable()
    {
        if(_button == null)
            _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);

        if(transform.GetSiblingIndex() == 0)
        {
            OnClick();
        }
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);

        ButtonCristalData.gameObject.SetActive(false);
    }

    private void OnClick()
    {
        _isSelected = !_isSelected;

        Click?.Invoke(this, ButtonCristalData);        
    }
}
