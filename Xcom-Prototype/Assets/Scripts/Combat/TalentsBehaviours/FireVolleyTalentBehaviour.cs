using DG.Tweening;
using UnityEngine;

public class FireVolleyTalentBehaviour : BaseSkillBehaviour
{
    private readonly int _missileCountToSpawn = 5;

    private BaseProjectile[] _projectiles;
    private float _startAngle;
    private float _endAngle;
    private float _radius;
    private Vector3 startPoint;

    public FireVolleyTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _projectiles = new BaseProjectile[_missileCountToSpawn];

        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;

        AnimationDelay = 0.5f;
        _startAngle = 135f;
        _endAngle = 225f;
        _radius = 5f;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {

        if (target == null || target.IsDead)
        {
            target = _charactersRegistry.GetClosestEnemy(Team.Allies, selfCharacter.transform.position) as BaseCharacerView;
        }

        if (!IsTargetInRange(target, selfCharacter) || target == null)
            return false;

        var moveUpPosition = new Vector3(0f, 1.2f, 0f);
        startPoint = selfCharacter.transform.position + moveUpPosition;
        
        SpawnMissiles(target, selfCharacter, startPoint);

        return base.Use(target, selfCharacter);
    }

    private void SpawnMissiles(BaseCharacerView target, BaseCharacerView selfCharacter, Vector3 startPoint)
    {
        var minAngle = _startAngle;
        var maxAngle = _endAngle;
        var totalAngleRange = maxAngle - minAngle;
    
        var backwardDirection = target.transform.forward;

        for (var i = 0; i < _missileCountToSpawn; i++)
        {
            InitializeProjectile(target, selfCharacter, i);

            var currentAngle = minAngle + (totalAngleRange / (_missileCountToSpawn - 1)) * i;
            var rotation = Quaternion.Euler(0, currentAngle, 0);
            var rotatedDirection = rotation * backwardDirection;

            
            float heightOffset = 5f; 
            
            var missileStartPosition = target.transform.position + rotatedDirection * _radius + Vector3.up * heightOffset;

            SetupProjectile(i, target, selfCharacter, missileStartPosition);
            AnimateMissileMovement(i, missileStartPosition, target);
        }
    }


    private void InitializeProjectile(BaseCharacerView target, BaseCharacerView selfCharacter, int index)
    {
        _target = target;
        _selfCharacter = selfCharacter;
        _projectiles[index] = _projectileFactory.Create(Skill.ProjectileType);
    }

    private void SetupProjectile(int index, BaseCharacerView target, BaseCharacerView selfCharacter, Vector3 missileStartPosition)
    {
        var projectile = _projectiles[index];

        if (projectile != null)
        {
            projectile.transform.position = startPoint;
            projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);
            projectile.transform.forward = (target.transform.position - missileStartPosition).normalized; // Point towards the target
            projectile.transform.position = new Vector3(projectile.transform.position.x, startPoint.y, projectile.transform.position.z);
        }
        else
        {
            Debug.LogError($"Projectile at index {index} is null.");
        }
    }

    private void AnimateMissileMovement(int index, Vector3 startPosition, BaseCharacerView target)
    {
        var projectile = _projectiles[index];
        var delay = index * 0.2f;
        var targetPos = target.transform.position + new Vector3(0f, 1f, 0f);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(projectile.transform.DOMove(startPosition, 0.9f).SetDelay(delay));
        sequence.Append(projectile.transform.DOMove(targetPos, 1f).SetEase(Ease.Linear));
    }

    private void OnHit()
    {
        foreach (var projectile in _projectiles)
        {
            if (projectile != null)
            {
                var target = projectile.GetTarget();
                if (target != null && !target.SkillServiceProvider.IsBuffOnMe(Skill.OnCollisionBuff))
                {
                    BangBehaveour(projectile.transform.position, Skill.OnCollisionParticle);
                    target.TakeDamage(Skill.Value, AttackType.Magical);
                    var burn = GetBuff(Skill.OnCollisionBuff);
                    target.SkillServiceProvider.AddBuff(burn);
                }
            }
        }
    }

    public override BaseParticleView BangBehaveour(Vector3 target, ParticleType particleType)
    {
        var bang = _particleFactory.Create(particleType);
        foreach (var projectile in _projectiles)
        {
            bang.duration = Skill.Duration;
            bang.transform.position = target;
            bang.transform.position = projectile.transform.position;
        }
        return bang;
    }
}
