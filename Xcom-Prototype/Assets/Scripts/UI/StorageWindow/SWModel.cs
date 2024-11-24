using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SWModel : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    public List<IStorageItemData> ItemsData { get; private set; }

    public void Initialize()
    {
        ItemsData = new List<IStorageItemData>();

        List<BaseCharacterModel> storageCharacters = _playerData.PlayerGroup.GetStorageCharacters();
        List<BaseItem> storageItems = _playerData.PlayerInventoryStorage;

        foreach (BaseCharacterModel character in storageCharacters)
        {
            ItemsData.Add(character);
        }

        foreach(BaseItem item in storageItems)
        {
            ItemsData.Add(item);
        }
    }
}