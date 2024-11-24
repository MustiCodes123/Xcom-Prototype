using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AFPurchaseButton : MonoBehaviour
{
    public Action Click;

    [SerializeField] private TMP_Text _price;

    [field: SerializeField] public Button Button { get; private set; }

    private void OnEnable() => Button.onClick.AddListener(OnClick);

    public void SetPrice(int priveValue) => _price.text = priveValue.ToString();

    public void OnClick() => Click?.Invoke();
}