using UnityEngine;

public class StartBattleState : IBehaviourState
{
    public bool IsReadyToBattle { get; private set; }
    public State State { get; set; } = State.StartBattle;

    private BaseCharacerView _characerView;
    private BattleController _battleController;

    private Vector3 _destinationPoint;
    
    public StartBattleState(BaseCharacerView baseCharacerView,
        Vector3 startBattlePosition,
        BattleController battleController)
    {
        _characerView = baseCharacerView;
        _battleController = battleController;
        _destinationPoint = startBattlePosition;
    }

    public bool Exit() => true;

    public IDamageable GetCurrentTarget()
    {
        throw new System.NotImplementedException();
    }

    public void SetNewTarget(IDamageable newTarget)
    {
        
    }

    public void Update()
    {
        if (_characerView.IsBot || IsReadyToBattle) return;
        
        if ((_characerView.transform.position - _destinationPoint).magnitude > 0.75f)
        {
            Walk();
        }
        else
        {
            IsReadyToBattle = true;
            _battleController.OnCompleteMoveToCombatZone?.Invoke();
        }
    }

    private void Walk()
    {
        _characerView.SetAnimation(AnimStates.Walk);
        if (_characerView.NavMeshAgent.isOnNavMesh)
            _characerView.NavMeshAgent.SetDestination(_destinationPoint);           
    }

    public void Enter()
    {
        if (_characerView.NavMeshAgent.isOnNavMesh)
            _characerView.NavMeshAgent.isStopped = false;
    }

    public void FixedUpdate() { }
}
