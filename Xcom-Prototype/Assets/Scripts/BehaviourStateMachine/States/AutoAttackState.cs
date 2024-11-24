using UnityEngine;

public class AutoAttackState : IBehaviourState
{
    public State State { get; set; } = State.Attack;
    protected BaseCharacerView CharacterView;
    protected CharactersRegistry CharactersRegistry;
    protected IDamageable Target;
    protected float NextAttackTime = 0;
    protected TemploaryInfo TemploaryInfo;
    protected bool IsNewTargetSet = false;

    public AutoAttackState(BaseCharacerView baseCharacerView, CharactersRegistry charactersRegistry, TemploaryInfo temploaryInfo)
    {
        CharacterView = baseCharacerView;
        CharactersRegistry = charactersRegistry;
        TemploaryInfo = temploaryInfo;
    }

    public virtual void Enter()
    {
    }

    public virtual bool Exit() => true;

    public virtual void FixedUpdate()
    {
    }

    public virtual void Update()
    {
        if (IsNewTargetSet || Target == null)
        {
            if (Target != null && Target.CharacterView != null)
            {
                Walk();
            }
            else
            {
                FindTarget();
            }
            IsNewTargetSet = false;
        }
        else if (Target != null && Target.CharacterView != null && IsEnoughtDistance())
        {
            Attack();
        }
        else if (Target != null && Target.CharacterView != null)
        {
            Walk();
        }
        else
        {
            FindTarget();
        }
    }
    public void Walk()
    {
        CharacterView.NavMeshAgent.isStopped = false;
        CharacterView.SetAnimation(AnimStates.Walk);
        CharacterView.SetDestination(Target.Position);
    }

    public virtual void Attack()
    {
        if (Target == null || Target.IsDead)
        {
            Target = null;
            return;
        }

        CharacterView.NavMeshAgent.isStopped = true;
        CharacterView.Target = Target as BaseCharacerView;
        CharacterView.SetAnimation(AnimStates.Idle);
        CharacterView.transform.LookAt(Target.Position);

        if (TemploaryInfo.Autobattle || CharacterView.IsBot)
        {
            if (CharacterView.SkillServiceProvider.IsHaveAvalableSkill() && CharacterView.SkillServiceProvider.IsAvalable)
            {
                CharacterView.SkillServiceProvider.TryUseRandomTalent();
            }
            else
            {
                UseStandartAttack();
            }
        }
        else
        {
            UseStandartAttack();
        }
    }

    protected virtual void UseStandartAttack()
    {
        if (Time.time > NextAttackTime)
        {
            CharacterView.SetDestination(CharacterView.transform.position);
            CharacterView.SetAnimation(AnimStates.Atack);
            NextAttackTime = Time.time + CharacterView.characterData.GetAttackSpeed();
        }
    }

    protected bool IsEnoughtDistance()
    {
        if(Target == null)
        {
            FindTarget();
        }

        var distance = Vector3.Distance(CharacterView.transform.position, Target.Position);
        return distance < CharacterView.characterData.GetAtackDistance();
    }

    protected virtual void FindTarget()
    {
        Target = CharactersRegistry.GetClosestEnemy(CharacterView.Team, CharacterView.transform.position);
    }

    public void SetNewTarget(IDamageable newTarget)
    {
        Target = newTarget;
        IsNewTargetSet = true;
    }

    public IDamageable GetCurrentTarget() => Target;
}