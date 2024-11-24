using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using UnityEngine.SceneManagement;

public class TestOfStrenghtWindow : BossWindowView
{
    [SerializeField] private Transform _selectedContentContainer;
    [SerializeField] private Transform _freeCharactersContainer;
    [SerializeField] private CharacterBattleSlotButton _characterSlotPrefab;
    [SerializeField] private EmptyCharacterSlot _emptyCharacterSlotPrefab;
    [SerializeField] private GameObject _characterSelectMenu;
    [SerializeField] private Button _showMainWindowButton;

    [SerializeField] private Sprite[] _playerContentIndexSprites;
    [SerializeField] private PlayerContent _playerContentPrefab;
    [SerializeField] private Transform _playerContentContainer;

    private List<PlayerContent> _playerContents = new List<PlayerContent>();
    private PlayerContent _currentPlayerContent { get; set; }

    [Inject] private CharacterHandler _characterHandler;

    public override void Show()
    {
        base.Show();
        CreatePlayerContent();
        _showMainWindowButton.onClick.AddListener(() => ShowMainWindow());
    }

    private void OnEnable()
    {
        if(_currentPlayerContent != null)
            _currentPlayerContent.RespawnContent();
    }

    public void SelectBattleSlot(CharacterBattleSlotButton battleSlotButton)
    {       
        foreach (Transform child in _selectedContentContainer)
        {
            if (child.GetComponent<EmptyCharacterSlot>())
            {
                int index = child.GetSiblingIndex();               
                Destroy(child.gameObject);

                battleSlotButton.transform.parent = _selectedContentContainer;
                battleSlotButton.transform.SetSiblingIndex(index);
                break;
            }
        }
        _characterHandler.SetCharacterToGroup(battleSlotButton.Model, GroupType.Battle);
    }

    public void DeselectBattleSlot(CharacterBattleSlotButton battleSlotButton)
    {
        battleSlotButton.transform.parent = _freeCharactersContainer;
        Instantiate(_emptyCharacterSlotPrefab, _selectedContentContainer);
        _characterHandler.SetCharacterToGroup(battleSlotButton.Model, GroupType.None);
    }

    private void CreatePlayerContent()
    {
        for (int i = 0; i < GameConstants.TestOfStrenghtTeamCountLimit; i++)
        {
            var playerContent = Instantiate(_playerContentPrefab, _playerContentContainer);
            playerContent.OnSpawn(_playerContentIndexSprites[i], (x) => ShowCharacterSelectMenu(playerContent), _resourceManager);
            _playerContents.Add(playerContent);
        }
    }

    private void ShowMainWindow()
    {
        _characterSelectMenu.gameObject.SetActive(false);
        _currentPlayerContent.RespawnContent();
    }

    private void ShowCharacterSelectMenu(PlayerContent playerContent)
    {
        _currentPlayerContent = playerContent;
        TemploaryInfo.CurrentPlayerContent = playerContent;

        UIWindowManager.ShowWindow(WindowsEnum.PreBattleWindow);
    }

    public override void Hide()
    {
        base.Hide();
        // gameObject.SetActive(false);
    }

    public override void StartBattle()
    {
        TemploaryInfo.SelectedCharacters.Clear();
        TemploaryInfo.SelectedCharacterGroups.Clear();
        int index = 0;
        foreach (var content in _playerContents)
        {
            if (content.SelectedCharacters.Count > 0)
            {
                TemploaryInfo.SelectedCharacterGroups.Add(index, content.SelectedCharacters);
                index++;
            }
        }

        if (index > 0)
        {
            PlayerData.PlayerGroup.ResetBattleCharacters();
            bool isEnoughKeys = Wallet.Instance.SpendCachedCurrency(GameCurrencies.Key, (uint)KeysCost);

            if (isEnoughKeys)
            {
                _resourceManager.ShowLoadingScreen();
                _resourceManager.LoadLevelAsync("TestOfStrenghtArenaPvE");
            }
            else
                Debug.Log("Not enough keys!");
        }
    }
}
