public class DecreaseIncomingHitChanceBuff : BaseBuff
{
    private float _dodgeChanceIncrease;

    public DecreaseIncomingHitChanceBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleType = ParticleType.DoTShadow;
        _particleFactory = particleFactory;
        _icon = skill.BuffIcon;
        _duration = skill.BuffDuration;
        _dodgeChanceIncrease = skill.Value * 0.01f;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        base.OnApply(target);
        Owner.characterData.DodgeAdditional += _dodgeChanceIncrease;
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        base.OnRemove(target);
        Owner.characterData.DodgeAdditional -= _dodgeChanceIncrease;
    }
}