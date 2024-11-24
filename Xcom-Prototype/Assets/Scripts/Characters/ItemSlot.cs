using System.Collections;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class ItemSlot : MonoBehaviour
{
    public SlotEnum SlotType;

    [SerializeField] private Transform _itemSlot;
    [SerializeField] private GameObject _currentItem;
    [SerializeField] private Vector3 scale = new Vector3(1, 1, 1);

    private ResourceManager _resourceManager;

    public void Setup (ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }
    public Transform ItemPlaceholder => _itemSlot;
    

    public ItemView ItemView { get; private set; }

    public async void EquipItem(string ItemName)
    {
        var prefab =  await _resourceManager.LoadItemPrefabAsync(ItemName);


        BaseCharacterModel equippedBaseCharacterModel = SmalCharacterCard.equippedBaseCharacterModel;
        PositionAndRotationData offsetDataOfThisItem = new();
        if (equippedBaseCharacterModel != null)
        {
            string equippedBaseCharacterModelName = equippedBaseCharacterModel.Name;
            foreach (CharacterPresetData characterPresetData in _resourceManager.CharacterPresetsRegister)
            {
                if(equippedBaseCharacterModelName == characterPresetData.Name)
                {
                    offsetDataOfThisItem = characterPresetData.CharacterPreset.ItemsOffsetData.Find(itemOffsetData => ItemName.Equals(itemOffsetData.ItemAddressableName)).Offset;
                    break;
                }
            }
        }

        var instantiated = Instantiate(prefab, _itemSlot);
        instantiated.transform.localScale = scale;
        instantiated.gameObject.SetActive(true);
        instantiated.transform.localPosition = Vector3.zero;

        instantiated.transform.localPosition += offsetDataOfThisItem.Position;
        instantiated.transform.localEulerAngles += offsetDataOfThisItem.EulerAngles;

        if (instantiated.TryGetComponent<ItemView>(out var view)) ItemView = view;
        if (_currentItem) Destroy(_currentItem.gameObject);

        _currentItem = instantiated;
    }

    public void HideItemSlot()
    {
        if (_currentItem)
            Destroy(_currentItem.gameObject);
    }

    public Transform GetItemSlotTransform()
    {
        return _itemSlot;
    }

    public GameObject GetCurrentItem()
    {
        return _currentItem;
    }
}
