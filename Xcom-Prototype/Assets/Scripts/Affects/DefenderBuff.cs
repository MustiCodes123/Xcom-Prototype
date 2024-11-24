
public class DefenderBuff : BaseBuff, IBuff
{
    private int _resistanceIncrease;
    public DefenderBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _skill = skill;
        _icon = skill.BuffIcon;
        _resistanceIncrease = skill.Value;
        _duration = skill.Duration;
        buffType = BuffsEnum.DefenderBuff;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        Owner.HealthBar.SetBuffOnBar(_icon);
        Owner.characterData.AddResistance(0, 0, _resistanceIncrease);
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        Owner.HealthBar.RemoveBuufOnBar(_icon);
        Owner.characterData.AddResistance(0, 0, -_resistanceIncrease);
    }
}