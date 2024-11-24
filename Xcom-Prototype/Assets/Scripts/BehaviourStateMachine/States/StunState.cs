using UnityEngine;

public class StunState : IBehaviourState
{
    public State State { get; set; } = State.Stun;

    private BaseCharacerView characerView;

    public StunState(BaseCharacerView baseCharacerView)
    {
        characerView = baseCharacerView;
    }

    public void Enter()
    {
        characerView.SetAnimation(AnimStates.Idle);
    }

    public bool Exit()
    {
        characerView.SetAnimation(AnimStates.Idle);
        return true;
    
    }

    public void FixedUpdate()
    {
    }

    public IDamageable GetCurrentTarget()
    {
        throw new System.NotImplementedException();
    }

    public void SetNewTarget(IDamageable newTarget)
    {
       // throw new System.NotImplementedException();
    }

    public void Update()
    {
    }

    public void Walk()
    {
        characerView.SetAnimation(AnimStates.Idle);
    }
}