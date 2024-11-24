using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using Zenject;

public class ProjectileFactory : IFactory<ProjectileType, BaseProjectile>
{
    DiContainer _container;
    private ResourceManager _resourceManager;
    private Dictionary<ProjectileType, List<BaseProjectile>> pool;

    public ProjectileFactory(DiContainer container, ResourceManager resourceManager)
    {
        _container = container;
        _resourceManager = resourceManager;
        pool = new Dictionary<ProjectileType, List<BaseProjectile>>();
    }

    public BaseProjectile Create(ProjectileType projectileType)
    {
        if (!pool.ContainsKey(projectileType))
        {
            pool.Add(projectileType, new List<BaseProjectile>());
        }

        if (pool[projectileType].Count > 0)
        {
            for (int i = 0; i < pool[projectileType].Count; i++)
            {
                var oldprojectile = pool[projectileType][i];
                if (!oldprojectile.isActiveAndEnabled)
                {
                    oldprojectile.OnSpawned();
                    return oldprojectile;
                }
            }
        }

        var prefab = _resourceManager.LoadProjectilePrefab(projectileType.ToString());
        var projectile = _container.InstantiatePrefabForComponent<BaseProjectile>(prefab);
        projectile.projectileType = projectileType;
        projectile.OnSpawned();
        pool[projectileType].Add(projectile);
        return projectile;
    }
}