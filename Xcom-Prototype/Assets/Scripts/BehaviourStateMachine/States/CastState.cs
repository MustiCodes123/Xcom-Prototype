using System.Collections;
using UnityEngine;

public class CastState : IBehaviourState
{
    public State State { get; set; } = State.Cast;

    private float _originSpeed;
    private BaseCharacerView characerView;

    public CastState(BaseCharacerView baseCharacerView)
    {
        characerView = baseCharacerView;
    } 

    public void Enter()
    {
        _originSpeed = characerView.characterData.MoveSpeed;
        characerView.NavMeshAgent.speed = 0;
        Idle();
    }

    public bool Exit()
    {
        characerView.NavMeshAgent.speed = _originSpeed;
        return true;
    }

    public void FixedUpdate()
    {
        Idle();
    }

    public IDamageable GetCurrentTarget()
    {
        throw new System.NotImplementedException();
    }

    public void Idle()
    {
        characerView.SetAnimation(AnimStates.Idle);
    }

    public void SetNewTarget(IDamageable newTarget)
    {
        // throw new System.NotImplementedException();
    }

    public void Update()
    {
    }
}