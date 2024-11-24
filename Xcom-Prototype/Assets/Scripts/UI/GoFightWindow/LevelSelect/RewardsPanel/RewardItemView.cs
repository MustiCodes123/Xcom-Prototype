using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class RewardItemView : MonoBehaviour
{
    public Action<string, string, Transform> Click;

    [SerializeField] private Image _icon;

    [SerializeField] private TMP_Text _amountText;

    private RewardItem _rewardData;
    private Button _button;

    private void OnDisable()
    {
        if(_button != null)
            _button.onClick.RemoveListener(() => Click?.Invoke(_rewardData.Title, _rewardData.Description, transform));
    }

    public void DisplayReward(RewardItem data)
    {
        _rewardData = data;

        _button = GetComponent<Button>();

        _button.onClick.AddListener(() => Click?.Invoke(_rewardData.Title, _rewardData.Description, transform));

        _icon.sprite = data.Icon;

        _amountText.text = "";
        if (_rewardData?.Amount > 0)
            _amountText.text = _rewardData.Amount.ToString();            
    }
}
