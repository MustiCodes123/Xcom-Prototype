using UnityEngine;

public class BaseSkillBehaviour 
{
    public float cooldown;

    public BaseSkillModel Skill;
    public float AnimationDelay = 0.9f;

    protected BaseDecale _decale;
    protected BaseProjectile _projectile;
    protected BaseParticleView _particle;
    protected BaseCharacerView _target;
    protected BaseCharacerView _selfCharacter;

    protected bool ReadyToUse = true;

    protected BaseParticleView.Factory _particleFactory;
    protected BaseProjectile.Factory _projectileFactory;
    protected BaseDecale.Factory _decaleFactory;
    protected CharactersRegistry _charactersRegistry;

    public BaseSkillBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null)
    {
        Skill = skill;
        ReadyToUse = false;
        this._particleFactory = particleFactory;
        this._charactersRegistry = charactersRegistry;
        this._projectileFactory = projectileFactory;
        cooldown = 0;
        _decaleFactory = decaleFactory;
    }

    public int ManaCost()
    {
        int mana = Skill.ManaCost;

        return mana;

    }

    public bool IsUsable()
    {
        return Skill.IsUssable;
    }   

    public bool IsReady()
    {
        return ReadyToUse;
    }
    public virtual BaseDecale CreateDecale(Transform decalCenter)
    {
        var decal = _decaleFactory.Create(Skill.DecaleType);
        decal.transform.position = decalCenter.transform.position;
        decal.transform.SetParent(decalCenter);
        _decale = decal;
        return decal;
    }

    public virtual BaseParticleView BangBehaveour(BaseCharacerView target, ParticleType particleType)
    {
        var bang = _particleFactory.Create(particleType);
        bang.SetParent(target.transform);
        bang.transform.position = _projectile.transform.position;
        return bang;
    }

    public virtual BaseParticleView BangBehaveour(Vector3 target, ParticleType particleType)
    {
        var bang = _particleFactory.Create(particleType);
        bang.duration = Skill.Duration;
        bang.transform.position = target;
        bang.transform.position = _projectile.transform.position;
        return bang;
    }

    public virtual bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        ReadyToUse = false;
        cooldown = Skill.Cooldown;
        return true;
    }

    public virtual void CustomUpdate()
    {
        if (Skill.Cooldown == 0)
        {
            return;
        }
        
        if (cooldown >= 0)
        {
            cooldown -= Time.deltaTime;

            if(cooldown <= 0)
            {
                ReadyToUse = true;
            }
        }
    }

    public BaseProjectile.Factory GetProjectileFactory()
    {
        return this._projectileFactory;
    }

    public BaseDecale.Factory GetDecale()
    {
        return _decaleFactory;
    }

    protected virtual void CreateParticleAndProjectile(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;
        var particle = _particleFactory.Create(Skill.ParticleType);
        _particle = particle;
        var projectile = _projectileFactory.Create(Skill.ProjectileType);
        _projectile = projectile;
    }

    protected bool IsAutobattle(BaseCharacerView selfCharacter)
    {
        return selfCharacter.AutoBattle;
    }

    protected bool IsTargetInRange(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        float distance = Vector3.Distance(target.transform.position, selfCharacter.transform.position);
        return distance <= Skill.DamageRange;
    }

    protected IBuff GetBuff(BuffsEnum buff)
    {
        switch(buff)
        {
            case BuffsEnum.StunBuff:
                return new StunBuff(_particleFactory, Skill);
            case BuffsEnum.FrostBuff:
                return new FrostDebuff(_particleFactory, Skill);
            case BuffsEnum.GiantBuff:
                return new GiantBuff(_particleFactory, Skill);
            case BuffsEnum.StoneSkinBuff:
                return new StoneSkinBuff(_particleFactory, Skill);
            case BuffsEnum.ColdDebuff:
                return new ColdDebuff(_particleFactory, Skill);
            case BuffsEnum.ElectricDebuff:
                return new ElectricDebuff(_particleFactory, Skill);
            case BuffsEnum.ArmorDebuff:
                return new ArmorDebuff(_particleFactory, Skill);
            case BuffsEnum.HunterDebuff:
                return new HunterDebuff(_particleFactory, Skill);
            case BuffsEnum.MovementAndArmorDebuff:
                return new MovementAndArmorDebuff(_particleFactory, Skill);
            case BuffsEnum.SlowDebuff:
                return new SlowDebuff(_particleFactory, Skill);
            case BuffsEnum.FireArmor:
                return new FireArmorBuff(_particleFactory, Skill);
            case BuffsEnum.IceArmor:
                return new IceArmorBuff(_particleFactory, Skill);
            case BuffsEnum.SpeedBuff:
                return new SpeedBuff(_particleFactory, Skill);
            case BuffsEnum.DefenderBuff:
                return new DefenderBuff(_particleFactory, Skill);
            case BuffsEnum.AttackSpeedBuster:
                return new AttackSpeedBusterBuff(_particleFactory, Skill);
            case BuffsEnum.DecreaseIncomingHitChance:
                return new DecreaseIncomingHitChanceBuff(_particleFactory, Skill);
            default:
                return new BurnDebuff(_particleFactory, Skill);
        }
    }
}
