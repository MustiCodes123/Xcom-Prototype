using Zenject;

public class LevelUpCommand : ICommand
{
    private readonly ICharacterUpgradeStrategy _strategy;
    private readonly BaseCharacterModel _character;
    private readonly SmalCharacterCard[] _smalCharacterCards;
    private readonly PlayerData _playerData;
    private readonly SignalBus _signalBus;

    public LevelUpCommand(ICharacterUpgradeStrategy strategy, BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards, PlayerData playerData, SignalBus signalBus)
    {
        _strategy = strategy;
        _character = character;
        _smalCharacterCards = smalCharacterCards;
        _playerData = playerData;
        _signalBus = signalBus;
    }

    public void Execute()
    {
        _strategy.Upgrade(_character, _smalCharacterCards, _playerData, _signalBus);
    }

    public bool CanExecute()
    {
        return _strategy.IsUpgradeAvailable(_character, _smalCharacterCards, _playerData);
    }

    public int CalculateXP()
    {
        SmalCharacterCard[] availableSmallCards = _strategy.GetAvailableSmallCards(_character, _smalCharacterCards);
        return _strategy.GetXpForLevelUp(_character, availableSmallCards, _playerData);
    }
    // public int CalculateNextLevel()
    // {
    //     SmalCharacterCard[] availableSmallCards = _strategy.GetAvailableSmallCards(_character, _smalCharacterCards);
    //     int xpToAdd = _strategy.GetXpForLevelUp(_character, availableSmallCards, _playerData);
    //     return _character.Level + _character.CaclulateLevelsToUpFromXP(xpToAdd);
    // }
}