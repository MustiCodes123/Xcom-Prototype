
public class FettersDebuff : BaseBuff, IBuff
{
    public int damage = 0;
    public float duration = 10;

    private IBehaviourState _stunState;
    private IBehaviourState _originState;

    public FettersDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _skill = skill;
        duration = _skill.BuffDuration;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _stunState = new StunState(target);
        _originState = target.CurrentState;
        Owner.SetState(_stunState);
        Owner.NavMeshAgent.enabled = false;
        _particle = _particleFactory.Create(_skill.OnCollisionParticle);
        _particle.SetParent(target.transform);
        _particle.duration = _duration;
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        Owner.NavMeshAgent.enabled = true;
        Owner.SetState(_originState);
    }

    protected override void OnTick()
    {
        duration--;
        if (duration <= 0)
        {
            Owner.SkillServiceProvider.RemoveBuff(this);
        }
        Owner.SetAnimation(AnimStates.Idle);
    }
}