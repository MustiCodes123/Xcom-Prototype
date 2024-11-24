using System;
using Data.Resources.AddressableManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SWItem : MonoBehaviour
{
    public Action<SWItem> TakeClick;

    [SerializeField] private Image _itemIcon;
    [SerializeField] private Button _takeButton;

    public string ID { get; set; }
    public IStorageItemData Data { get; set; }

    public async void Initialize(IStorageItemData data, string ID, ResourceManager resourceManager)
    {
        _takeButton.onClick.AddListener(OnTakeButtonClick);

        Data = data;
        this.ID = ID;
        

        if (Data is BaseCharacterModel characterModel)
        {
            _itemIcon.sprite = resourceManager.LoadSprite(characterModel.AvatarId);
        }
        else
        {
            _itemIcon.sprite = await data.GetItemIcon();
        }
    }

    private void OnTakeButtonClick()
    {
        TakeClick?.Invoke(this);
    }
}
