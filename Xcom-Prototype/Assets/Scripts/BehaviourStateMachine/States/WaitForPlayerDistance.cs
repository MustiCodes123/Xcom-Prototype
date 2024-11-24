using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPlayerDistance : AutoAttackState
{
    private float distanceToPlayer = 5f;

    public WaitForPlayerDistance(BaseCharacerView baseCharacerView, CharactersRegistry charactersRegistry, TemploaryInfo temploaryInfo) : base(baseCharacerView, charactersRegistry, temploaryInfo)
    {
        CharacterView = baseCharacerView;
        this.CharactersRegistry = charactersRegistry;
        this.TemploaryInfo = temploaryInfo;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public void SetDistanceToPlayer(float distance)
    {
        distanceToPlayer = distance;
    }

    public override void Update()
    {
        if (Target == null)
        {
            FindTarget();
        }
        else if (IsEnoughtDistance())
        {
            Attack();
        }
        else
        {
            float distance = Vector3.Distance(CharacterView.transform.position, Target.Position);
            if (distance > distanceToPlayer)
            {
                Target = null;
            }
            else
            {
                FindTarget();

                if (Target != null)
                {
                    Walk();
                }
                Debug.Log("Distance to player: " + distance);
            }
          
        }
    }

    protected override void FindTarget()
    {
        Target = CharactersRegistry.GetClosestEnemy(CharacterView.Team, CharacterView.transform.position);
        if (Target != null)
        {
            float distance = Vector3.Distance(CharacterView.transform.position, Target.Position);
            if (distance > distanceToPlayer)
            {
                Target = null;
            }
            else
            {
                Debug.Log("Distance to player: " + distance);
            }
        }
    }

    public override bool Exit()
    {
        if (CharacterView.IsDead)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
