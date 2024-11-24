using System;
using System.Collections.Generic;
using System.Linq;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class TrainingCharacterList : MonoBehaviour
{
    private Action<BaseCharacterModel> OnCardCLicked;

    [Inject] private PlayerData _playerData;
    [Inject] private ResourceManager _resourceManager;
    [Inject] private TrainingDataContainer _dataContainer;

    [SerializeField] private Transform _parentForCards;
    [SerializeField] private SmalCharacterCard _cardPrefab;

    private List<SmalCharacterCard> _cards = new List<SmalCharacterCard>();

    public PlayerData PlayerData { get => _playerData; }
    public List<SmalCharacterCard> Cards { get => _cards; }

    public void BackCharacterToList(BaseCharacterModel characterModel)
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].IsEmpty)
            {
                _cards[i].SetCharacterData(characterModel, false, _resourceManager);
                break;
            }
        }
    }

    public void RemoveCharacterFromList(BaseCharacterModel characterModel)
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].baseCharacterInfo == characterModel)
            {
                _cards[i].SetCharacterData();
                _cards[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    public BaseCharacterModel GetFirstCharacter()
    {
        return _cards.FirstOrDefault().baseCharacterInfo;
    }

    public void ShowCharacters(Action<BaseCharacterModel> onCardClicked, BaseCharacterModel characterModel = null)
    {
        OnCardCLicked = onCardClicked;

        var characters = characterModel == null || !characterModel.IsMaxLevel ? ShowAllCharacters() : GetSameStarsCharacters(characterModel);

        UpdateCharacterCards(characters);
        ShowEmptySlots(characters.Count);
    }

    private void ShowEmptySlots(int charactersCount)
    {
        for (var i = charactersCount; i < _playerData.PlayerGroup.MaxGroupSize; i++)
        {
            if (i >= _cards.Count)
            {
                CreateEmptyCard();
            }
            else
            {
                _cards[i].SetCharacterData();
                _cards[i].gameObject.SetActive(true);
            }
        }
    }

    private void UpdateCharacterCards(List<BaseCharacterModel> characters)
    {
        for (var i = 0; i < characters.Count; i++)
        {
            if (i >= _cards.Count)
            {
                CreateNewCard(characters[i]);
            }
            else
            {
                _cards[i].SetCharacterData(characters[i], false, _resourceManager);
                _cards[i].SubscribeToClick(OnCharacterClicked);
                _cards[i].gameObject.SetActive(true);
            }
        }
    }

    private void CreateNewCard(BaseCharacterModel character)
    {
        var card = Instantiate(_cardPrefab, _parentForCards);
        card.SetCharacterData(character, false, _resourceManager);
        card.SubscribeToClick(OnCharacterClicked);
        _cards.Add(card);
    }

    private void CreateEmptyCard()
    {
        var emptyCard = Instantiate(_cardPrefab, _parentForCards);
        emptyCard.SetCharacterData();
        _cards.Add(emptyCard);
    }

    private List<BaseCharacterModel> GetSameStarsCharacters(BaseCharacterModel character)
    {
        List<BaseCharacterModel> listToReturn = new List<BaseCharacterModel>();
        var characters = _playerData.PlayerGroup.GetCharactersFromBothGroup();

        for (var i = 0; i < characters.Count; i++)
        {
            if (character == characters[i])
                continue;

            bool isInSmallCards = _dataContainer.SmallCharacterCards.Any(card => card.baseCharacterInfo == characters[i]);

            if (characters[i].Stars == character.Stars && !isInSmallCards)
            {
                listToReturn.Add(characters[i]);
            }
        }

        return listToReturn;
    }

    public List<BaseCharacterModel> ShowAllCharacters()
    {
        return _playerData.PlayerGroup.GetCharactersFromBothGroup();
    }

    private void OnCharacterClicked(BaseCharacterModel characterModel)
    {
        if (!_dataContainer.SmallCharacterCards.Any(card => card.IsEmpty && !card.IsLocked))
            return;
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].baseCharacterInfo == characterModel)
            {
                _cards[i].SetCharacterData();
                _cards[i].gameObject.SetActive(false);
            }
        }

        OnCardCLicked?.Invoke(characterModel);
    }

    public void ShowRankUpCharacters(BaseCharacterModel character)
    {
        List<BaseCharacterModel> characters = GetSameStarsCharacters(character);
        UpdateCharacterCards(characters);
        ShowEmptySlots(characters.Count);
    }
}
