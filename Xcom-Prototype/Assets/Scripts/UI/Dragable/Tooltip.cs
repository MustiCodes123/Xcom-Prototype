using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [SerializeField] private Button CloseButton;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Image itemImage;
    private RectTransform panelRectTransform;
    private RectTransform parentRectTransform;

    [Inject]
    public void Init()
    {
        Instance = this;
    }

    private void Awake()
    {
        CloseButton.onClick.AddListener(Close);
        panelRectTransform = transform.parent as RectTransform;
        parentRectTransform = panelRectTransform.parent as RectTransform;
    }

    void ClampToWindow()
    {
        Vector3 pos = panelRectTransform.localPosition;

        Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
        Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

        pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

        panelRectTransform.localPosition = pos;
    }

    private void Close()
    {
       gameObject.SetActive(false);
    }

    public void ShowTooltip(BaseItem item, Vector2 pos)
    {
        return;
        gameObject.SetActive(true);
        pos.x -= 200;

        gameObject.transform.position = pos;

        itemImage.sprite = item.itemSprite;

        itemNameText.text = item.itemName;  
        tooltipText.text = item.itemDescription;

        ClampToWindow();

        WeaponItem weaponItem = item as WeaponItem;
        if(weaponItem != null)
        {
            statsText.text =string.Format("Damage {0}-{1}\n Critical Chance {2}", weaponItem.minDamage, weaponItem.maxDamage, weaponItem.CriticalChance);
            return;
        }

        ArmorItem armorItem = item as ArmorItem;
        if (armorItem != null)
        {
            statsText.text = string.Format("Armor {0}\n HP {1} ", armorItem.Armor, armorItem.Health);
            return;
        }
    }

    public void ShowTooltip(ItemTemplate item, Vector2 pos)
    {
        return;
        gameObject.SetActive(true);
        pos.x -= 200;

        gameObject.transform.position = pos;

        itemImage.sprite = item.itemSprite;

        itemNameText.text = item.itemName;
        tooltipText.text = item.itemDescription;
        ClampToWindow();
        //WeaponItem weaponItem = item as WeaponItem;
        //if (weaponItem != null)
        //{
        //    statsText.text = string.Format("Damage {0}-{1}\n Required lvl:{2} ", weaponItem.minDamage, weaponItem.maxDamage, weaponItem.minLevel);
        //    return;
        //}

        //ArmorItem armorItem = item as ArmorItem;
        //if (armorItem != null)
        //{
        //    statsText.text = string.Format("Armor {0}\n Required lvl:{1} ", armorItem.Armor, armorItem.minLevel);
        //    return;
        //}
    }

    public void ShowTooltip(BaseSkillTemplate skill, Vector2 pos)
    {
        return;
        gameObject.SetActive(true);
        pos.x -= 300;

        gameObject.transform.position = pos;

        itemImage.sprite = skill.Icon;

        itemNameText.text = skill.Id.ToString();
        tooltipText.text = skill.Description;
        ClampToWindow();
        // statsText.text = string.Format("Damage {0}-{1}\n Required lvl:{2} ", talent.minDamage, talent.maxDamage, talent.minLevel);

    }


    public void ShowTooltip(BaseSkillModel skill, Vector2 pos)
    {
        return;
        if (skill == null)
        {
            return;
        }

        gameObject.SetActive(true);
        pos.x -= 100;

        gameObject.transform.position = pos;

        itemImage.sprite = skill.Icon;

        itemNameText.text = skill.Id.ToString();
        tooltipText.text = skill.Description;
        ClampToWindow();
        // statsText.text = string.Format("Damage {0}-{1}\n Required lvl:{2} ", talent.minDamage, talent.maxDamage, talent.minLevel);

    }

    public void HideTooltip()
    {
        return;
        gameObject.SetActive(false);
        statsText.text = "";
        tooltipText.text = "";
        itemNameText.text = "";
    }

    private void OnDestroy()
    {
        Instance = null;
    }

}
