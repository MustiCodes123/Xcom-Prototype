using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class AttractionSphereTaletnBehaviour : BaseSkillBehaviour
{
    private BaseProjectile.Factory projectileFactory;
    private List<BaseCharacerView> _targets;
    private int _attractionSpeed;

    public AttractionSphereTaletnBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        this.projectileFactory = projectileFactory;
        _decaleFactory = decaleFactory;
        _attractionSpeed = skill.Duration;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _targets = new List<BaseCharacerView>();

        var particle = _particleFactory.Create(Skill.ParticleType);
        var projectile = projectileFactory.Create(Skill.ProjectileType);

        particle.SetParent(projectile.transform);
        projectile.Setup(target, selfCharacter, particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
        _projectile = projectile;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();
        _targets.Add(_target);

        if (_target != null)
        {
            var deBuff = GetBuff(Skill.OnCollisionBuff);
            _target.SkillServiceProvider.AddBuff(deBuff);

            _target.transform.DOMove(_projectile.transform.position, _attractionSpeed).OnComplete(() =>
            {
                foreach (var target in _targets)
                {
                    target.TakeDamage(Skill.Value, AttackType.Magical);
                }
            });
        }
    }
}