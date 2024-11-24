using UnityEngine;

public class MagneticWavesTalentBehaviour : BaseSkillBehaviour
{
    private float _range;
    private IBuff _deBuff;
    
    public MagneticWavesTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateParticleAndProjectile(target, selfCharacter);

        var transform = selfCharacter.transform;
        _projectile.transform.position = transform.position;
        _particle.transform.position = transform.position;
        
        _projectile.SetParent(transform);
        _particle.SetParent(_projectile.transform);

        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale, Skill.OnCollisionBuff);
        _range = CalculateRange();

        return base.Use(target, selfCharacter);
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();

        if (_target == null) return;
        var distance = Vector3.Distance(_target.transform.position, _selfCharacter.transform.position);
        
        if (!(distance > _range)) return;
        RemoveDebuff();
    }

    private float CalculateRange()
    {
        var collider = _projectile.GetComponent<Collider>();
        return collider.bounds.size.magnitude;
    }

    private void OnHit()
    {
        _target = _projectile.GetTarget();

        if (_target != null)
        {
            _deBuff = GetBuff(Skill.OnCollisionBuff);
            _target.SkillServiceProvider.AddBuff(_deBuff);
        }
    }

    private void RemoveDebuff()
    {
        if (_deBuff == null) return;
        _target.SkillServiceProvider.RemoveBuff(_deBuff);
    }
}
