using UnityEngine;
using Zenject;
using System.Linq;
using System;

public class LevelUpStrategy : ICharacterUpgradeStrategy
{
    private readonly CharacterUpgradePrices _characterUpgradePrices;
    private CharacterHandler _characterHandler;

    [Inject] private SkillsDataInfo _skillsData;
    [Inject]
    public LevelUpStrategy(CharacterUpgradePrices characterUpgradePrices, CharacterHandler characterHandler)
    {
        _characterUpgradePrices = characterUpgradePrices;
        _characterHandler = characterHandler;
    }

    public SmalCharacterCard[] GetAvailableSmallCards(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards)
    {
        return smalCharacterCards.Where(card => !card.IsEmpty).ToArray();
    }

    public bool IsUpgradeAvailable(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards, PlayerData playerData)
    {
        if (character == null || character.IsMaxLevel)
            return false;

        SmalCharacterCard[] availableSmallCards = GetAvailableSmallCards(character, smalCharacterCards);
        if (availableSmallCards.Length == 0)
            return false;

        int xpToAdd = GetXpForLevelUp(character, availableSmallCards, playerData);
        BaseCharacterModel newCharacter = GetNewCharacter(character, xpToAdd);
        int upLevels = Math.Max(newCharacter.Level - character.Level, 1);
        uint price = _characterUpgradePrices.CalculatePrice(character, upLevels);
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
        int xpToAdd = GetXpForLevelUp(character, availableSmallCards, playerData);

        foreach (var card in availableSmallCards)
        {
            playerData.PlayerGroup.DeleteCharacer(card.baseCharacterInfo);
            card.SetCharacterData();
            card.gameObject.SetActive(false);
        }

        character.AddXP(xpToAdd);

        _characterHandler.ChangeCharacter(character);
        signalBus.Fire(new UpgradeSignal { IsCharacterUpgrade = true, PlayerData = playerData, Hero = character });
    }

    public int GetXpForLevelUp(BaseCharacterModel character, SmalCharacterCard[] availableSmallCards, PlayerData playerData)
    {
        int xpToAdd = 0;
        float lvlCoef = 400f;
        float starCoef = 600f;

        foreach (var card in availableSmallCards)
        {
            xpToAdd += (int)(card.baseCharacterInfo.Level * lvlCoef);
            xpToAdd += (int)(card.baseCharacterInfo.Stars * starCoef);
        }

        return xpToAdd;
    }

    private BaseCharacterModel GetNewCharacter(BaseCharacterModel character, int addXP)
    {
        foreach (CharacterPreset charPreset in _skillsData.CharacterPresets)
            if (charPreset.CharacterID == character.CharacterID)
                return charPreset.CreateCharacter(character.Rare, character.Level, character.Xp, addXP);
        return null;
    }
}