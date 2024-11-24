using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestOfStrengthPreBattleState : PreBattleState
{  
    public TestOfStrengthPreBattleState(PreBattleController controller) : base(controller) { }

    #region Initialization
    public override void Initialize()
    {
        _controller.InitializeBossIcon();

        _controller.CharacterView.SetDisplayPositionsCount(_controller.TemploaryInfo.CurrentMode.GameMode);

        CreateOrUpdateCharacterButtons();
    }

    public override void CreateOrUpdateCharacterButtons()
    {
        ClearCharacterButtons();

        List<BaseCharacterModel> selectedGroup = _controller.TemploaryInfo.CurrentPlayerContent.SelectedCharacters
            .ToList();

        List<BaseCharacterModel> notAssignedCharacters = _controller.PlayerData.PlayerGroup.GetCharactersFromNotAsignedGroup()
            .Except(selectedGroup)
            .Except(_controller.TemploaryInfo.SelectedCharacters)
            .ToList();

        foreach (var character in selectedGroup)
        {
            CreateAndSetupSelectedCharacterButton(character);
        }

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

        foreach (CharacterButton button in _controller.ActiveCharacterButtons)
        {
            _controller.CharacterView.ClearCharacterDisplay(button);
            _controller.SelectedCharactersCount = 0;
            _controller.FightWindowPool.RemoveCharacterButton(button);

            button.Unsubscribe();
            GameObject.Destroy(button.gameObject);
        }

        foreach (var character in _controller.TemploaryInfo.SelectedCharacters)
        {
            _controller.PlayerData.PlayerGroup.AddCharacterToNotAsignedGroup(character);
        }

        _controller.TemploaryInfo.SelectedCharacters.Clear();
        _controller.ActiveCharacterButtons.Clear();
    }

    public override void OnStartButtonClicked()
    {
        foreach (var character in _controller.TemploaryInfo.SelectedCharacters)
        {
            if (_controller.TemploaryInfo.CurrentPlayerContent.CanAddCharacter(character))
            {
                _controller.TemploaryInfo.CurrentPlayerContent.SelectedCharacters.Add(character);
            }
        }

        _controller.TemploaryInfo.SelectedCharacters.Clear();
        _controller.UIWindowManager.ShowWindow(WindowsEnum.TestOfStrenghtWindow);
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
                _controller.TemploaryInfo.CurrentPlayerContent.SelectedCharacters.Remove(button.CharacterModel);
                _controller.TemploaryInfo.CurrentPlayerContent.RespawnContent();

                _controller.CharacterView.ShowSelectedCharacter(button);

                _controller.SelectedCharactersCount++;

                _controller.PlayerData.PlayerGroup.AddCharacterToBattleGroup(button.CharacterModel);
                _controller.PlayerData.PlayerGroup.RemoveCharacterFromNotAsignedGroup(button.CharacterModel);
            }
            else
            {
                _controller.TemploaryInfo.SelectedCharacters.Remove(button.CharacterModel);
                _controller.TemploaryInfo.CurrentPlayerContent.SelectedCharacters.Remove(button.CharacterModel);
                _controller.TemploaryInfo.CurrentPlayerContent.RespawnContent();

                button.IsAssigned = false;
                button.ButtonView.DisplayNotAssignedView();

                _controller.CharacterView.ClearCharacterDisplay(button);

                _controller.SelectedCharactersCount--;

                _controller.PlayerData.PlayerGroup.RemoveCharacterFromBattleGroup(button.CharacterModel);
                _controller.PlayerData.PlayerGroup.AddCharacterToNotAsignedGroup(button.CharacterModel);
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

    private void CreateAndSetupSelectedCharacterButton(BaseCharacterModel character)
    {
        CharacterButton button = GameObject.Instantiate(_controller.CharacterButtonPrefab, _controller.CharacterButtonsRoot);
        button.CharacterModel = character;
        _controller.ActiveCharacterButtons.Add(button);
        button.Initialize(character.AvatarId, _controller);
        button.IsAssigned = true;
        button.ButtonView.DisplayAssignedView();
        _controller.CharacterView.ShowSelectedCharacter(button);
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