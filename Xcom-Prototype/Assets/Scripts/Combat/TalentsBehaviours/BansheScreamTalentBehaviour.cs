public class BansheScreamTalentBehaviour : BaseSkillBehaviour
{
    public BansheScreamTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);
        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
        _particle.SetParent(_projectile.transform);

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();
        _target.TakeDamage(Skill.Value, AttackType.Magical);

        if (_target != null)
        {
            var deBuff = GetBuff(Skill.OnCollisionBuff);
            _target.SkillServiceProvider.AddBuff(deBuff);
        }
    }
}