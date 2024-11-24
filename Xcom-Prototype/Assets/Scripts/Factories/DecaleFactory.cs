using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using Zenject;

public class DecaleFactory : IFactory<DecaleType, BaseDecale>
{
    DiContainer _container;
    private ResourceManager _resourceManager;
    private Dictionary<DecaleType, List<BaseDecale>> pool;

    public DecaleFactory(DiContainer container, ResourceManager resourceManager)
    {
        _container = container;
        _resourceManager = resourceManager;
        pool = new Dictionary<DecaleType, List<BaseDecale>>();
    }

    public BaseDecale Create(DecaleType decaleType)
    {
        if (!pool.ContainsKey(decaleType))
        {
            pool.Add(decaleType, new List<BaseDecale>());
        }


        if (pool[decaleType].Count > 0)
        {
            for (int i = 0; i < pool[decaleType].Count; i++)
            {
                var oldDecale = pool[decaleType][i];
                if (!oldDecale.isActiveAndEnabled)
                {
                    oldDecale.OnSpawned();
                    return oldDecale;
                }
            }
        }
        var prefab = _resourceManager.LoadDecalePrefab(decaleType.ToString());
        var decale = _container.InstantiatePrefabForComponent<BaseDecale>(prefab);
        decale.DecaleType = decaleType;
        decale.OnSpawned();
        pool[decaleType].Add(decale);
        return decale;
    }
}