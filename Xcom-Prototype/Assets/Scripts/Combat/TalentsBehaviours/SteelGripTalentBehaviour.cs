using DG.Tweening;
using UnityEngine;

public class SteelGripTalentBehaviour : BaseSkillBehaviour
{
    private const float MinDistanceToTarget = 2.0f;
    private const float MoveDuration = 0.5f;
    private const float ParticleSpeed = 0.1f;
    private const float DirectionMultiplier = 2.0f;

    private bool _isHit;
    private ItemSlot _shieldSlot;
    private ParticleSystem _particleSystem;
    private Vector3 _backPosition;

    public SteelGripTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
        AnimationDelay = 0f;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _selfCharacter = selfCharacter;

        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        _shieldSlot = selfCharacter.SlotsHolder.GetSlot(SlotEnum.Weapon);

        SetParticlePositionAndRotation();

        _particleSystem = _particle.ParticleSystem;

        return base.Use(target, selfCharacter);
    }

    private void SetParticlePositionAndRotation()
    {
        _particle.transform.position = _shieldSlot.GetItemSlotTransform().position;
        _backPosition = _particle.transform.position;
        _particle.transform.rotation = _selfCharacter.transform.rotation;
    }

    private void OnHit()
    {
        if (_isHit) return;

        _isHit = true;
        _target = _projectile.GetTarget();

        if (ShouldMoveTarget())
        {
            MoveTarget();
            ApplyDebuff();
        }
    }

    private bool ShouldMoveTarget()
    {
        var distance = Vector3.Distance(_selfCharacter.transform.position, _target.transform.position);
        return distance > MinDistanceToTarget && _target != null;
    }

    private void MoveTarget()
    {
        var directionToTarget = (_target.transform.position - _selfCharacter.transform.position).normalized;
        var newPosition = _selfCharacter.transform.position + directionToTarget * DirectionMultiplier;

        var backParticle = _particleFactory.Create(Skill.ParticleType);
    
        var forwardOffset = _particle.transform.forward * 6f;
        var backParticleStartPosition = _particle.transform.position + forwardOffset;
        backParticle.transform.position = backParticleStartPosition;

        var backParticleDirection = -directionToTarget;

        backParticle.transform.rotation = Quaternion.LookRotation(backParticleDirection);

        AdjustParticleSpeed(ParticleSpeed);

        _target.transform.DOMove(newPosition, MoveDuration).OnComplete(() =>
        {
            AdjustParticleSpeed(-ParticleSpeed);
            _isHit = false;
            _backPosition = Vector3.zero;
        });
    }

    private void AdjustParticleSpeed(float speed)
    {
        var mainModule = _particleSystem.main;
        mainModule.startSpeed = speed;
    }

    private void ApplyDebuff()
    {
        var debuff = new StunBuff(_particleFactory, Skill);
        _target.SkillServiceProvider.AddBuff(debuff);
    }
}
