using Zenject;

public class TalentFactory
{
    private BaseParticleView.Factory particleFactory;
    private BaseProjectile.Factory projectileFactory;
    private BaseDecale.Factory decaleFactory;
    private CharactersRegistry charactersRegistry;

    [Inject]
    public TalentFactory(BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null)
    {
        this.particleFactory = particleFactory;
        this.charactersRegistry = charactersRegistry;
        this.projectileFactory = projectileFactory;
        this.decaleFactory = decaleFactory;
    }


    public BaseSkillBehaviour GetTalent(BaseSkillModel skill)
    {
        switch (skill.Id)
        {
            case TalentsEnum.Health:
                return new HealTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.Mana:
                break;
            case TalentsEnum.MagicArmor:
                return new MagicArmorTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.FireBall:
                return new FireBallTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.IceBall:
                return new CastedBallTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.LightningBall:
                return new LightingBallTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.SummonSkeleton:
                return new SummonTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.Poison:
                return new PoisonTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.Agro:
                return new AgroTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.SingleHpBoost:
                return new SingleHPBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.DamageBuff:
                return new DamageBuffTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.DamageDebuff:
                return new DamageDeBuffTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.Giant:
                return new GiantTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.FireArmor:
                return new CastedArmorTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.IceArmor:
                return new CastedArmorTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.InstantLightning:
                return new CastedArmorTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.Defender:
                return new CastedArmorTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.IceArrow:
                return new IceArrowTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.StoneSkin:
                return new StoneSkinTalentBehaviour(skill, particleFactory, charactersRegistry);
            case TalentsEnum.GlacialPeriod:
                return new GlacialPeriodTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.InstantStrike:
                return new InstantStrikeTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.MeteorRain:
                return new MeteorRainTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.LightningInvocation:
                return new CastedAOETalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireWheel:
                return new FireWheelTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireWave:
                return new CastedWaveTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FrostWave:
                return new CastedWaveTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireWall:
                return new CastedWallTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FrostWall:
                return new FrostWallTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FirePrison:
                return new FirePrisonTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireTraps:
                return new TrapsTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FrostTraps:
                return new TrapsTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.EyeOfGod:
                return new EyeOfGodTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.FireAura:
                return new CastedAuraTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.FreezeAura:
                return new CastedAuraTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.HunterMark:
                return new HunterMarkTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.SonicBoom:
                return new SonicBoomTaletBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.BansheScream:
                return new BansheScreamTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.AttractionSphere:
                return new AttractionSphereTaletnBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireWhip:
                return new FireWhipTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.AssasinCombo:
                return new AssassinComboTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.EarthBall:
                return new EarthBallTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.Swamp:
                return new SwampTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.ElementalBlast:
                return new ElementalBlastTalentBehavior(skill, particleFactory, charactersRegistry, projectileFactory); 
            case TalentsEnum.BullRun:
                return new BullRunTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.ShieldHammer:
                return new ShieldHammerTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.WarriorPunishment:
                return new WarriorPunishmentTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.DragonDexterity:
                return new DragonDexterityTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.FireArrow:
                return new FireArrowTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireThreads:
                return new FireThreadsTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.IceThreads:
                return new IceThreadsTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.QuickKiller:
                return new QuickKillerTalentBehavior(skill, particleFactory, charactersRegistry, projectileFactory); 
            case TalentsEnum.ThrowShield:
                return new ThrowShieldTalent(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.SteelGrip:
                return new SteelGripTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.MagneticWaves:
                return new MagneticWavesTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.FireSpikes:
                return new FireSpikesTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.DanceWithDeath:
                return new DanceWithDeathTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            case TalentsEnum.WaveOfDestruction:
                return new WaveOfDestructionTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.ElementalStakes:
                return new ElementalStakesTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.LeapOfFaith:
                return new LeapOfFaithTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.Manikin:
                return new ManikinTalentBehaviour(skill, particleFactory, charactersRegistry, decaleFactory);
            case TalentsEnum.ThrowOfDeath:
                return new ThrowOfDeathTalentBehavoiur(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.WavesOfArrows:
                return new WavesOfArrowsTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireSpurt:
                return new FireSpurtTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory);
            case TalentsEnum.FireVolley:
                return new FireVolleyTalentBehaviour(skill, particleFactory, charactersRegistry, projectileFactory);
            default:
                break;
        }

        return new BaseSkillBehaviour(skill, particleFactory, charactersRegistry);
    }
}
