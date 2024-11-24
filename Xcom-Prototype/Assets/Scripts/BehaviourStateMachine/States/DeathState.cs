using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : IBehaviourState
{
    public State State { get; set; } = State.Death;

    private BaseCharacerView characerView;

    public DeathState(BaseCharacerView baseCharacerView)
    {
        characerView = baseCharacerView;
    }

    public void Enter()
    {
        characerView.SetAnimation(AnimStates.Die);
    }

    public bool Exit() => true;

    public void FixedUpdate()
    {
    }

    public IDamageable GetCurrentTarget()
    {
        throw new System.NotImplementedException();
    }

    public void SetNewTarget(IDamageable newTarget)
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
    }
}
