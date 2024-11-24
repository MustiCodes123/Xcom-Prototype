using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PortalCristalHint : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    [Inject] private PlayerData playerData;
    [Inject] private ItemsDataInfo itemsDataInfo;

    private void OnEnable()
    {

        Refresh();
    }

    public void Refresh()
    {
        if (playerData.CommonSummonCristal > 0)
        {
            text.text = playerData.CommonSummonCristal.ToString();
            image.sprite = itemsDataInfo.CristalsInfo[0].Sprite;
        }
        else if (playerData.RareSummonCristal > 0)
        {
            text.text = playerData.RareSummonCristal.ToString();
            image.sprite = itemsDataInfo.CristalsInfo[1].Sprite;
        }
        else if (playerData.EpicSummonCristal > 0)
        {
            text.text = playerData.EpicSummonCristal.ToString();
            image.sprite = itemsDataInfo.CristalsInfo[2].Sprite;
        }
        else if (playerData.LegendarySummonCristal > 0)
        {
            text.text = playerData.LegendarySummonCristal.ToString();
            image.sprite = itemsDataInfo.CristalsInfo[3].Sprite;
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }   

}
