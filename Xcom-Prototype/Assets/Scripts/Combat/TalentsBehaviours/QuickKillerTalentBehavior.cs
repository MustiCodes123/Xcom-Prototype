public class QuickKillerTalentBehavior : BaseSkillBehaviour
{
    public QuickKillerTalentBehavior(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _particleFactory = particleFactory;
    }
    
    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticle(target, selfCharacter);

        _particle.SetParent(selfCharacter.transform);

        var buff = GetBuff(BuffsEnum.AttackSpeedBuster);
        selfCharacter.SkillServiceProvider.AddBuff(buff);
        return base.Use(target, selfCharacter);
    }

    private void CreateParticle(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;
        var particle = _particleFactory.Create(Skill.ParticleType);
        _particle = particle;
    }
}