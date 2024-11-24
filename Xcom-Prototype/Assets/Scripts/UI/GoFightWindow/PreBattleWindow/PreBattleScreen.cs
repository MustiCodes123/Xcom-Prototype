using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class PreBattleScreen : UIWindowView
{
    [Inject] private FightWindowDataProvider _dataProvider;
    [SerializeField] private PreBattleController _preBattleController;
    [SerializeField] private TMP_Text _difficultyTMP;
    [SerializeField] private TMP_Text _levelTMP;
    [SerializeField] private GameObject _BG;

    [SerializeField] private AnimateUIElements _uiElements;

    protected override void Awake()
    {
        base.Awake();
        if (_tempInfo.CurrentMode.GameMode is GameMode.PvP or GameMode.ThreeToOne or GameMode.TestOfStrenght)
        {
            int keys = (int)Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Key);
            _levelTMP.gameObject.SetActive(false);
        } 
    }
    public override void Show()
    {
        base.Show();
        _BG.SetActive(true);
        _preBattleController.Initialize();
        _uiElements.AnimatePanelsIn();
        UpdateDifficultyDisplay();
        UpdateLevelDisplay();
    }

    public override void Hide()
    {
        base.Hide();
        _BG.SetActive(false);
        _uiElements.AnimatePanelsOut();
        _preBattleController.OnClose();
        if (_tempInfo.FirstWinfow == WindowsEnum.PreBattleWindow)
            Show();
    }

    public void UpdateCharacterButtonsDisplay(List<CharacterButton> activeCharacterButtons)
    {
        foreach (var button in activeCharacterButtons)
        {
            button.transform.SetAsLastSibling();
        }
    }

    private void UpdateDifficultyDisplay()
    {
        if (_dataProvider.DifficultyColors.TryGetValue(_dataProvider.CurrentDifficulty, out Color difficultyColor))
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(difficultyColor);

            _difficultyTMP.text = $"Difficulty: <color=#{colorHex}>{_dataProvider.CurrentDifficulty}</color>";
        }
        else
        {
            Debug.LogWarning("Color for the selected difficulty level not found. Showing default color.");
            _difficultyTMP.text = $"Difficulty: {_dataProvider.CurrentDifficulty}";
        }
    }

    private void UpdateLevelDisplay()
    {
        if (_dataProvider.Level == 0)
            _dataProvider.Level = _tempInfo.LevelInfo.Id + 1;
        _levelTMP.text = $"Level: {_dataProvider.Level}";
    }

    private void OnDisable()
    {
        _levelTMP.gameObject.SetActive(true);
    }

    protected override void OnCloseClicked()
    {
        if (_tempInfo.FirstWinfow == WindowsEnum.PreBattleWindow)
            _tempInfo.FirstWinfow = WindowsEnum.None;
        base.OnCloseClicked();
    }
}