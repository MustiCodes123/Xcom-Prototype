using DG.Tweening;
using UnityEngine;

public class ThrowShieldTalent : BaseSkillBehaviour
{
    private  float _pushForce;
    private  float _attractionSpeed;
    private  float _animDelay;
    private Transform _shieldItem;
    private GameObject _currentItem;

    public ThrowShieldTalent(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
        _attractionSpeed = 1f;
        _pushForce = 4f;
        AnimationDelay = 0f;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        var shieldSlot = selfCharacter.SlotsHolder.GetSlot(SlotEnum.Shield);
      
        _currentItem = shieldSlot.GetCurrentItem();
        _shieldItem = _currentItem.transform;
        
        
        SetupProjectile(target, selfCharacter, _currentItem.transform);
        

        return base.Use(target, selfCharacter);
    }

    private void SetupProjectile(BaseCharacerView target, BaseCharacerView selfCharacter, Transform parent)
    {
        _target = target;
        _selfCharacter = selfCharacter;
        
        var projectile = _projectileFactory.Create(Skill.ProjectileType);
        _projectile = projectile;
        
        GetShieldPosition();

        var particle = _particleFactory.Create(Skill.ParticleType);
        _particle = particle;
        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);
        
        var particle2 = _particleFactory.Create(Skill.ParticleType);
        var projectileTransform = selfCharacter.transform;
        projectileTransform.position = projectileTransform.position;

        _particle.SetParent(parent);
        _particle.transform.localPosition = new Vector3(-0.5f, 0.55f, 0f);
        _particle.transform.localRotation = Quaternion.Euler(Vector3.zero); 
        
        particle2.SetParent(parent);
        particle2.transform.localPosition = new Vector3(0.5f, 0.55f, 0f);
        particle2.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void GetShieldPosition()
    {
        _shieldItem.SetParent(_projectile.transform);
        _shieldItem.localPosition = new Vector3(0f, 1f, 1f);
    }

    public void OnHit()
    {
        var target = _projectile.GetTarget();

        int damage = Skill.Value;
        target.TakeDamage(damage, AttackType.Physical);

        Vector3 difference = new Vector3(_projectile.transform.position.x - target.transform.position.x, 0, _projectile.transform.position.z - target.transform.position.z);
        Vector3 pushVector = target.transform.position - difference * _pushForce;

        target.transform.DOMove(pushVector, _attractionSpeed);
    }
}
