using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(BattleModeButtonView))]
public class BattleModeButton : MonoBehaviour
{
    [Inject] private FeatureLockConfig _featureLockConfig;

    [SerializeField] private Button _button;
    [SerializeField] private LockableGameFeature _feature;

    [field: SerializeField] public BattleModeButtonView BattleModeButtonView { get; private set; }
    [field: SerializeField] public Mode Mode { get; private set; }

    private void OnEnable()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if(BattleModeButtonView == null)
            BattleModeButtonView = GetComponent<BattleModeButtonView>();

        if(Mode.GameMode != GameMode.Default)
        {
            _button.enabled = false;
        }
    }

    public void Initialize(int currentStage, Action action)
    {
        SetInitialViewStates(currentStage);

        _button.onClick.AddListener(() =>
        {
            action?.Invoke();
        });
    }

    private void SetInitialViewStates(int currentStage)
    {
        FeatureLockConfig.FeatureLock lockSettings = _featureLockConfig.FeatureLocks.FirstOrDefault(cfg => cfg.Feature == _feature);

        switch (lockSettings.IsLocked(currentStage))
        {
            case true:
                BattleModeButtonView.SetLockedState(lockSettings.LockMessage);
                _button.interactable = false;
                break;

            case false:
                BattleModeButtonView.SetUnselectedState();
                _button.interactable = true;
                break;
        }

        if (transform.GetSiblingIndex() == 0)
            BattleModeButtonView.SetSelectedState();
    }
}