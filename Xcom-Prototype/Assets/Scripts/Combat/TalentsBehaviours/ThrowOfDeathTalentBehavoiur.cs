using DG.Tweening;
using UnityEngine;

public class ThrowOfDeathTalentBehavoiur : BaseSkillBehaviour
{
    private float _animDelay;
    private Transform _weaponItem;
    private GameObject _currentItem;

    public ThrowOfDeathTalentBehavoiur(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
        _animDelay = 0.2f;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        selfCharacter.SkillServiceProvider.SetAnimDelay(_animDelay);

        var weaponSlot = selfCharacter.SlotsHolder.GetSlot(SlotEnum.Weapon);

        _currentItem = weaponSlot.GetCurrentItem();
        _weaponItem = _currentItem.transform;


        SetupProjectile(target, selfCharacter, _currentItem.transform);


        return base.Use(target, selfCharacter);
    }

    private void SetupProjectile(BaseCharacerView target, BaseCharacerView selfCharacter, Transform parent)
    {
        _target = target;
        _selfCharacter = selfCharacter;

        var projectile = _projectileFactory.Create(Skill.ProjectileType);
        _projectile = projectile;

        GetWeaponPosition();

        var particle = _particleFactory.Create(Skill.ParticleType);
        _particle = particle;
        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        var particle2 = _particleFactory.Create(Skill.ParticleType);
        var projectileTransform = selfCharacter.transform;

        _particle.SetParent(parent);
        _particle.transform.localPosition = new Vector3(-0.5f, 0.55f, 0f);
        _particle.transform.localRotation = Quaternion.Euler(Vector3.zero);

        particle2.SetParent(parent);
        particle2.transform.localPosition = new Vector3(0.5f, 0.55f, 0f);
        particle2.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void GetWeaponPosition()
    {
        _weaponItem.SetParent(_projectile.transform);
        _weaponItem.localPosition = new Vector3(0f, 1f, 1f);

        _weaponItem.localScale = Vector3.zero;

        _weaponItem.DOScale(Vector3.one, 0.1f).OnComplete(() => {
            _currentItem.SetActive(true);
        });
    }

    public void OnHit()
    {
        var target = _projectile.GetTarget();

        int damage = Skill.Value;
        target.TakeDamage(damage, AttackType.Physical);
    }

}
