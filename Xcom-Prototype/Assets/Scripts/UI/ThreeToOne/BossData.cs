using UnityEngine;

public enum BossName
{
    Dragon,
    Griphon,
    Hydra,
    Ogre,
    Cyclops,
    Demon
};

[System.Serializable]
public class BossData : EnemyCharacter
{
    [field: SerializeField] public BossName BossName { get; private set; }

    public BossPreset BossPreset => _bossPreset;
    public Difficulty MaxDifficulty { get; set; } = Difficulty.Mythical;
    public Difficulty Difficulty { get; set; }
    public string SceneName => _sceneName;
    public string[] SkillsIconPaths => _skillsIconPaths;
    public int KeysCost => _keysCost;
    public BossReward[] Rewards => _rewards;
    public float[] DifficiltyStatsMultiplier => _difficultyStatsMultiplier;
    public int[] ExperiencePerBattle => _experiencePerBattle;

    [SerializeField] private BossPreset _bossPreset;
    [SerializeField] private string _sceneName;
    [SerializeField] private string[] _skillsIconPaths;
    [SerializeField] private int _keysCost = 2;
    [SerializeField] private float[] _difficultyStatsMultiplier = new float[5] { 1, 2, 3, 4, 5 };
    [SerializeField] private int[] _experiencePerBattle = new int[5] { 100, 200, 300, 400, 500 };
    [SerializeField] private BossReward[] _rewards = new BossReward[5];
}

public enum Difficulty
{
    Easy,
    Rare,
    Epic,
    Legendary,
    Mythical,
    None
}
