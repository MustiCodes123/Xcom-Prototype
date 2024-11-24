using System;
using UnityEngine;

public class CombatServiceProvider
{
    public AttackersHandler AttackersHandler;
    public Action<BaseCharacerView> AdditionalAttackAction;

    private BaseCharacerView _baseCharacterView;
    private BaseShootProjectile.Factory _projectileFactory;
    private BaseParticleView.Factory _particleFactory;
    private CharactersRegistry _charactersRegistry;

    public CombatServiceProvider(BaseCharacerView characerView, BaseShootProjectile.Factory factory, UniRxDisposable uniRxDisposable, BaseParticleView.Factory particleFactory, CharactersRegistry characterRegistry)
    {
        AttackersHandler = new AttackersHandler(uniRxDisposable);
        _baseCharacterView = characerView;
        _projectileFactory = factory;
        _particleFactory = particleFactory;
        _charactersRegistry = characterRegistry;
    }

    public void Shoot()
    {
        var weapon = _baseCharacterView.Weapon;
        var weaponView = _baseCharacterView.RangeWeaponView as RangeWeaponView;

        var projectile = _projectileFactory.Create(weaponView);
        projectile.Setup(_baseCharacterView, _baseCharacterView.characterData.GetDamage(weapon.DamageType), weapon.DamageType, weaponView.ProjectileParent);
    }

    public void MeleeAttack()
    {
        if (_baseCharacterView.Target == null) return;

        if (!_baseCharacterView.Target.CombatServiceProvider.AttackersHandler.Attackers.Contains(_baseCharacterView))
        {
            _baseCharacterView.Target.CombatServiceProvider.AttackersHandler.AddAttacker(_baseCharacterView);
        }

        AdditionalAttackAction?.Invoke(_baseCharacterView);

        var weapon = _baseCharacterView.Weapon;
        if (weapon != null)
            _baseCharacterView.Target.TakeDamage(_baseCharacterView.characterData.GetDamage(weapon.DamageType), weapon.DamageType, _baseCharacterView.characterData.GetAccuracy());
        else
            _baseCharacterView.Target.TakeDamage(_baseCharacterView.characterData.GetDamage(), AttackType.Physical, _baseCharacterView.characterData.GetAccuracy());
    }

    public void ShieldHammerAttack()
    {
        if (_baseCharacterView.Target == null || _baseCharacterView.Target.IsDead)
        {
            _baseCharacterView.Target = _charactersRegistry.GetClosestEnemy(Team.Allies, _baseCharacterView.transform.position) as BaseCharacerView;
        }

        if (!IsTargetInRange(_baseCharacterView.Target, _baseCharacterView) || _baseCharacterView.Target == null)
            return;

        if (!_baseCharacterView.Target.CombatServiceProvider.AttackersHandler.Attackers.Contains(_baseCharacterView))
        {
            _baseCharacterView.Target.CombatServiceProvider.AttackersHandler.AddAttacker(_baseCharacterView);
        }

        BaseParticleView particle = _particleFactory.Create(ParticleType.ArcaneSlashHit);
        particle.SetParent(_baseCharacterView.Target.transform);

        var shieldSkill = _baseCharacterView.SkillServiceProvider.GetSkillByID(TalentsEnum.ShieldHammer);

        if (shieldSkill != null)
        {
            _baseCharacterView.Target.TakeDamage(shieldSkill.Value, shieldSkill.DamageType, _baseCharacterView.characterData.GetAccuracy());
        }
        else
        {
            _baseCharacterView.Target.TakeDamage(_baseCharacterView.characterData.GetDamage(), AttackType.Physical, _baseCharacterView.characterData.GetAccuracy());
        }
    }

    public void CritMeleeAttack()
    {
        if (_baseCharacterView.Target == null) return;

        var weapon = _baseCharacterView.Weapon;
        if (weapon != null)
            _baseCharacterView.Target.TakeCriticalDamage(_baseCharacterView.characterData.GetDamage(weapon.DamageType), weapon.DamageType, _baseCharacterView.characterData.GetAccuracy());
        else
            _baseCharacterView.Target.TakeCriticalDamage(_baseCharacterView.characterData.GetDamage(), AttackType.Physical, _baseCharacterView.characterData.GetAccuracy());
    }

    private bool IsTargetInRange(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        float distance = Vector3.Distance(target.transform.position, selfCharacter.transform.position);
        return distance <= selfCharacter.characterData.GetAtackDistance();
    }

    public void SubscribeAdditionalAttack(Action<BaseCharacerView> action)
    {
        AdditionalAttackAction += action;
    }

    public void UnSubscribeAdditionalAttack(Action<BaseCharacerView> action)
    {
        AdditionalAttackAction -= action;
    }
}
