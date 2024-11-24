using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CristalButton))]
public class CristalButtonView : MonoBehaviour
{
    [SerializeField] private PortalWindowController _controller;
    [SerializeField] private CristalButton button;

    [SerializeField] private Image _buttonBackground;
    [SerializeField] private Image _cristalPlaceholderBackground;
    [SerializeField] private Image _counterPlaceholderBackground;
    [SerializeField] private Sprite _selectedButton;
    [SerializeField] private Sprite _unselectedButton;
    [SerializeField] private Sprite _selectedPlaceholderBackground;
    [SerializeField] private Sprite _unselectedPlaceholderBackground;
    [SerializeField] private Sprite _selectedCounter;
    [SerializeField] private Sprite _unselectedCounter;
    [SerializeField] private TMP_Text _cristalCountText;

    [field: SerializeField] public CristalButton CristalButton { get; private set; }

    private void Start()
    {
        PlayerInventory.Instance.OnCristalAmountChanged += OnCristalAmountChanged;
    }

    private void OnDestroy()
    {
        PlayerInventory.Instance.OnCristalAmountChanged += OnCristalAmountChanged;
    }

    private void OnEnable()
    {
        if(CristalButton == null)
            CristalButton = GetComponent<CristalButton>();
    }

    public void SwitchButtonView(bool isSelected)
    {
        _buttonBackground.sprite = isSelected ? _selectedButton : _unselectedButton;
        _cristalPlaceholderBackground.sprite = isSelected ? _selectedPlaceholderBackground : _unselectedPlaceholderBackground;
        _counterPlaceholderBackground.sprite = isSelected ? _selectedCounter : _unselectedCounter;
    }

    public void ShowCristalCount(int count)
    {
        _cristalCountText.text = count.ToString();
    }

    private void OnCristalAmountChanged(SummonCristalsEnum cristalsRarity, int cristalAmount)
    {
        if (button.ButtonCristalData.CristalEnum == cristalsRarity)
        {
            _cristalCountText.text = $"{cristalAmount}";
        }
    }
}
