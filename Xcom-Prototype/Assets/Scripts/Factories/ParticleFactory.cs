using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class ParticleFactory : IFactory<ParticleType, BaseParticleView>
{
    DiContainer _container;
    private ResourceManager _resourceManager;
    private Dictionary<ParticleType, List<BaseParticleView>> pool;

    public ParticleFactory(DiContainer container, ResourceManager resourceManager)
    {
        _container = container;
        _resourceManager = resourceManager;
        pool = new Dictionary<ParticleType, List<BaseParticleView>>();
    }

    public void BackToPool(BaseParticleView particle)
    {
        if (!pool.ContainsKey(particle.particleType))
        {
            pool.Add(particle.particleType, new List<BaseParticleView>());
        }
        pool[particle.particleType].Add(particle);
    }

    public BaseParticleView Create(ParticleType particleType)
    {
        if (!pool.ContainsKey(particleType))
        {
            pool.Add(particleType, new List<BaseParticleView>());
        }


        if (pool[particleType].Count > 0)
        {
            for (int i = 0; i < pool[particleType].Count; i++)
            {
                BaseParticleView oldparticle = pool[particleType][i];

                if (!oldparticle.isActiveAndEnabled)
                {
                    oldparticle.OnSpawned();
                    return oldparticle;
                }
            }
        }

        var prefab = _resourceManager.LoadParticlePrefab(particleType.ToString());
        var particle = _container.InstantiatePrefabForComponent<BaseParticleView>(prefab);
        particle.particleType = particleType;
        particle.OnSpawned();
        pool[particleType].Add(particle);
        return particle;
    }
}