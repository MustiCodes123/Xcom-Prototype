using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopPrefabRepository : MonoBehaviour
{
    [System.Serializable]
    public struct WindowPrefabPair
    {
        public WindowType WindowType;
        public GameObject Prefab;
    }

    [SerializeField] private List<WindowPrefabPair> _windowPrefabs;

    private Dictionary<WindowType, GameObject> _prefabDictionary;

    public void Initialize()
    {
        _prefabDictionary = new Dictionary<WindowType, GameObject>();

        foreach (var pair in _windowPrefabs)
        {
            _prefabDictionary[pair.WindowType] = pair.Prefab;
        }
    }

    public GameObject GetPrefabForWindowType(WindowType windowType)
    {
        if (_prefabDictionary.TryGetValue(windowType, out var prefab))
        {
            return prefab;
        }

        Debug.LogError($"No prefab found for window type: {windowType}");

        return null;
    }
}
