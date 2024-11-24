using System;
using UnityEngine;
using Zenject;

public class FightWindowController : MonoBehaviour
{
    [Inject] private PlayerData _playerData;
    [Inject] private UIWindowManager _UIWindowManager;
    [Inject] private BattleCampInfo _battleCampInfo;
    [Inject] private TemploaryInfo _temploaryInfo;

    [SerializeField] private LevelSelectController _levelSelectController;
    [SerializeField] private BattleModeButton[] _battleModeButtons;
    [SerializeField] private Transform _stageButtonsRoot;

    private FightWindowPool _fightWindowPool;

    [Inject]
    public void Constructor(FightWindowPool pool)
    {
        _fightWindowPool = pool;
    }

    #region Initialization
    public void Initialize()
    {
        InitializeModeButtons();

        CreateOrUpdateStageButtons();
    }

    private void InitializeModeButtons()
    {
        foreach (BattleModeButton button in _battleModeButtons)
        {
            Action buttonAction = () => OnModeButtonClick(button);
            button.Initialize(_playerData.GetCompanyProgres().Keys.Count, buttonAction);
        }
    }

    private void CreateOrUpdateStageButtons()
    {
        for (int i = 0; i < _battleCampInfo.Stages.Length; i++)
        {
            StageSelectionButton button = _fightWindowPool.SpawnStageButton(_stageButtonsRoot);

            button.gameObject.SetActive(true);

            bool isLocked = !IsStageUnlocked(_battleCampInfo.Stages[i]);

            button.ButtonStage = _battleCampInfo.Stages[i];

            button.Initialize(isLocked, () => OnStageButtonClick(button));
        }
    }
    #endregion

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        Initialize();
    }
    #endregion

    #region Events
    private void OnStageButtonClick(StageSelectionButton button)
    {
        if (!button.IsLocked)
        {
            _levelSelectController.CurrentStage = button.ButtonStage;
            _levelSelectController.Initialize(_temploaryInfo.CurrentStage);

            _temploaryInfo.CurrentStage = button.ButtonStage;
            _UIWindowManager.ShowWindow(button.NextWindow);
        }
    }

    private void OnModeButtonClick(BattleModeButton button)
    {
        if(button.Mode.Window == WindowsEnum.ThreeToOneWindow || button.Mode.Window == WindowsEnum.TestOfStrenghtWindow)
            _UIWindowManager.ShowWindow(WindowsEnum.BossMapWindow);
        else
            _UIWindowManager.ShowWindow(button.Mode.Window);

        _temploaryInfo.CurrentMode = button.Mode;
    }
    #endregion

    #region Utility Methods
    private bool IsStageUnlocked(Stage stage)
    {
        return _playerData.GetCompanyProgres().ContainsKey(stage.Name);
    }
    #endregion
}