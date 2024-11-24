using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartBattleButton : MonoBehaviour
{
    public Action<CampLevel> Click;

    [SerializeField] private Button _button;

    private CampLevel _levelData;

    [field: SerializeField] public Image Background { get; set; }

    public void Initialize(CampLevel levelData)
    {
        if(_button == null)
            _button = GetComponent<Button>();

        _levelData = levelData;

        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable() => _button.onClick.RemoveListener(OnClick);

    private void OnClick() => Click?.Invoke(_levelData);
}
