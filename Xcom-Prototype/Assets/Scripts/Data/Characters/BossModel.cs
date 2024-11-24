using System.Collections;
using UnityEngine;

public class BossModel : BaseCharacterModel
{
    public BossLevelUpModel BossLevelUpModel;
    public float LevelUpFactor;
    public BossModel(BossData bossData, int difficulty)
    {
        BossLevelUpModel = bossData.BossPreset.LevelUpModel;

        Attack = (int)(bossData.BossPreset.BossStats[difficulty].Damage);
        Magic = (int)(bossData.BossPreset.BossStats[difficulty].Magic);
        MagicCriticalDamage = (float)(bossData.BossPreset.BossStats[difficulty].CriticalMagicDamage);
        CriticalDamage = (float)(bossData.BossPreset.BossStats[difficulty].CriticalPhysicalDamage);
        Mana = (int)(bossData.BossPreset.BossStats[difficulty].MP);
        MoveSpeed = bossData.BossPreset.BossStats[difficulty].MoveSpeed;
        LiftingCapacity = _enemyLiftiongCapacity;
        MaxHealth = (int)(bossData.BossPreset.BossStats[difficulty].HP);
        AdditionalAttackSpeed = -(int)(bossData.BossPreset.BossStats[difficulty].AttackSpeed);
        BlockChance = (int)(bossData.BossPreset.BossStats[difficulty].BlockChanse);
        PhysicalResistance = (float)(bossData.BossPreset.BossStats[difficulty].PhysicalResistance);
        MagicalResistance = (float)(bossData.BossPreset.BossStats[difficulty].MagicalResistance);

        DodgeAdditional = (float)(bossData.BossPreset.BossStats[difficulty].DodgeAdditional);
        _additionalArmor = ((int)(bossData.BossPreset.BossStats[difficulty].Armor));

        CharacterTalents = new CharacterTalents(this);
        CharacterTalents.GenerateTalents(bossData.BossPreset.Talents);

        Stats = new Stat[3];
        Stats[0] = new Stat() { Param = StatsEnum.Strength, Value = 1 };
        Stats[1] = new Stat() { Param = StatsEnum.Agility, Value = 1 };
        Stats[2] = new Stat() { Param = StatsEnum.Intelligence, Value = 1 };

        IncreaseStats(StatsEnum.Strength, (int)(bossData.EnemyStats.Strength * difficulty));
        IncreaseStats(StatsEnum.Agility, (int)(bossData.EnemyStats.Agility * difficulty));
        IncreaseStats(StatsEnum.Intelligence, (int)(bossData.EnemyStats.Intelligence * difficulty));
    }

    public void BossStatsIncrease()
    {
        float levelUpPercent = LevelUpFactor * 0.01f + 1;

        MaxHealth = (int)(MaxHealth * levelUpPercent) + MaxHealth + BossLevelUpModel.AdditionalHealth;
        Health = MaxHealth;
       
        Attack = (int)(Attack * levelUpPercent) + Attack + BossLevelUpModel.AdditionalAttack;
        Mana = (int)(Mana * levelUpPercent) + Mana + BossLevelUpModel.AdditionalMana;

        if (PhysicalResistance < BossLevelUpModel.LimitPhysicalResistance)
        {
            PhysicalResistance = (PhysicalResistance * levelUpPercent) + PhysicalResistance + BossLevelUpModel.AdditionalPhysicalResistance;
        }
        else
        {
            PhysicalResistance = BossLevelUpModel.LimitPhysicalResistance;
        }

        if(MagicalResistance < BossLevelUpModel.LimitMagicalResistance)
        {
            MagicalResistance = (MagicalResistance * levelUpPercent) + MagicalResistance + BossLevelUpModel.AdditionalMagicalResistance;
        }
        else
        {
            MagicalResistance = BossLevelUpModel.LimitMagicalResistance;
        }
    }
}