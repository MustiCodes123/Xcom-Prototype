using System.Collections;
using System.Threading.Tasks;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class ItemViewFactory : IFactory<string, Task<ItemView>>
{
    private DiContainer _container;
    private ResourceManager _resourceManager;

    public ItemViewFactory(DiContainer container, ResourceManager resourceManager)
    {
        _container = container;
        _resourceManager = resourceManager;
    }

    public async Task <ItemView> Create(string param)
    {
        GameObject prefab = await _resourceManager.LoadItemPrefabAsync(param);
        var view = _container.InstantiatePrefabForComponent<ItemView>(prefab);
        view.OnSpawned();
        return view;
    }
}