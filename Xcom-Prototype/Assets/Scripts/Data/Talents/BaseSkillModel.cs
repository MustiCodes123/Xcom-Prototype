using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class BaseSkillModel : IDMData
{
    public TalentsEnum Id;
    public ParticleType ParticleType;
    public ProjectileType ProjectileType;
    public DecaleType DecaleType;
    public ElementsEnum Element;
    public WeaponSkillEnum WeaponSkill;
    public bool IsUssable;
    public string Name;
    public string Description;
    [JsonIgnore] public Sprite Icon;
    [JsonIgnore] public Sprite BuffIcon;
    public int Level;
    public int Cost;
    public float Cooldown;
    public int Duration;
    public AttackType DamageType;
    public int Value;
    public int ManaCost;
    public float DamageRange;
    public ParticleType OnCollisionParticle;
    public BuffsEnum OnCollisionBuff;
    public AttackType BuffDamageType;
    public int BuffPreiodDamage;
    public int BuffDuration;
    public bool isInUse;


    public BaseSkillModel GetSkill()
    {
        BaseSkillModel skill = new BaseSkillModel
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Icon = Icon,
            Level = Level,
            Cost = Cost,
            ParticleType = ParticleType,
            ProjectileType = ProjectileType,
            Cooldown = Cooldown,
            ManaCost = ManaCost,
            Value = Value,
            IsUssable = IsUssable,
            Duration = Duration,
            BuffIcon = BuffIcon,
            WeaponSkill = WeaponSkill,
            DecaleType = DecaleType,
            OnCollisionParticle = OnCollisionParticle,
            OnCollisionBuff = OnCollisionBuff,
            DamageType = DamageType,
            BuffDamageType = BuffDamageType,
            BuffPreiodDamage = BuffPreiodDamage,
            BuffDuration = BuffDuration,
            isInUse = isInUse
        };

        return skill;
    }

    public SlotEnum GetWeaponSlotBySkillWeapon(WeaponSkillEnum waponSkillType)
    {
        switch(waponSkillType)
        {
            case WeaponSkillEnum.OneHandedWeapon:
                return SlotEnum.Weapon;
            case WeaponSkillEnum.TwoHandedWeapon:
                return SlotEnum.Weapon;
            case WeaponSkillEnum.None:
                return SlotEnum.Weapon;
            case WeaponSkillEnum.Shield:
                return SlotEnum.Shield;
            case WeaponSkillEnum.Wand:
                return SlotEnum.Weapon;
            case WeaponSkillEnum.Spear:
                return SlotEnum.Weapon;
            case WeaponSkillEnum.Bow:
                return SlotEnum.Bow;
            case WeaponSkillEnum.Stuff:
                return SlotEnum.Weapon;
            default:
                return SlotEnum.Weapon;
        }
    }
}
