using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampLevelCard : MonoBehaviour
{
    [SerializeField] private Image mainImage;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Button button;

    [SerializeField] private ItemShopCard itemShopCard;
    [SerializeField] private Transform itemShopCardParent;
    [SerializeField] private GameObject[] starsGO;

    private List<ItemShopCard> _itemShopCards = new List<ItemShopCard>();

    private CampLevel _level;

    public void SetLevel(CampLevel level, Action<CampLevel> action, int stars = 0, bool showPlayButton = false)
    {
        if (showPlayButton || stars > 0)
        {
            button.gameObject.SetActive(true);
        }
        else
        {
            button.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        _level = level;
        button.onClick.AddListener(() => { action?.Invoke(_level); });
        levelName.text = level.Name;
        energyText.text = level.EnergyCost.ToString();
        for (int i = 0; i < starsGO.Length; i++)
        {
            starsGO[i].SetActive(stars > i);
        }
    }
}
