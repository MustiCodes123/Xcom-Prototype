public class ElementalStakesTalentBehaviour : BaseSkillBehaviour
{
    public ElementalStakesTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _target = target;
        _selfCharacter = selfCharacter;

        BaseParticleView particle = _particleFactory.Create(Skill.ParticleType);
        BaseProjectile projectile = _projectileFactory.Create(Skill.ProjectileType);
        projectile.Setup(target, selfCharacter, particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
        _projectile = projectile;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();

        if (_target != null)
        {
            _target.TakeDamage(Skill.Value, Skill.DamageType);
            IBuff deBuff = GetBuff(Skill.OnCollisionBuff);
            _target.SkillServiceProvider.AddBuff(deBuff);
        }
    }
}
