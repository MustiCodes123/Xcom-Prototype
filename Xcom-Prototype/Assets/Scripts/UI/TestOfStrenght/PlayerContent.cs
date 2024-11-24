using System;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContent : MonoBehaviour
{
    public List<BaseCharacterModel> SelectedCharacters => _selectedCharacters;

    [SerializeField] private Image _contentIndexImage;
    [SerializeField] private Button _button;
    [SerializeField] private Transform _characterContainer;
    [SerializeField] private CharacterBattleSlot _battleSlotPrefab;
    [SerializeField] private EmptyCharacterSlot _emptyBattleSlot;

    private List<BaseCharacterModel> _selectedCharacters = new List<BaseCharacterModel>();

    private ResourceManager _resourceManager;

    public void OnSpawn(Sprite levelSprite, Action<PlayerContent> action, ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
        _contentIndexImage.sprite = levelSprite;
        for (int i = 0; i < GameConstants.TestOfStrenghtCharacterLimit - _selectedCharacters.Count; i++)
        {
            Instantiate(_emptyBattleSlot, _characterContainer);
        }
        _button.onClick.AddListener(() =>
        {
            action?.Invoke(this);
        });
    }

    public void RespawnContent()
    {
        Extension.DestroyChilds(_characterContainer);

        foreach (var characterModel in _selectedCharacters)
        {
            var slot = Instantiate(_battleSlotPrefab, _characterContainer);
            slot.Initialize(characterModel, _resourceManager);
        }

        for (int i = 0; i < GameConstants.TestOfStrenghtCharacterLimit - _selectedCharacters.Count; i++)
        {
            Instantiate(_emptyBattleSlot, _characterContainer);
        }
    }

    public bool CanAddCharacter(BaseCharacterModel model)
    {
        return !_selectedCharacters.Contains(model) && _selectedCharacters.Count < GameConstants.TestOfStrenghtCharacterLimit;
    }

    public void RemoveCharacter(BaseCharacterModel model)
    {
        if (_selectedCharacters.Contains(model))
            _selectedCharacters.Remove(model);
    }
}
