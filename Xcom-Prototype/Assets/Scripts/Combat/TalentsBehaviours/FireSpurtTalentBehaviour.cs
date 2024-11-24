using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class FireSpurtTalentBehaviour : BaseSkillBehaviour
{
    private int _damage;
    private float _knockbackForce;
    private float _speed;
    
    public FireSpurtTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) 
        : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;

        _damage = skill.Value;
        _knockbackForce = 3f;
        _speed = Skill.DamageRange;
        AnimationDelay = 0.5f;
    }
    
    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        if (_decale != null)
        {
            var direction = (_decale.Target.transform.position - selfCharacter.transform.position).normalized;
            var destination = GetValidDestination(selfCharacter.transform.position, direction, _speed);

            MoveCharacter(selfCharacter, destination);

            _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

            _projectile.SetParent(selfCharacter.transform);
            _particle.SetParent(selfCharacter.transform);
            _particle.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
        }
        else
        {
            var direction = (target.transform.position - selfCharacter.transform.position).normalized;
            var destination = GetValidDestination(selfCharacter.transform.position, direction, _speed);

            MoveCharacter(selfCharacter, destination);

            _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

            _projectile.SetParent(selfCharacter.transform);
            _particle.SetParent(selfCharacter.transform);
            _particle.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
        }
        return base.Use(target, selfCharacter);
    }


    private void OnHit()
    {
        _target = _projectile.GetTarget();

        BangBehaveour(_target, Skill.OnCollisionParticle);
        var difference = new Vector3(_projectile.transform.position.x - _target.transform.position.x, 0, _projectile.transform.position.z - _target.transform.position.z);
        var pushVector = _target.transform.position - difference * _knockbackForce;
        _target.transform.DOMove(pushVector, 1);
        
        _target.TakeDamage(_damage, AttackType.Physical);
    }

    private Vector3 GetValidDestination(Vector3 startPosition, Vector3 direction, float maxDistance)
    {
        Vector3 destination = startPosition + direction * maxDistance;
        NavMeshHit hit;

        if (NavMesh.Raycast(startPosition, destination, out hit, NavMesh.AllAreas))
        {
            var distanceStep = maxDistance / 10f;
            for (var distance = maxDistance - distanceStep; distance > 0; distance -= distanceStep)
            {
                destination = startPosition + direction * distance;
                if (!NavMesh.Raycast(startPosition, destination, out hit, NavMesh.AllAreas))
                {
                    if (IsOnGroundLayer(destination))
                    {
                        return destination;
                    }
                }
            }
            
            return startPosition;
        }
        
        if (IsOnGroundLayer(destination))
        {
            return destination;
        }
        
        return Vector3.zero;
    }

    private bool IsOnGroundLayer(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 0.1f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }


    private void MoveCharacter(BaseCharacerView character, Vector3 destination)
    {
        if (character.NavMeshAgent != null)
        {
            character.NavMeshAgent.ResetPath();
        }

        character.transform.DOMove(destination, _speed / character.NavMeshAgent.speed).OnComplete(() =>
        {
            character.NavMeshAgent.enabled = false;
            character.NavMeshAgent.enabled = true;
        });
    }
}
