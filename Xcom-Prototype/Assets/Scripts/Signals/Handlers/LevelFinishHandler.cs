using Zenject;

public class LevelFinishHandler 
{
    private SignalBus signalBus;

    public LevelFinishHandler(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    public void LevelComplete(Stage stage, CampLevel campLevel, GameMode mode, bool isWin, int heroCount)
    {
        signalBus.Fire(new LevelFinishSignal() {Stage = stage, campLevel = campLevel, GameMode = mode, IsWin = isWin, HeroCount = heroCount });
    }
}
