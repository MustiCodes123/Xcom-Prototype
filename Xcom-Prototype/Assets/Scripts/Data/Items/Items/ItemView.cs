using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ItemView : MonoBehaviour
{
    public virtual void OnSpawned()
    {

    }

    public class Factory : PlaceholderFactory<string, Task<ItemView>>
    {


    }
}