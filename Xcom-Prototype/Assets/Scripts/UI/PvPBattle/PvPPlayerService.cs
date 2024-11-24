using System;

[Serializable]
public class PvPPlayerService
{
    public int FirstLevelExperience = 100;
    
    public PlayerPvPSavedData Data { get; set; }

    public int NextLevelExp => Fibonacci(Data.LeagueLevel) * FirstLevelExperience;

    public void AddExperience(int count, PvPBattleData battleData, PlayerData playerData)
    {
        if (Data.LeagueLevel == GameConstants.MaxPvPLevel) return;

        Data.Experience += count;
        if (Data.Experience > NextLevelExp)
        {
            Data.Experience -= NextLevelExp;
            Data.LeagueLevel++;
        }
    }

    public void AddScore(int count)
    {
        Data.Score += count;
    }

    private int Fibonacci(int n)
    {
        int a = 0;
        int b = 1;

        if (n == 0 || n == 1) return n += 1;

        for (int i = 0; i < n; i++)
        {
            int temp = a;
            a = b;
            b = temp + b;
        }
        return a;
    }
}

[Serializable]
public class PlayerPvPSavedData
{
    public string Name = "Player";
    public int LeagueLevel;
    public int Experience;
    public int Score;
}
