using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BossSelectionButton : MonoBehaviour
{
    public Action<BossData> Click;

    [SerializeField] private BossData _bossData;

    private Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick() => Click?.Invoke(_bossData);
}