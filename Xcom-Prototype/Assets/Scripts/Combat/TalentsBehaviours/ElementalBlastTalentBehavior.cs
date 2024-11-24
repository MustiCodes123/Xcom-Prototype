public class ElementalBlastTalentBehavior : BaseSkillBehaviour
{
    public ElementalBlastTalentBehavior(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);
        _particle.SetParent(selfCharacter.transform);
        selfCharacter.SkillServiceProvider.RemoveAllBuffs();
        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {  
        _target = _projectile.GetTarget();

        if (_target != null)
        {
            int damage = Skill.Value;
            _target.TakeDamage(damage, AttackType.Magical);
        }
    }
}