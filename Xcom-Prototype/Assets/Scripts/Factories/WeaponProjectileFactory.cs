using System.Collections.Generic;
using Zenject;

public class WeaponProjectileFactory : IFactory<RangeWeaponView, BaseShootProjectile>
{
    DiContainer _container;
    private Dictionary<RangeWeaponView, List<BaseShootProjectile>> _rangeWeaponPool = new Dictionary<RangeWeaponView, List<BaseShootProjectile>>();

    public WeaponProjectileFactory(DiContainer container)
    {
        _container = container;
    }

    public BaseShootProjectile Create(RangeWeaponView rangeWeapon)
    {
        if (!_rangeWeaponPool.ContainsKey(rangeWeapon))
            _rangeWeaponPool.Add(rangeWeapon, new List<BaseShootProjectile>());

        if (_rangeWeaponPool[rangeWeapon].Count > 0)
        {
            foreach (var oldProjectile in _rangeWeaponPool[rangeWeapon])
            {
                if (!oldProjectile.IsActive)
                {
                    oldProjectile.OnSpawned();
                    return oldProjectile;
                }
            }
        }
        var projectile = _container.InstantiatePrefabForComponent<BaseShootProjectile>(rangeWeapon.BaseShootProjectilePrefab);
        projectile.OnSpawned();
        _rangeWeaponPool[rangeWeapon].Add(projectile);
        return projectile;
    }
}
