
public class CastedArmorTalentBehaviour : BaseSkillBehaviour
{
    public CastedArmorTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {

    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        base.Use(target, selfCharacter);
        var buff = GetBuff(Skill.OnCollisionBuff);
        selfCharacter.SkillServiceProvider.AddBuff(buff);

        return true;
    }
}