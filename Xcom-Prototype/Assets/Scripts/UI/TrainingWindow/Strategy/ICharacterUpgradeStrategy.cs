using Zenject;

public interface ICharacterUpgradeStrategy
{
    public void Upgrade(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards, PlayerData playerData, SignalBus signalBus);
    public SmalCharacterCard[] GetAvailableSmallCards(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards);
    public bool IsUpgradeAvailable(BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards, PlayerData playerData);
    public int GetXpForLevelUp(BaseCharacterModel character, SmalCharacterCard[] availableSmallCards, PlayerData playerData);
}