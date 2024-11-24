public interface IBehaviourState 
{  
    public void Enter();
    public void Update();
    public void FixedUpdate();
    public bool Exit();

    public State State { get; set; }

    public void SetNewTarget(IDamageable newTarget);
    public IDamageable GetCurrentTarget();    
}

public enum State
{
    Attack,
    Cast,
    Idle,
    Walk,
    Death,
    Stun,
    StartBattle,
    RangeAttack
}


