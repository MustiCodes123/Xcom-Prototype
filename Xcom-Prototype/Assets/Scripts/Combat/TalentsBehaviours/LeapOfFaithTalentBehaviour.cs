using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Collections;
using UnityEngine.AI;

public class LeapOfFaithTalentBehaviour : BaseSkillBehaviour
{
    public static Action<Vector3> Jump;

    private List<BaseCharacerView> _targetsInRange = new List<BaseCharacerView>();
    private Vector3 _targetPosition;
    private CharacterController _characterController;
    private Animator _animator;
    private AgentLinkMover _agentLinkMover;

    public LeapOfFaithTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory)
        : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;
        _characterController = _selfCharacter.CharacterController;
        _animator = _selfCharacter.GetComponentInChildren<Animator>();
        _agentLinkMover = _selfCharacter.GetComponent<AgentLinkMover>();

        if (_decale != null)
        {
            target = null;
            _targetPosition = _decale.Target.transform.position;

            _characterController.enabled = false;
            _animator.SetTrigger("LeapOfFaith_Jump");

            Jump?.Invoke(_targetPosition);

            _agentLinkMover.m_Method = OffMeshLinkMoveMethod.Parabola;
            _agentLinkMover.StartCoroutine(PerformJump(_selfCharacter.NavMeshAgent, _targetPosition, 2.0f, 0.5f));

            Observable.Timer(TimeSpan.FromSeconds(0.69f))
                .Subscribe(_ =>
                {
                    OnLanded();
                }).AddTo(selfCharacter);
        }

        return base.Use(target, selfCharacter);
    }

    private void OnLanded()
    {
        _characterController.enabled = true;
        CreateParticleAndProjectileOnLand();

        _animator.SetTrigger("LeapOfFaith_Land");
    }
    private IEnumerator PerformJump(NavMeshAgent agent, Vector3 targetPosition, float height, float duration)
    {
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = targetPosition + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;

        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }


    private void CreateParticleAndProjectileOnLand()
    {
        _particle = _particleFactory.Create(Skill.ParticleType);
        _particle.SetParent(_selfCharacter.transform);
        _particle.transform.position = _selfCharacter.transform.position;

        _projectile = _projectileFactory.Create(Skill.ProjectileType);
        _projectile.Setup(_target, _selfCharacter, _particle, Skill, OnHit, _decale);
        _projectile.SetParent(_selfCharacter.transform);
        _projectile.transform.position = _selfCharacter.transform.position;

        FindTargetsInRange();
        OnHit();
    }

    private void FindTargetsInRange()
    {
        _targetsInRange.Clear();
        Collider[] colliders = Physics.OverlapSphere(_selfCharacter.transform.position, Skill.DamageRange);

        foreach (var collider in colliders)
        {
            BaseCharacerView characerView = collider.GetComponent<BaseCharacerView>();
            if (characerView != null && characerView != _selfCharacter)
            {
                _targetsInRange.Add(characerView);
            }
        }
    }

    private void OnHit()
    {
        foreach (var target in _targetsInRange)
        {
            target.TakeDamage(Skill.Value, AttackType.Physical);
            IBuff deBuff = GetBuff(Skill.OnCollisionBuff);
            target.SkillServiceProvider.AddBuff(deBuff);
        }
    }
}