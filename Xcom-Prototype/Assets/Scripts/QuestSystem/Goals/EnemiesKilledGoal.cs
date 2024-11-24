
public class EnemiesKilledGoal : QuestGoal
{
    public CharacterRace characterRace;
    
    public override void Process(ISignal signal)
    {
        if (signal is LevelFinishSignal)
        {
            LevelFinishSignal levelSignal = (LevelFinishSignal)signal;
            if (levelSignal.GameMode != GameMode.PvP)
            {
                if (characterRace == CharacterRace.Dummy)
                {
                    foreach(Wave wave in levelSignal.campLevel.Waves)
                    {
                        CurrentAmount += wave.Enemie.Length;
                    }
                    //CurrentAmount += levelSignal.campLevel.Enemies.Length;
                }
                else
                {
                    foreach(Wave wave in levelSignal.campLevel.Waves)
                    {
                        foreach(var enemy in  wave.Enemie)
                        {
                            if(enemy.EnemyRace == characterRace)
                            {
                                CurrentAmount++;
                            }
                        }
                    }
                    /*
                    foreach (var enemy in levelSignal.campLevel.Enemies)
                    {
                        if (enemy.EnemyRace == characterRace)
                        {
                            CurrentAmount++;
                        }
                    }*/
                }
            }
        }
    }
}
