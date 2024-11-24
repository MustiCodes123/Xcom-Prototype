using UnityEngine;
using Zenject;
using System.Linq;

public class RankUpStrategy : ICharacterUpgradeStrategy
{
    private readonly CharacterUpgradePrices _characterUpgradePrices;
    private CharacterHandler _characterHandler;

    [Inject]
    public RankUpStrategy(CharacterUpgradePrices characterUpgradePrices, CharacterHandler characterHandler)
    {
        _characterUpgradePrices = characterUpgradePrices;
        _characterHandler = characterHandler;
    }

    public SmalCharacterCard[] GetAvailableSmallCards(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards)
    {
        int charactersNeededAmount = character.Stars;
        SmalCharacterCard[] availableSmallCards = smalCharacterCards
            .Where(card => !card.IsEmpty && card.baseCharacterInfo.Rare == character.Rare)
            .Take(charactersNeededAmount)
            .ToArray();
            
        if (availableSmallCards.Length < charactersNeededAmount)
            return new SmalCharacterCard[0];
        return availableSmallCards;
    }

    public bool IsUpgradeAvailable(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards, PlayerData playerData)
    {
        if (character == null || !character.IsMaxLevel || character.IsMaxRank)
            return false;
            
        SmalCharacterCard[] availableSmallCards = GetAvailableSmallCards(character, smalCharacterCards);
        if (availableSmallCards.Length == 0)
            return false;

        uint price = _characterUpgradePrices.CalculatePrice(character);
        return Wallet.Instance.IsEnoughCachedCurrency(GameCurrencies.Gold, price);
    }

    public void Upgrade(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards, PlayerData playerData, SignalBus signalBus)
    {
        if (!IsUpgradeAvailable(character, smalCharacterCards, playerData))
            return;

        uint price = _characterUpgradePrices.CalculatePrice(character);
        if (!Wallet.Instance.SpendCachedCurrency(GameCurrencies.Gold, price))
            return;

        SmalCharacterCard[] availableSmallCards = GetAvailableSmallCards(character, smalCharacterCards);
        foreach (var card in availableSmallCards)
        {
            playerData.PlayerGroup.DeleteCharacer(card.baseCharacterInfo);
            card.SetCharacterData();
            card.gameObject.SetActive(false);
        }

        character.RankUp();

        _characterHandler.ChangeCharacter(character);
        signalBus.Fire(new UpgradeSignal { IsCharacterUpgrade = true, PlayerData = playerData, Hero = character });
    }

    public int GetXpForLevelUp(BaseCharacterModel character, SmalCharacterCard[] availableSmallCards, PlayerData playerData)
    {
        return 0;
    }
}
