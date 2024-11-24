using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SonicBoomTaletBehaviour : BaseSkillBehaviour
{
    private int _attractionSpeed;
    private float _pushForse;
    public SonicBoomTaletBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
        _attractionSpeed = 1;
        _pushForse = 2f;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        if (_projectile is WaveThroughWithDecale wave)
        {
            wave.Trail = _particle;
        }

        _particle.transform.position = selfCharacter.transform.position;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();
        int damage = Skill.Value;
        _target.TakeDamage(damage, AttackType.Magical);

        Vector3 differense = new Vector3(_projectile.transform.position.x - _target.transform.position.x, 0, _projectile.transform.position.z - _target.transform.position.z);
        Vector3 pushVector = _target.transform.position - differense * _pushForse;
        _target.transform.DOMove(pushVector, _attractionSpeed);
    }

    public BaseParticleView IceTrailBehaveour(BaseCharacerView target)
    {
        return null;
    }
}