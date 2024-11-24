public class LockedState : ILevelButtonViewState
{
    public void ApplyState(LevelButtonView button)
    {
        button.ChangeView(button.LockedConfig);
    }
}

public class LockedStageButtonState : IModeButtonViewState
{
    public void ApplyState(BattleModeButtonView button)
    {
        button.ChangeView(button.LockedConfig);
    }
}