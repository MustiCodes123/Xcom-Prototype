using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class ChestCardPrefab : MonoBehaviour
{
    [SerializeField] private Image _image;

    private ItemTemplate _droppedItem;

    private Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Show(ItemTemplate itemData)
    {
        _droppedItem = itemData;

        _image.sprite = itemData.itemSprite;

        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        if(_droppedItem == null)
        {
            Debug.LogError($"Item is null");
            return;
        }

        ItemInfoPopup.Instance.Show(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(_droppedItem));
        ItemInfoPopup.Instance.ActivateButtons("", null, () => ItemInfoPopup.Instance.gameObject.SetActive(false), true);
    }
}