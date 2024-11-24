using System.Collections.Generic;

public class IceArmorBuff : BaseBuff
{
    private List<BaseCharacerView> _attackers = new List<BaseCharacerView>();
    public IceArmorBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _skill = skill;
        _particleFactory = particleFactory;
        _icon = skill.BuffIcon;
        _damage = skill.Value;
        _duration = skill.Duration;
        _damageType = skill.BuffDamageType;
        _particleType = skill.ParticleType;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _particle = _particleFactory.Create(_particleType);

        if (_particle is MeshParticleView meshParticle)
        {
            meshParticle.SetMesh(Owner.gameObject);
        }

        foreach (var attacker in Owner.CombatServiceProvider.AttackersHandler.Attackers)
        {
            _attackers.Add(attacker);
        }

        foreach (var attacker in _attackers)
        {
            attacker.CombatServiceProvider.SubscribeAdditionalAttack(Damage);
        }

        _owner.HealthBar.SetBuffOnBar(_icon);
    }

    protected override void OnTick()
    {
        var newAttackers = Owner.CombatServiceProvider.AttackersHandler.Attackers;

        if (newAttackers.Count != _attackers.Count)
        {
            foreach (var attacker in _attackers)
            {
                attacker.CombatServiceProvider.UnSubscribeAdditionalAttack(Damage);
            }

            foreach (var attacker in newAttackers)
            {
                _attackers.Add(attacker);
            }

            foreach (var attacker in _attackers)
            {
                attacker.CombatServiceProvider.SubscribeAdditionalAttack(Damage);
            }
        }

        base.OnTick();
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        base.OnRemove(target);

        if (_particle is MeshParticleView meshParticle)
        {
            meshParticle.SetNeitralMesh();
        }

        foreach (var attacker in _attackers)
        {
            attacker.CombatServiceProvider.UnSubscribeAdditionalAttack(Damage);
        }
    }

    private void Damage(BaseCharacerView attacker)
    {

        ColdDebuff burn = new ColdDebuff(_particleFactory, _skill);
        if (!attacker.SkillServiceProvider.IsBuffOnMe(burn.buffType))
        {
            attacker.SkillServiceProvider.AddBuff(burn);
        }

        attacker.TakeDamage(_damage, _damageType);
    }
}