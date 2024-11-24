using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CampLevel", menuName = "Data/CampLevel")]
[Serializable]
public class CampLevel : ScriptableObject
{
    [Header("BattleSceneId")]
    public int Id;
    public string Name;
    public string SceneName;
    public Wave[] Waves;
    public int XP;
    public int Gold;
    public DropSettings[] DropSettings;
    public int EnergyCost = 4;
    public CampLevel nextLevel;
    [SerializeField] private RewardData levelRewards;
    public string BestTime;

    public Stage stage;
    private const string stageRewardsIconsPath = "StageRewardsIcons";


    private bool IsRewardItemExist(string itemName)
    {
        foreach (var rewardItem in levelRewards.RewardItems)
        {
            if (rewardItem.Title == itemName)
                return true;
        }
        return false;
    }

    private void CreateAndAddRewardItem(string title, string description, string iconSpriteName, int amount = 0)
    {
        if (IsRewardItemExist(title))
            return;

        RewardItem rewardItem = new RewardItem
        {
            Item = null,
            Title = title,
            Description = description,
            Amount = amount,
            Icon = Resources.Load<Sprite>($"{stageRewardsIconsPath}/{iconSpriteName.ToLower()}")
        };
        
        if (rewardItem.Icon == null)
        {
            Debug.LogError($"Sprite {iconSpriteName} not found in Resources/{stageRewardsIconsPath}");
            rewardItem.Icon = Resources.Load<Sprite>($"{stageRewardsIconsPath}/unknown");
        }

        levelRewards.RewardItems.Add(rewardItem);
    }

    public RewardData LevelRewards
    {
        get
        {
            if (levelRewards == null)
            {
                levelRewards = new RewardData();
                levelRewards.RewardItems = new List<RewardItem>();

                CreateAndAddRewardItem("Gold", "Gold reward", "money-top", Gold);
                CreateAndAddRewardItem("XP", "XP reward", "exp-top", XP);

                foreach (var dropSetting in DropSettings)
                {
                    foreach (var chance in dropSetting.DropItemChances)
                    {
                        if (dropSetting.CalcDropItemTotalChance(chance) > 0)
                        {
                            string dropItemName = chance.ItemType.ToString().Replace("TH", "").ToLower();
                            CreateAndAddRewardItem(dropItemName, $"{dropItemName} reward", dropItemName);
                        }
                    }

                    if (dropSetting.DropCristalChances?.Any(chance =>
                            dropSetting.CalcDropCristalTotalChance(chance) > 0) == true)
                        CreateAndAddRewardItem("Cristal", "Cristal reward", "cristal");

                    if (dropSetting.DropBrokenChestChances?.Any(chance =>
                            dropSetting.CalcDropBrokenChestTotalChance(chance) > 0) == true)
                        CreateAndAddRewardItem("BrokenChest", "BrokenChest reward", "brokenchest");

                    if (dropSetting.DropChestChances?.Any(chance =>
                            dropSetting.CalcDropChestTotalChance(chance) > 0) == true)
                        CreateAndAddRewardItem("Chest", "Chest reward", "chest");
                }
            }
            return levelRewards;
        }
    }
}

[Serializable]
public struct CampLevelEnemies
{
    public CharacterRace EnemyRace;
    public EnemyStats Stats;
    public CharacterType CharacterType;
}

[Serializable]
public struct Wave
{
    public CampLevelEnemies[] Enemie;
}

[Serializable]
public struct EnemyStats
{
    public int Level;
    public int HP;
    public int MP;
    public float Speed;
    public WeaponItemTemaplate[] Weapons;
    public ArmorItemTemplate[] Armors;
    public int Strength;
    public int Agility;
    public int Intelligence;
    public BaseSkillTemplate[] EnemySkills;
}

