using UnityEngine;

public class BaseSkillTemplate : MonoBehaviour
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
    public Sprite Icon;
    public Sprite BuffIcon;
    public int Level;
    public int Cost;
    public float Cooldown;
    public int Duration;
    public AttackType DamageType;
    public int Value;
    public int ManaCost;
    public int BuyPrice;
    public float DamageRange;
    public ParticleType OnCollisionParticle;
    public BuffsEnum OnCollisionBuff;
    public AttackType BuffDamageType;
    public int BuffPreiodDamage;
    public int BuffDuration;

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
            Value = Value,
            ManaCost = ManaCost,
            IsUssable = IsUssable,
            Duration = Duration,
            Element = Element,
            DamageRange = DamageRange,
            BuffIcon = BuffIcon,
            WeaponSkill = WeaponSkill,
            DecaleType = DecaleType,
            OnCollisionParticle = OnCollisionParticle,
            OnCollisionBuff = OnCollisionBuff,
            DamageType = DamageType,
            BuffDamageType = BuffDamageType,
            BuffPreiodDamage = BuffPreiodDamage,
            BuffDuration = BuffDuration

        };
        return skill;
    }
}
