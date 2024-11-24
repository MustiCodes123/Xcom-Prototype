public class FireArrowTalentBehaviour : BaseSkillBehaviour
{
    private UnityEngine.Vector3 _arrowOfset = new UnityEngine.Vector3(0, 1, -1);
    public FireArrowTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);
        _particle.SetParent(_projectile.transform);
        _particle.transform.localPosition = new UnityEngine.Vector3(_particle.transform.localPosition.x + _arrowOfset.x, _particle.transform.localPosition.y + _arrowOfset.y, _particle.transform.localPosition.z + _arrowOfset.z);

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();

        IBuff fireBuff = GetBuff(Skill.OnCollisionBuff);
        _target.SkillServiceProvider.AddBuff(fireBuff);

        int damage = Skill.Value;
        _target.TakeDamage(damage, AttackType.Magical);
    }
}