using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class DefaultPreBattleState : PreBattleState
{
    public DefaultPreBattleState(PreBattleController controller) : base(controller) 
    {
    }

    #region Initialization
    public override void Initialize()
    {
        if (_controller.TemploaryInfo.CurrentMode.GameMode == GameMode.Default || _controller.TemploaryInfo.CurrentMode.GameMode == GameMode.PvP)
            _controller.InitializeEnemiesIcons();
        else if (_controller.TemploaryInfo.CurrentMode.GameMode == GameMode.ThreeToOne)
            _controller.InitializeBossIcon();

        CreateOrUpdateCharacterButtons();

        _controller.CharacterView.SetDisplayPositionsCount(_controller.TemploaryInfo.CurrentMode.GameMode);
    }

    public override void CreateOrUpdateCharacterButtons()
    {
        ClearCharacterButtons();

        List<BaseCharacterModel> notAssignedCharacters = _controller.PlayerData.PlayerGroup.GetCharactersFromNotAsignedGroup()
            .ToList();

        foreach (var character in notAssignedCharacters)
        {
            CreateAndSetupNotAssignedCharacterButton(character);
        }
    }
    #endregion

    #region Callbacks
    public override void OnClose()
    {
        _controller.SortPanel.OnClose();
        _controller.TemploaryInfo.SelectedCharacters.Clear();

        foreach (CharacterButton button in _controller.ActiveCharacterButtons)
        {
            _controller.CharacterView.ClearCharacterDisplay(button);

            _controller.SelectedCharactersCount = 0;

            _controller.FightWindowPool.RemoveCharacterButton(button);
        }

        _controller.ActiveCharacterButtons.Clear();
        _controller.PlayerData.PlayerGroup.ResetBattleCharacters();
    }

    public override void OnStartButtonClicked()
    {
        if (_controller.SelectedCharactersCount <= 0) return;

        switch (_controller.TemploaryInfo.CurrentMode.GameMode)
        {
            case GameMode.Default:
                _controller.SetupCampaignGameMode();
                break;

            case GameMode.PvP:
                _controller.SetupPvPGameMode();
                break;

            case GameMode.ThreeToOne:
                _controller.SetupThreeToOneGameMode();
                break;
        }     
    }

    public override void OnCharacterButtonClick(CharacterButton button)
    {
        if (_controller.SelectedCharactersCount >= _controller.MaxSelectedCharacters && !button.IsAssigned)
            return;

        if (button.ButtonView.CharacterIcon.isActiveAndEnabled)
        {
            if (!button.IsAssigned)
            {
                button.IsAssigned = true;
                button.ButtonView.DisplayAssignedView();

                _controller.TemploaryInfo.SelectedCharacters.Add(button.CharacterModel);

                _controller.CharacterView.ShowSelectedCharacter(button);

                _controller.SelectedCharactersCount++;
            }
            else
            {
                _controller.TemploaryInfo.SelectedCharacters.Remove(button.CharacterModel);

                button.IsAssigned = false;
                button.ButtonView.DisplayNotAssignedView();

                _controller.CharacterView.ClearCharacterDisplay(button);

                _controller.SelectedCharactersCount--;
            }
        }
    }

    public override void OnSortByNameButtonClick()
    {
        _controller.ActiveCharacterButtons.Sort((x, y) => x.CharacterModel.Name.CompareTo(y.CharacterModel.Name));
        _controller.PreBattleScreen.UpdateCharacterButtonsDisplay(_controller.ActiveCharacterButtons);
    }

    public override void OnSortByRarityButtonClick()
    {
        _controller.ActiveCharacterButtons.Sort((x, y) => x.CharacterModel.Stars.CompareTo(y.CharacterModel.Stars));
        _controller.PreBattleScreen.UpdateCharacterButtonsDisplay(_controller.ActiveCharacterButtons);
    }

    public override void OnSortByLevelButtonClick()
    {
        _controller.ActiveCharacterButtons.Sort((x, y) => x.CharacterModel.Level.CompareTo(y.CharacterModel.Level));
        _controller.PreBattleScreen.UpdateCharacterButtonsDisplay(_controller.ActiveCharacterButtons);
    }
    #endregion

    #region Utility Methods
    private void CreateAndSetupNotAssignedCharacterButton(BaseCharacterModel character)
    {
        CharacterButton button = GameObject.Instantiate(_controller.CharacterButtonPrefab, _controller.CharacterButtonsRoot);
        button.CharacterModel = character;
        _controller.ActiveCharacterButtons.Add(button);
        button.Initialize(character.AvatarId, _controller);
        Action<CharacterButton> clickAction = (btn) => OnCharacterButtonClick(btn);
        button.Subscribe(clickAction);
    }

    private void ClearCharacterButtons()
    {
        foreach (CharacterButton button in _controller.CharacterButtonsRoot.GetComponentsInChildren<CharacterButton>())
        {
            GameObject.Destroy(button.gameObject);
        }

        _controller.ActiveCharacterButtons.Clear();
    }

    #endregion
}
