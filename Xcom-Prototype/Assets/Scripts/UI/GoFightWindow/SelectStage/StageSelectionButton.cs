using Zenject;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StageSelectionButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [field: SerializeField] public BattleModeButtonView BattleModeButtonView { get; private set; }
    [field: SerializeField] public WindowsEnum NextWindow {  get; private set; }
    public Stage ButtonStage { get; set; }
    public bool IsLocked { get; private set; }

    private void OnEnable()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (BattleModeButtonView == null)
            BattleModeButtonView = GetComponent<BattleModeButtonView>();
    }

    public void Initialize(bool isLocked, Action action)
    {
        if (transform.GetSiblingIndex() == 0)
            isLocked = false;

        IsLocked = isLocked;

        SetInitialViewStates(isLocked);

        _button.onClick.RemoveAllListeners();

        _button.onClick.AddListener(() => action?.Invoke());

        _button.interactable = !isLocked;


        BattleModeButtonView.Initialize(ButtonStage);

        transform.SetSiblingIndex(ButtonStage.Id - 1);
    }

    private void SetInitialViewStates(bool isLocked)
    {
        switch (isLocked)
        {
            case true:
                BattleModeButtonView.SetLockedState();
                break;
            case false:
                BattleModeButtonView.SetUnselectedState();
                break;
        }

        if (transform.GetSiblingIndex() == 0)
            BattleModeButtonView.SetSelectedState();
    }
}
