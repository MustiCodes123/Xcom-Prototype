
public class StunBuff : BaseBuff, IBuff
{
    public int damage = 0;
    public float duration = 10;

    private IBehaviourState _stunState;
    private IBehaviourState _originState;

    public StunBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _skill = skill;
        duration = _skill.BuffDuration;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;

        _originState = target.CurrentState;
        _stunState = new StunState(target);
        Owner.SetState(_stunState);
        Owner.NavMeshAgent.enabled = false;
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        Owner.NavMeshAgent.enabled = true;
        Owner.SetState(_originState);
    }

    protected override void OnTick()
    {
        base.OnTick();
        Owner.SetAnimation(AnimStates.Idle);
    }
}