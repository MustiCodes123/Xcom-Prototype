using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsInfoViewer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] Button upgradeButton;

    private Action<StatsEnum> onUpgradeButtonClick;
    private Stat stat;

    private void Awake()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        }
    }

    private void OnUpgradeButtonClick()
    {
        onUpgradeButtonClick?.Invoke(stat.Param);
    }

    public void SetInfo(string value, string value1 = "")
    {
        descriptionText.text = value;
  
        valueText.gameObject.SetActive(!string.IsNullOrEmpty(value1));
        valueText.text = value1;
    }
    
    public void SetInfo(Stat stat,bool showUpdateButton , Action<StatsEnum> action)
    {
        upgradeButton.gameObject.SetActive(showUpdateButton);
        this.stat = stat;
        onUpgradeButtonClick = action;
        descriptionText.text =   stat.Param.ToString();
        valueText.text = stat.Value.ToString();
    }

    public void SetValueColor(Color color)
    {
        valueText.color = color;
    }

    private void OnDestroy()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
        }
    }   

}
