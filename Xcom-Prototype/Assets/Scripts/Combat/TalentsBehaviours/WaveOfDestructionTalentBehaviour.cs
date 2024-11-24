public class WaveOfDestructionTalentBehaviour : BaseSkillBehaviour
{
    public WaveOfDestructionTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        if (_projectile is DividedProjectile wave)
        {
            wave.Trail = _particle;
        }

        _particle.transform.position = selfCharacter.transform.position;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();

        IBuff stun = GetBuff(Skill.OnCollisionBuff);
        _target.SkillServiceProvider.AddBuff(stun);

        int damage = Skill.Value;
        _target.TakeDamage(damage, Skill.BuffDamageType);
    }
}
