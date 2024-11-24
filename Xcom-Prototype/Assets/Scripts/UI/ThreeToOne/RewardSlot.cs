using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _rewardText;

    public void InitializeItem(ItemTemplate item)
    {
        _icon.sprite = item.itemSprite;
        _rewardText.text = item.itemName;
    }

    public void InitializeResource(Resource resource)
    {
        var data = ItemsDataInfo.Instance.GetResourceData(resource);
        _icon.sprite = data.Sprite;
        _rewardText.text = data.Name;
    }

    public void InitializeResource(ResourceType type, int count)
    {
        var data = ItemsDataInfo.Instance.GetResourceData(type);
        _icon.sprite = data.Sprite;
        _rewardText.text = count.ToString();
    }
}
