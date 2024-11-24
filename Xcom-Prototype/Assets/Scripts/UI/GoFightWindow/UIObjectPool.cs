using UnityEngine;
using Zenject;

public class UIObjectPool<T> : MemoryPool<Transform, T> where T : Component
{
    protected override void Reinitialize(Transform parent, T item)
    {
        item.transform.SetParent(parent);
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);
    }
}