

public class LevelFinishGoal : QuestGoal
{
    public bool IsNeedToWin;
    public bool IsRandomGameMode;
    public int HeroCount;
    public GameMode Mode;
    public CampLevel CampLevel;

    public override void Process(ISignal signal)
    {
        if (signal is LevelFinishSignal)
        {

            LevelFinishSignal levelFinishSignal = (LevelFinishSignal)signal;
            if (Mode == levelFinishSignal.GameMode && levelFinishSignal.campLevel == CampLevel && IsNeedToWin && levelFinishSignal.IsWin)
            {
                CurrentAmount++;
            }

            else if (Mode == levelFinishSignal.GameMode && CampLevel == null && IsNeedToWin && levelFinishSignal.IsWin)
            {
                CurrentAmount++;
            }

            else if (Mode == levelFinishSignal.GameMode && !IsNeedToWin)
            {
                CurrentAmount++;
            }

            else if (HeroCount > 0 && HeroCount == levelFinishSignal.HeroCount && Mode == levelFinishSignal.GameMode)
            {
                CurrentAmount++;
            }
        }
    }

    public void SetNewMode(GameMode mode)
    {
        Mode = mode;
    }
}
