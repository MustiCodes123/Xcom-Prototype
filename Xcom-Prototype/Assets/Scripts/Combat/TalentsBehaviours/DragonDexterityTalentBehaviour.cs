public class DragonDexterityTalentBehaviour : BaseSkillBehaviour
{
    public DragonDexterityTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _particleFactory = particleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        selfCharacter.SetAnimation(AnimStates.TalentID, (int)Skill.Id);

        IBuff buff = GetBuff(BuffsEnum.DecreaseIncomingHitChance);
        selfCharacter.SkillServiceProvider.AddBuff(buff);

        return base.Use(target, selfCharacter);
    }
}