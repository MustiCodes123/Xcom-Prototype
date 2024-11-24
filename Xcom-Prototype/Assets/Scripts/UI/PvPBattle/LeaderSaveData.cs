using System;

[Serializable]
public class LeaderSaveData
{
    public int Level;
    public int Score;

    public LeaderSaveData(int level, int score)
    {
        Level = level;
        Score = score;
    }
}
