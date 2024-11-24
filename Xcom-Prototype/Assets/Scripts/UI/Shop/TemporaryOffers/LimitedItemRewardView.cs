using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LimitedItemRewardView : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemInfo;
    [SerializeField] private Image _icon;

    public void Initialize(string info)
    {
        _itemInfo.text = info;
    }
}
