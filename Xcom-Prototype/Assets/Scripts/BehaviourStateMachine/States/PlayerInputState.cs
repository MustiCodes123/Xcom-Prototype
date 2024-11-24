using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlayerInputState : AutoAttackState
{
    private const float StopDistance = 0.5f;
    private const float dragThreshold = 0.2f;
    private Vector3 _movePosition;
    private float _dragTime = 0f;
    private BaseDecale.Factory _decaleFactory;
    [Inject] private IShapeMath _shapeMath;

    public PlayerInputState(BaseCharacerView baseCharacerView, CharactersRegistry charactersRegistry, TemploaryInfo temploaryInfo, BaseDecale.Factory decaleFactory, IShapeMath shapeMath) : base(baseCharacerView, charactersRegistry, temploaryInfo)
    {
        CharacterView = baseCharacerView;
        CharactersRegistry = charactersRegistry;
        _decaleFactory = decaleFactory;
        _shapeMath = shapeMath;
    }

    public override void Update()
    {
        if (CharacterView.AutoBattle)
        {
            base.Update();
            return;
        }
        if (CharacterView.IsSelected)
        {
            if (HitTargetPosition()) return;
        }

        if (Target != null)
        {
            _movePosition = CharacterView.Position;
            if (IsEnoughtDistance())
                Attack();
            else
                Walk();
        }
        else
        {
            if ((CharacterView.Position - _movePosition).magnitude < StopDistance)
                StopMotion();
            else
                StartMotion();

            if (CharacterView.CombatServiceProvider.AttackersHandler.Attackers.Count > 0 && !CharacterView.IsSelected)
            {
                Target = CharacterView.CombatServiceProvider.AttackersHandler.Attackers[0];
            }
        }
    }

    private bool HitTargetPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragTime = 0f;
        }

        if (Input.GetMouseButton(0))
        {
            _dragTime += Time.deltaTime;
            if (_dragTime > dragThreshold)
            {
                return false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_dragTime <= dragThreshold)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        CharacterView.SetTarget(null);
                        _movePosition = hit.point;
                        CharacterView.SetAnimation(AnimStates.Walk);
                        CharacterView.SetDestination(_movePosition);
                        var decal = _decaleFactory.Create(DecaleType.NavPoint);
                        decal.transform.position = hit.point;
                    }
                    else if (_shapeMath.RayCast(ray, out var shape) && shape.GameObject.layer == LayerMask.NameToLayer("Characters"))
                    {
                        var target = shape.GameObject.GetComponent<IDamageable>();
                        if (target != null)
                        {
                            if (target.CharacterView == CharacterView || target.CharacterView.Team == Team.Allies && !target.CharacterView.IsBot)
                            {
                                return true; 
                            }

                            CharacterView.SetTarget(target);
                            target.CharacterView.EnemyIndicator.ShowEnemyIndicator();


                            return true; 
                        }
                    }
                }
            }
        }

        return false;
    }

    private void StartMotion()
    {
        if (CharacterView.NavMeshAgent.isActiveAndEnabled)
        {
            CharacterView.NavMeshAgent.isStopped = false;
            CharacterView.SetAnimation(AnimStates.Walk);
        }

        if (_movePosition != Vector3.zero)
            CharacterView.SetDestination(_movePosition);
    }

    private void StopMotion()
    {
        CharacterView.NavMeshAgent.isStopped = true;
        CharacterView.SetAnimation(AnimStates.Idle);
    }
}
