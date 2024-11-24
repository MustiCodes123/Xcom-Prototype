using System;
using Data.Resources.AddressableManagement;
using Zenject;
using UnityEngine;

public class CharacterFactory : IFactory<BaseCharacterModel,BaseCharacerView>
{
    private DiContainer _container;
    private ResourceManager _resourceManager;

    public CharacterFactory(DiContainer container, ResourceManager resourceManager)
    {
        _container = container;
        _resourceManager = resourceManager;
    }

    public BaseCharacerView Create(BaseCharacterModel param)
    {
        GameObject prefab = _resourceManager.LoadBaseCharacterPrefab(param.Name);
        var view = _container.InstantiatePrefabForComponent<BaseCharacerView>(prefab);
        view.SlotsHolder.SetupItems(param.EquipedItems, _resourceManager);
        view.OnSpawned(param, false);
        return view;
    }
}