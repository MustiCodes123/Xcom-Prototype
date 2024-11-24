using Zenject;

public class RankUpCommand : ICommand
{
    private readonly ICharacterUpgradeStrategy _strategy;
    private readonly BaseCharacterModel _character;
    private readonly SmalCharacterCard[] _smalCharacterCards;
    private readonly PlayerData _playerData;
    private readonly SignalBus _signalBus;

    public RankUpCommand(ICharacterUpgradeStrategy strategy, BaseCharacterModel character, SmalCharacterCard[] smalCharacterCards, PlayerData playerData, SignalBus signalBus)
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
        return 0;
    }
}