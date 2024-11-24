using UnityEngine;

public class DanceWithDeathTalentBehaviour : BaseSkillBehaviour
{
    public DanceWithDeathTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        _projectile.transform.SetParent(selfCharacter.RangeWeaponView.transform);
        _particle.transform.SetParent(selfCharacter.RangeWeaponView.transform);

        _projectile.transform.localPosition = Vector3.zero;
        _particle.transform.localPosition = new Vector3(0f, 0f, -1f);

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();

        if (_target != null)
        {
            IBuff deBuff = GetBuff(Skill.OnCollisionBuff);
            _target.SkillServiceProvider.AddBuff(deBuff);
        }
    }
}
