using System.Collections.Generic;
using UnityEngine;

public class FightWindowDataProvider
{
    public readonly Dictionary<Difficulty, Color> DifficultyColors;

    public List<Sprite> LevelEnemiesIcons { get; set; }
    public List<Sprite> ThreToOneBossIcons { get; set; }

    public int Level { get; set; }
    public Difficulty CurrentDifficulty { get; set; }

    public FightWindowDataProvider()
    {
        DifficultyColors = new Dictionary<Difficulty, Color>
        {
            { Difficulty.Easy, Color.green },
            { Difficulty.Rare, Color.blue },
            { Difficulty.Epic, Color.magenta },
            { Difficulty.Legendary, Color.red },
            { Difficulty.Mythical, Color.red }
        };
    }
}