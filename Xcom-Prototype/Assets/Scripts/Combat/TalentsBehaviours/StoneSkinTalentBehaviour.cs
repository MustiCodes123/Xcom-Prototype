public class StoneSkinTalentBehaviour : BaseSkillBehaviour
{
    public StoneSkinTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {

    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        base.Use(target, selfCharacter);
        var stoneSkinBuff =  new StoneSkinBuff(_particleFactory, Skill);
        selfCharacter.SkillServiceProvider.AddBuff(stoneSkinBuff);

        var particle = _particleFactory.Create(Skill.ParticleType);
        particle.SetParent(selfCharacter.transform);

        return true;
    }
}