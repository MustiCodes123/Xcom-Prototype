using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public partial class BaseCharacterModel : IDMData, IStorageItemData
{
    public CharacterID CharacterID;
    public RareEnum Rare;
    public string Name;
    public string BackGroundPath;
    public CharacterRace Race;
    public CharacterBehaviourEnum characterBehaviourEnum;
    public int Stars;
    public int UnasignedStatPoints;
    public Stat[] Stats;

    [JsonIgnore] public Sprite Avatar;
    [JsonIgnore, HideInInspector] public List<ItemOffsetData> ItemsOffsetData = new();
    public string AvatarId;
    public int Xp;
    public int XpToNextLevel;
    public int Level;
    public int Health;
    public int MaxHealth;
    public int AdditionalHP;

    public int BlockChance;
    public float DodgeAdditional;
    public float MagicalResistance;
    public float PhysicalResistance;

    public int Mana;
    public int MaxMana;

    public float LiftingCapacity;

    [JsonIgnore]
    public int Armor;

    [JsonProperty]

    protected int _armor;
    protected int _additionalArmor;

    public int MaxArmor
    {
        set { _armor = value; }

        get
        {
            int result = 0;
            float armorMultiplier = 1.0f;

            if (EquipedItems != null)
            {
                foreach (var item in EquipedItems)
                {
                    if (item is ArmorItem armor)
                    {
                        result += (int)(armor.Armor * (1 + armor.ArmorIncreasePercent));
                    }
                    else if (item is WeaponItem weapon)
                    {
                        result += (int)(weapon.Armor * (1 + weapon.ArmorIncreasePercent));
                    }
                    else if (item is ArmletItem armletItem)
                    {
                        armorMultiplier += armletItem.ArmorBuff;
                    }
                }
            }

            result = Mathf.RoundToInt(result * armorMultiplier);
            result += _additionalArmor;

            return result;
        }
    }

    public float MoveSpeed;
    [JsonIgnore] public float SpeedMultiplier { get; private set; }

    public int Attack;
    public int AdditionalaAttack;
    public float AdditionalAttackSpeed;
    public float AttackSpeed;

    public float Evasion;
    public float CriticalDamage;
    public float CriticalChance;
    public float MagicCriticalDamage;
    public float MagicCriticalChance;

    public int Magic;
    public int MagicDefense;

    public LevelUpParameters LevelUpParameters;

    [JsonIgnore] public List<BaseItem> EquipedItems = new List<BaseItem>();
    [JsonProperty] protected List<int> IdsInventory;
    [JsonProperty] protected List<int> LevelsInventory;
    [JsonProperty] protected List<int> SkillsIds;
    [JsonProperty] protected List<int> ItemSkillsCount;

    [JsonIgnore] public List<ItemSetEnum> ApprovedSets;
    [JsonIgnore] public List<BaseSkillModel> SkillsInUse = new List<BaseSkillModel>();

    public CharacterTalents CharacterTalents;
    public CharacterScore Score;


    protected const float _speedDecreasePercent = 0.05f;

    protected const float _enemyLiftiongCapacity = 1000f;

    public bool IsMaxLevel { get { return Level >= Stars * 10; } }
    public bool IsMaxRank { get { return Rare == RareEnum.Mythical; } }

    public BaseCharacterModel()
    {
        Xp = 0;
        XpToNextLevel = 100;
        Level = 1;
        Health = 1000;
        MaxHealth = 1000;
        MaxMana = 100;
        Mana = 100;

        MoveSpeed = 3.5f;
        LiftingCapacity = 100;
        Attack = 5;
        Armor = 1;
        MaxArmor = 1;
        BlockChance = 10;
        CriticalChance = 0.05f;
        CriticalDamage = 1.5f;
        MagicCriticalChance = 0.1f;
        MagicCriticalDamage = 1.5f;
        MagicalResistance = 5;
        PhysicalResistance = 5;
        Magic = 1;
        MagicDefense = 1;
        CharacterTalents = new CharacterTalents(this);
        Score = new CharacterScore(this);
        Stats = new Stat[3];
        Stats[0] = new Stat() { Param = StatsEnum.Strength, Value = 1 };
        Stats[1] = new Stat() { Param = StatsEnum.Agility, Value = 1 };
        Stats[2] = new Stat() { Param = StatsEnum.Intelligence, Value = 1 };

    }

    public BaseCharacterModel(int level)
    {
        //setup all stats based on level
        Xp = 0;
        XpToNextLevel = 100 + (level * 100);
        Level = level;
        Health = 1000 + (level * 100);
        MaxHealth = Health;
        MaxMana = 100 + (level * 10);
        Mana = MaxMana;
        MoveSpeed = 3.5f;
        LiftingCapacity = 100 * level;
        Attack = level * 2;
        Armor = level;
        MaxArmor = level;
        Magic = level;
        MagicDefense = level;
        CharacterTalents = new CharacterTalents(this);
        Score = new CharacterScore(this);
        Stats = new Stat[3];
        Stats[0] = new Stat() { Param = StatsEnum.Strength, Value = 1 + level };
        Stats[1] = new Stat() { Param = StatsEnum.Agility, Value = 1 + level };
        Stats[2] = new Stat() { Param = StatsEnum.Intelligence, Value = 1 + level };
    }

    public BaseCharacterModel(EnemyStats enemyStats)
    {
        Health = enemyStats.HP;
        Mana = enemyStats.MP;
        MoveSpeed = enemyStats.Speed;
        MaxHealth = enemyStats.HP;
        LiftingCapacity = _enemyLiftiongCapacity;

        foreach (var item in enemyStats.Weapons)
        {
            EquipedItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item, 0));
        }
        foreach (var item in enemyStats.Armors)
        {
            EquipedItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item, 0));
        }
        foreach (var skillTamplate in enemyStats.EnemySkills)
        {
            if (skillTamplate != null)
            {
                SkillsInUse.Add(skillTamplate.GetSkill());
            }
        }

        CharacterTalents = new CharacterTalents(this);

        Score = new CharacterScore(this);
        Stats = new Stat[3];
        Stats[0] = new Stat() { Param = StatsEnum.Strength, Value = enemyStats.Strength };
        Stats[1] = new Stat() { Param = StatsEnum.Agility, Value = enemyStats.Agility };
        Stats[2] = new Stat() { Param = StatsEnum.Intelligence, Value = enemyStats.Intelligence };
    }

    public BaseCharacterModel(CharacterPreset preset, EnemyStats enemyStats)
    {
        this.CharacterID = preset.CharacterID;

        ItemsOffsetData = preset.ItemsOffsetData;
        Debug.LogError($"Name: {Name}; ItemsOffsetData: {ItemsOffsetData.Count}");

        Health = preset.Parameters.Health;
        Mana = preset.Parameters.Mana;
        MoveSpeed = preset.Parameters.MoveSpeed;
        MaxHealth = preset.Parameters.Health;
        LiftingCapacity = _enemyLiftiongCapacity;

        foreach (var item in enemyStats.Weapons)
        {
            EquipedItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item, 0));
        }

        foreach (var item in enemyStats.Armors)
        {
            EquipedItems.Add(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item, 0));
        }

        CharacterTalents = new CharacterTalents(this);
        Score = new CharacterScore(this);
        Stats = new Stat[3];
        Stats[0] = new Stat() { Param = StatsEnum.Strength, Value = enemyStats.Strength };
        Stats[1] = new Stat() { Param = StatsEnum.Agility, Value = enemyStats.Agility };
        Stats[2] = new Stat() { Param = StatsEnum.Intelligence, Value = enemyStats.Intelligence };
    }

    public void AddItemToCharacterInventory(BaseItem item)
    {
        EquipedItems.Add(item);
        CalculateItemsSets();
    }

    public bool AddXP(int xp)
    {
        if (IsMaxLevel)
            return false;

        Xp += xp;
        if (Xp >= XpToNextLevel)
        {
            while (Xp >= XpToNextLevel)
            {
                Xp -= XpToNextLevel;
                LevelUp();

                if (IsMaxLevel)
                    return true;
            }

            return true;
        }
        return false;
    }

    public void RemoveItemFromCharacterInventory(BaseItem item)
    {
        if (EquipedItems.Contains(item))
        {
            EquipedItems.Remove(item);
        }
        CalculateItemsSets();
    }

    public void CalculateSpeed()
    {
        float playerWeight = 0f;
        float totalMovementSpeedBuff = 0f;
        float speedPenalty = 0f;
        float finalCharacterSpeed = 0f;

        foreach (var item in EquipedItems)
        {
            if (item is ArmorItem armor)
            {
                playerWeight += armor.ItemWeight;
            }

            if (item is WeaponItem weapon)
            {
                playerWeight += weapon.ItemWeight;
            }

            if (item is ArmletItem armlet)
            {
                playerWeight += armlet.ItemWeight;
            }

            if (item is AmuletItem amulet)
            {
                totalMovementSpeedBuff += amulet.MovementSpeedBuff;
            }
        }

        if (playerWeight - LiftingCapacity < 0)
        {
            //No Overweight
            MoveSpeed *= (1 + totalMovementSpeedBuff);
            return;
        }
        else
        {
            //Overweight. Calculate speed penalty https://shadoweagle.atlassian.net/wiki/spaces/SD/pages/37552148
            int excessWeight = (int)playerWeight - (int)LiftingCapacity;

            speedPenalty = excessWeight * _speedDecreasePercent;
            finalCharacterSpeed = MoveSpeed - speedPenalty;
            finalCharacterSpeed *= (1 + totalMovementSpeedBuff);

            MoveSpeed = finalCharacterSpeed;

            if (MoveSpeed < 1)
                MoveSpeed = 1f;
        }
    }

    public void CalculateItemsSets()
    {
        if (EquipedItems == null)
            return;

        if (ApprovedSets == null)
        {
            ApprovedSets = new List<ItemSetEnum>();
        }
        else
        {
            ApprovedSets.Clear();
        }

        Dictionary<ItemSetEnum, int> sets = new Dictionary<ItemSetEnum, int>();

        for (int i = 0; i < EquipedItems.Count; i++)
        {
            if (EquipedItems[i] != null && !sets.ContainsKey(EquipedItems[i].SetEnum))
            {
                sets.Add(EquipedItems[i].SetEnum, 1);
            }
            else if (EquipedItems[i] != null)
            {
                sets[EquipedItems[i].SetEnum]++;
            }
        }

        foreach (var item in sets)
        {
            for (int x = 0; x < ItemsDataInfo.Instance.itemSets.Count; x++)
            {
                if (ItemsDataInfo.Instance.itemSets[x].ItemSet == item.Key)
                {
                    ItemSetTemplate template = ItemsDataInfo.Instance.itemSets[x];

                    int numbers = Mathf.FloorToInt((float)item.Value / template.ItemsToSet);
                    if (numbers > 0)
                    {
                        for (int i = 0; i < numbers; i++)
                        {
                            Debug.Log("Set Added " + item.Key);
                            ApprovedSets.Add(item.Key);
                        }
                    }
                }
            }
        }
    }

    public float GetAtackDistance()
    {
        foreach (var item in EquipedItems)
        {
            if (item is WeaponItem)
            {
                var weapon = item as WeaponItem;
                if (weapon.weaponType != WeaponTypeEnum.Wand && weapon.weaponType != WeaponTypeEnum.Bow)
                {
                    return GameConstants.MeleeAttackDistance;
                }
                else
                {
                    return GameConstants.RangeAttackDistance;
                }
            }
        }

        return GameConstants.NoWeaponAttackDistance;
    }

    public void AddPointToStat(StatsEnum stat)
    {
        if (UnasignedStatPoints > 0)
        {
            for (int i = 0; i < Stats.Length; i++)
            {
                if (Stats[i].Param == stat)
                {
                    Stats[i].Value += 1;
                    UnasignedStatPoints -= 1;
                    break;
                }
            }
        }
    }

    public void IncreaseStats(StatsEnum statType, int value)
    {
        for (int i = 0; i < Stats.Length; i++)
        {
            if (Stats[i].Param == statType)
            {
                Stats[i].Value += value;
            }
        }
    }
    public void IncreaseAllStats(int count)
    {
        for (int i = 0; i < Stats.Length; i++)
        {
            Stats[i].Value += count;
        }
    }

    public void AddAdditionlHP(int hp)
    {
        AdditionalHP += hp;
        Health += hp;

        if (Health <= 0)
        {
            Health = 1;
        }
    }

    public void AddAdditionalArmor(int arm)
    {
        Armor += arm;
    }

    public void DamageArmor(int value)
    {
        Armor -= value;
    }

    public void AddResistance(int blockChanse, float magicalResist, float physicalResist)
    {
        BlockChance += blockChanse;
        MagicalResistance += magicalResist;
        PhysicalResistance += physicalResist;

        BlockChance = Math.Clamp(BlockChance, 0, 100);
        MagicalResistance = Math.Clamp(MagicalResistance, 0, 100);
        PhysicalResistance = Math.Clamp(PhysicalResistance, 0, 100);
    }

    public int GetMaxHP()
    {
        int hp = MaxHealth;
        float additionalHPPercent = 0;

        foreach (var item in EquipedItems)
        {
            if (item is ArmorItem)
            {
                var armor = item as ArmorItem;
                hp += armor.Health;
                additionalHPPercent += armor.HealthIncreasePercent;
            }

            if (item is WeaponItem weapon)
            {
                hp += (int)weapon.Health;
                additionalHPPercent += weapon.HealthIncreasePercent;
            }
        }

        foreach (var item in EquipedItems)
        {
            if (item is ArmletItem)
            {
                var armlet = item as ArmletItem;
                hp = Mathf.RoundToInt(hp * (1 + armlet.HealthBuff));
            }
        }

        hp = Mathf.RoundToInt(hp * (1 + additionalHPPercent));

        return hp;
    }

    public int GetMaxMana()
    {
        int mana = MaxMana;

        foreach (var item in EquipedItems)
        {
            if (item is ArmorItem)
            {
                var armor = item as ArmorItem;
                mana += armor.Mana;
            }

            if (item is WeaponItem)
            {
                var weapon = item as WeaponItem;
                mana += weapon.Mana;
            }
        }

        int currentMana = mana;

        foreach (var item in EquipedItems)
        {
            if (item is ArmorItem)
            {
                var armor = item as ArmorItem;
                mana += currentMana * armor.ManaInPercents;
            }

            if (item is WeaponItem)
            {
                var weapon = item as WeaponItem;
                mana += currentMana * weapon.ManaInPercents;
            }

            if (item is RingItem)
            {
                var ring = item as RingItem;
                mana += Mathf.RoundToInt(mana * ring.ManaBuff);
            }
        }

        return mana;
    }

    public float GetMagicalDamageBuff()
    {
        float magicalDamageBuff = 0;

        foreach (BaseItem item in EquipedItems)
        {
            if (item is RingItem)
            {
                RingItem ring = item as RingItem;
                magicalDamageBuff += ring.MagicDamageBuff;
            }
        }

        return magicalDamageBuff;
    }

    public float GetMagicalResistance()
    {
        float magicalResistance = MagicalResistance;

        foreach (var item in EquipedItems)
        {

            if (item is WeaponItem weapon)
            {
                magicalResistance += weapon.MagicalResistance;
            }

            if (item is RingItem)
            {
                RingItem ring = item as RingItem;
                magicalResistance += ring.MagicResistance;
            }

            if (item is ArmorItem)
            {
                ArmorItem armor = item as ArmorItem;
                magicalResistance += armor.MagicalResistance;
            }
        }

        return magicalResistance;
    }

    public float GetManaRegenerationSpeed()
    {
        float manaRegenerationSpeed = 0;

        foreach (BaseItem item in EquipedItems)
        {
            if (item is RingItem)
            {
                RingItem ring = item as RingItem;
                manaRegenerationSpeed += ring.ManaRegenerationRateBuff;
            }
        }

        return manaRegenerationSpeed;
    }

    public float GetManaCostReduction()
    {
        float manaCostReduction = 0;

        foreach (BaseItem item in EquipedItems)
        {
            if (item is RingItem)
            {
                RingItem ring = item as RingItem;
                manaCostReduction += ring.ManaCostReduction;
            }
        }

        return manaCostReduction;
    }

    private void IncreaseParams(LevelUpParameters levelUpParams, float rareIncreasePercent)
    {
        MaxHealth = MaxHealth + (int)(MaxHealth * rareIncreasePercent) + levelUpParams.HealthLevelUpIncrease;
        Health = GetMaxHP();

        MaxMana = MaxMana + (int)(MaxMana * rareIncreasePercent) + levelUpParams.ManaLevelUpIncrease;
        Mana = GetMaxMana();

        Attack = Attack + (int)(Attack * rareIncreasePercent) + levelUpParams.AttackLevelUpIncrease;
        Magic = Magic + (int)(Magic * rareIncreasePercent) + levelUpParams.MagicLevelUpIncrease;

        PhysicalResistance = PhysicalResistance + (PhysicalResistance * rareIncreasePercent) + levelUpParams.PhysicalResistanceLevelUpIncrease;
        MagicalResistance = MagicalResistance + (MagicalResistance * rareIncreasePercent) + levelUpParams.MagicalResistanceLevelUpIncrease;

        CriticalDamage = CriticalDamage + (CriticalDamage * rareIncreasePercent) + levelUpParams.CriticalPhysicalDamageLevelUpIncrease;
        MagicCriticalDamage = MagicCriticalDamage + (MagicCriticalDamage * rareIncreasePercent) + levelUpParams.CriticalMagicalDamageLevelUpIncrease;

        DodgeAdditional = DodgeAdditional + (DodgeAdditional * rareIncreasePercent) + levelUpParams.DodgeAdditionalLevelUpIncrease;

        LiftingCapacity = LiftingCapacity + (LiftingCapacity * rareIncreasePercent) + levelUpParams.LiftingCapacityLevelUpIncrease;
    }

    public void LevelUp()
    {
        var levelUpParams = LevelUpParameters;
        float rareIncreasePercent = levelUpParams.RareToLevelUpCoef[Rare] * 0.01f;
        IncreaseParams(levelUpParams, rareIncreasePercent);

        Level++;
        //Xp = 0;
        XpToNextLevel = (int)(XpToNextLevel + (XpToNextLevel * rareIncreasePercent));

        CharacterTalents.AddTalentPoint((int)this.Rare);
        UnasignedStatPoints += (1 + (int)this.Rare);
    }

    public void RankUp()
    {
        if (IsMaxRank)
            return;

        Rare++;
        Stars++;
        Level = 1;
        Xp = 0;

        var levelUpParams = LevelUpParameters;
        float rareIncreasePercent = levelUpParams.RareToLevelUpCoef[Rare] * 0.01f;
        IncreaseParams(levelUpParams, rareIncreasePercent);
    }

    public bool IsDead()
    {
        return Health <= 0;
    }

    public bool IsSkillWeaponEquip(WeaponSkillEnum weaponSkill)
    {
        switch (weaponSkill)
        {

            case WeaponSkillEnum.TwoHandedWeapon:
                return IsTwoHandedMeleeEquip();
            case WeaponSkillEnum.OneHandedWeapon:
                return IsOneHandedMeleeEquip();
            case WeaponSkillEnum.Shield:
                return IsShieldEquip();
            case WeaponSkillEnum.Bow:
                return IsBowdEquip();
            default:
                return false;
        }
    }

    public int GetStat(StatsEnum stat)
    {
        for (int i = 0; i < Stats.Length; i++)
        {
            if (Stats[i].Param == stat)
            {
                return Stats[i].Value;
            }
        }

        return 0;
    }

    public int GetDamage(AttackType attackType = AttackType.Physical)
    {
        int damage = 0;
        float chanse = RandomChanse();

        if (attackType == AttackType.Physical)
        {
            damage = Attack + AdditionalaAttack + GetWeaponDamage(attackType);

            if (CriticalChance > 0 && chanse <= CriticalChance)
            {
                damage = (int)(damage * CriticalDamage);
            }
        }
        else if (attackType == AttackType.Magical)
        {
            damage = Magic + GetWeaponDamage(attackType);

            if (MagicCriticalChance > 0 && chanse <= MagicCriticalChance)
            {
                damage = (int)(damage * MagicCriticalDamage);
            }
        }

        if (damage < 0)
        {
            damage = 0;
        }

        Score.SetDealedDamage(damage);

        return damage;
    }

    public float GetAccuracy()
    {
        float accuracy = 1;

        foreach (var item in EquipedItems)
        {
            if (item is AmuletItem)
            {
                var amulet = item as AmuletItem;
                accuracy += amulet.AttackAccuracy;
            }
        }

        return accuracy;
    }

    public float GetAttackSpeed()
    {
        AttackSpeed = 1;
        float totalAttackSpeedBuff = 0;

        foreach (var item in EquipedItems)
        {
            if (item is WeaponItem)
            {
                var weapon = item as WeaponItem;
                AttackSpeed += weapon.attackSpeed;
            }
            else if (item is AmuletItem amulet)
            {
                totalAttackSpeedBuff += amulet.AttackSpeedBuff;
            }
        }

        AttackSpeed *= (1 + totalAttackSpeedBuff);
        AttackSpeed += AdditionalAttackSpeed;

        return AttackSpeed;
    }

    protected bool IsTwoHandedMeleeEquip()
    {
        if (EquipedItems.Count != 0)
            foreach (var item in EquipedItems)
            {
                if (item is WeaponItem equipedWeapon)
                {
                    if (equipedWeapon.weaponType == WeaponTypeEnum.TwoHandedAxe ||
                        equipedWeapon.weaponType == WeaponTypeEnum.TwoHandedSword ||
                        equipedWeapon.weaponType == WeaponTypeEnum.TwoHandedHummer ||
                        equipedWeapon.weaponType == WeaponTypeEnum.TwoHandedMace ||
                        equipedWeapon.weaponType == WeaponTypeEnum.Spear ||
                        equipedWeapon.weaponType == WeaponTypeEnum.Staff)
                    {
                        return true;
                    }
                }
            }
        return false;
    }
    protected bool IsOneHandedMeleeEquip()
    {
        if (EquipedItems.Count != 0)
            foreach (var item in EquipedItems)
            {
                if (item is WeaponItem equipedWeapon)
                {
                    if (equipedWeapon.weaponType == WeaponTypeEnum.Sword ||
                        equipedWeapon.weaponType == WeaponTypeEnum.Hummer ||
                        equipedWeapon.weaponType == WeaponTypeEnum.Axe ||
                        equipedWeapon.weaponType == WeaponTypeEnum.Mace)
                    {
                        return true;
                    }
                }
            }
        return false;
    }

    protected bool IsShieldEquip()
    {
        if (EquipedItems.Count != 0)
            foreach (var item in EquipedItems)
            {
                if (item is WeaponItem equipedWeapon)
                {
                    if (equipedWeapon.weaponType == WeaponTypeEnum.Shield)
                    {
                        return true;
                    }
                }
            }
        return false;
    }
    protected bool IsBowdEquip()
    {
        if (EquipedItems.Count != 0)
            foreach (var item in EquipedItems)
            {
                if (item is WeaponItem equipedWeapon)
                {
                    if (equipedWeapon.weaponType == WeaponTypeEnum.Bow)
                    {
                        return true;
                    }
                }
            }
        return false;
    }

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext context)
    {
        EquipedItems = new List<BaseItem>();
        if (ItemsDataInfo.Instance != null && IdsInventory != null)
        {

            for (int i = 0; i < IdsInventory.Count; i++)
            {
                var template = ItemsDataInfo.Instance.GetItemTemplate(IdsInventory[i]);
                var item = ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(template, LevelsInventory[i], false);
                EquipedItems.Add(item);
            }

            int globalSkillID = 0;
            for (int i = 0; i < EquipedItems.Count; i++)
            {
                for (int j = 0; j < ItemSkillsCount[i]; j++)
                {
                    int concretteItemSkillID = SkillsIds[globalSkillID];
                    globalSkillID++;

                    BaseSkillModel skill = SkillsDataInfo.Instance.GetSkillTemplate(concretteItemSkillID).GetSkill();
                    EquipedItems[i].skillModels.Add(skill);
                }
            }
        }

        if (CharacterTalents == null)
        {
            CharacterTalents = new CharacterTalents(this);
        }
    }

    [OnSerializing]
    protected void OnSerializing(StreamingContext context)
    {
        IdsInventory = new List<int>();
        LevelsInventory = new List<int>();
        SkillsIds = new List<int>();
        ItemSkillsCount = new List<int>();

        for (int i = 0; i < EquipedItems.Count; i++)
        {
            if (EquipedItems[i] != null)
            {
                IdsInventory.Add(EquipedItems[i].itemID);
                LevelsInventory.Add(EquipedItems[i].CurrentLevel);

                ItemSkillsCount.Add(EquipedItems[i].skillModels.Count);

                for (int j = 0; j < EquipedItems[i].skillModels.Count; j++)
                {
                    SkillsIds.Add((int)EquipedItems[i].skillModels[j].Id);
                }
            }
        }
    }

    public WeaponTypeEnum GetCurrentWeaponType()
    {
        for (int i = 0; i < EquipedItems.Count; i++)
        {
            if (EquipedItems[i] != null && EquipedItems[i] is WeaponItem weaponItem)
            {
                return weaponItem.weaponType;
            }
        }

        return WeaponTypeEnum.WithOutWeapon;

    }

    public WeaponItem GetWeapon(bool isOffHand)
    {
        for (int i = 0; i < EquipedItems.Count; i++)
        {
            if (EquipedItems[i] != null && EquipedItems[i] is WeaponItem weapon && weapon.IsEquipedInOffHand == isOffHand)
            {
                return weapon;
            }
        }
        return null;
    }

    public List<BaseSkillModel> GetActiveSkills()
    {
        var skills = new List<BaseSkillModel>();

        if (EquipedItems == null) return null;

        for (int i = 0; i < EquipedItems.Count; i++)
        {
            if (EquipedItems[i] != null && EquipedItems[i].skillModels != null)
            {
                for (int x = 0; x < EquipedItems[i].skillModels.Count; x++)
                {
                    var skill = EquipedItems[i].skillModels[x];
                    skills.Add(skill);
                }
            }
        }

        return skills;
    }

    public int GetWeaponDamage(AttackType attackType)
    {
        int weaponDamage = 0;

        foreach (var item in EquipedItems)
        {
            if (item is WeaponItem weapon)
            {
                // weaponDamage += UnityEngine.Random.Range(weapon.minDamage, weapon.maxDamage);
                var averageDamage = (weapon.minDamage + weapon.maxDamage) / 2;
                weaponDamage += averageDamage;
                if (attackType == AttackType.Magical)
                {
                    float magicalDamageIncrease = weaponDamage * GetMagicalDamageBuff();
                    weaponDamage += Mathf.RoundToInt(magicalDamageIncrease);
                }
            }

            if (item is ArmletItem)
            {
                var armlet = item as ArmletItem;

                weaponDamage = Mathf.RoundToInt(weaponDamage * (1 + armlet.DamageBuff));
            }
        }

        return weaponDamage;
    }

    public (int min, int max) GetDamageRange(AttackType attackType = AttackType.Physical)
    {
        int minDamage = Attack + AdditionalaAttack;
        int maxDamage = Attack + AdditionalaAttack;

        foreach (var item in EquipedItems)
        {
            if (item is WeaponItem weapon)
            {
                minDamage += weapon.minDamage;
                maxDamage += weapon.maxDamage;
            }

            if (item is ArmletItem armlet)
            {
                minDamage = Mathf.RoundToInt(minDamage * (1 + armlet.DamageBuff));
                maxDamage = Mathf.RoundToInt(maxDamage * (1 + armlet.DamageBuff));
            }
        }

        if (attackType == AttackType.Magical)
        {
            float magicalDamageBuff = GetMagicalDamageBuff();
            minDamage = Mathf.RoundToInt(minDamage * (1 + magicalDamageBuff));
            maxDamage = Mathf.RoundToInt(maxDamage * (1 + magicalDamageBuff));
        }

        return (minDamage, maxDamage);
    }

    private float RandomChanse()
    {
        var random = new System.Random();
        float result = random.Next(0, 100);
        result = result / 100;

        return result;
    }

    // ***This Method not in use in Model, please, use ResourceManager.LoadSprite(characterModel.AvatarId);***
    public async Task<Sprite> GetItemIcon()
    {
        return Resources.Load<Sprite>($"Avatars/{AvatarId}");
    }

    //TODO: Remove bonuses
    public void AddSetBonusStats(StatsBonus bonusData)
    {
        AdditionalaAttack += Mathf.RoundToInt(AdditionalaAttack * bonusData.PhysicalDamage);

        Magic += Mathf.RoundToInt(Magic * bonusData.MagicDamage);

        MaxHealth += Mathf.RoundToInt(MaxHealth * bonusData.HealthPoints);
        Health = GetMaxHP();

        PhysicalResistance += bonusData.PhysicalResistance;
        PhysicalResistance = Mathf.Clamp(PhysicalResistance, 0, 100);

        MagicalResistance += bonusData.MagicalResistance;
        MagicalResistance = Mathf.Clamp(MagicalResistance, 0, 100);

        CriticalDamage += bonusData.CritDamage;
        MagicCriticalDamage += bonusData.CritDamage;

        CriticalChance += bonusData.CritChance;
        CriticalChance = Mathf.Clamp01(CriticalChance);
        MagicCriticalChance += bonusData.CritChance;
        MagicCriticalChance = Mathf.Clamp01(MagicCriticalChance);

        DodgeAdditional += bonusData.DodgeChance;
        DodgeAdditional = Mathf.Clamp01(DodgeAdditional);
    }
}
