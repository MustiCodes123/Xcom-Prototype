using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InfoPopup : MonoBehaviour
{
    public static InfoPopup Instance;

    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI statsText;

    [SerializeField] private Image itemImage;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private TextMeshProUGUI rightButtonText;
    [SerializeField] private List<Sprite> currencySprites = new List<Sprite>();

    private readonly Dictionary<BaseItem, List<SkillPopupView>> _displayedSkills = new Dictionary<BaseItem, List<SkillPopupView>>();
    [SerializeField] private Transform _skillsInfoContainer;
    [SerializeField] private SkillPopupView _skillViewPrefab;
    [SerializeField] private ItemStatsViewMore _itemStatsView;
    [SerializeField] private RarityStars _rarityStars;


    private Action OnleftButton;
    private Action OnRightButton;

    private ItemsDataInfo itemsDataInfo;

    private const float _popupAnimationSpeed = 0.5f;

    [Inject]
    private void Init(ItemsDataInfo itemsDataInfo)
    {
        Instance = this;

        this.itemsDataInfo = itemsDataInfo;

        leftButton.onClick.AddListener(OnLeftButtonClicked);
        rightButton.onClick.AddListener(OnRightButtonClicked);
        closeButton.onClick.AddListener(Hide);
    }

    private void OnRightButtonClicked()
    {
        OnRightButton?.Invoke();
        Hide();
    }

    private void OnLeftButtonClicked()
    {
        OnleftButton?.Invoke();
        Hide();
    }

    public void ActivateButtons(string leftButtonName, string rightButtonName, Action leftButtonAction, Action rightButtonAction)
    {
        OnleftButton = null;
        OnRightButton = null;
        OnleftButton = leftButtonAction;
        OnRightButton = rightButtonAction;

        if (!string.IsNullOrEmpty(leftButtonName))
        {

            leftButton.gameObject.SetActive(true);
            leftButtonText.text = leftButtonName;
        }
        else
        {
            leftButton.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(rightButtonName))
        {
            rightButton.gameObject.SetActive(true);
            rightButtonText.text = rightButtonName;
        }
        else
        {
            rightButton.gameObject.SetActive(false);
        }
    }

    public void ShowTooltip(BaseItem item)
{
    gameObject.SetActive(true);

    itemImage.sprite = item.itemSprite;
    itemNameText.text = item.itemName;
    tooltipText.text = item.itemDescription;

    ClearDisplayedSkills();
    DisplaySkills(item);
    itemNameText.text = CalculateTitle(item);
    _itemStatsView.DisplayStats(item);

    UpdateRarityStars(item);

    WeaponItem weaponItem = item as WeaponItem;
    if (weaponItem != null)
    {
        statsText.text = string.Format("Damage {0}-{1}\n Critical Chance {2}", weaponItem.minDamage, weaponItem.maxDamage, weaponItem.CriticalChance);
        return;
    }

    ArmorItem armorItem = item as ArmorItem;
    if (armorItem != null)
    {
        statsText.text = string.Format("Armor {0}\n HP {1} ", armorItem.Armor, armorItem.Health);
        return;
    }

    RingItem ringItem = item as RingItem;
    if (ringItem != null)
    {
        statsText.text = string.Format("Magic Damage Buff {0}\n Mana Cost Reduction {1}\n Mana Regeneration Rate Buff {2}",
            ringItem.MagicDamageBuff, ringItem.ManaCostReduction, ringItem.ManaRegenerationRateBuff);

        return;
    }

    AmuletItem amuletItem = item as AmuletItem;
    if (amuletItem != null)
    {
        statsText.text = string.Format("Critical Chance {0}\n Dodge Chance {1}\n Attack Accuracy {2}\n Movement Speed Buff {3}",
            amuletItem.CriticalChance, amuletItem.DodgeChance, amuletItem.AttackAccuracy, amuletItem.MovementSpeedBuff);

        return;
    }
}

    public void ShowTooltip(BaseItem item, string discription)
    {
        gameObject.SetActive(true);

        itemImage.sprite = item.itemSprite;

        itemNameText.text = item.itemName;
        tooltipText.text = discription;

        WeaponItem weaponItem = item as WeaponItem;
        if (weaponItem != null)
        {
            statsText.text = string.Format("Damage {0}-{1}\n Critical Chance {2}", weaponItem.minDamage, weaponItem.maxDamage, weaponItem.CriticalChance);
            return;
        }

        ArmorItem armorItem = item as ArmorItem;
        if (armorItem != null)
        {
            statsText.text = string.Format("Armor {0}\n HP {1} ", armorItem.Armor, armorItem.Health);
            return;
        }
    }

    public void ShowTooltip(ItemTemplate template)
    {
        gameObject.SetActive(true);

        itemImage.sprite = template.itemSprite;

        itemNameText.text = template.itemName;
        tooltipText.text = template.itemDescription;


        var item = itemsDataInfo.ConvertTemplateToItem<BaseItem>(template);

        WeaponItem weaponItem = item as WeaponItem;
        if (weaponItem != null)
        {
            statsText.text = string.Format("Damage {0}-{1}\n Critical Chance {2}", weaponItem.minDamage, weaponItem.maxDamage, weaponItem.CriticalChance);
            return;
        }

        ArmorItem armorItem = item as ArmorItem;
        if (armorItem != null)
        {
            statsText.text = string.Format("Armor {0}\n HP {1} ", armorItem.Armor, armorItem.Health);
            return;
        }

    }

    public void ShowTooltip(BaseSkillTemplate skill)
    {
        gameObject.SetActive(true);

        itemImage.sprite = skill.Icon;

        itemNameText.text = skill.Id.ToString();
        tooltipText.text = skill.Description;
    }


    public void ShowTooltip(BaseSkillModel skill)
    {
        if (skill == null)
        {
            return;
        }

        gameObject.SetActive(true);

        itemImage.sprite = skill.Icon;

        itemNameText.text = skill.Id.ToString();
        tooltipText.text = skill.Description;


        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
    }

    public void ShowTooltip(Quest quest)
    {
        gameObject.SetActive(true);
        itemNameText.text = quest.Name;
        tooltipText.text = quest.Description;

        if (quest.QuestIcon != null)
        {
            itemImage.sprite = quest.QuestIcon;
        }
    }

    public void ShowTooltip(Sprite priceImage, GameCurrencies type, uint count)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, _popupAnimationSpeed);
        gameObject.SetActive(true);
        itemNameText.text = type.ToString();
        tooltipText.text = "Buy One More Upgrade Slot.";
        itemImage.sprite = priceImage;
    }

    public void ShowTooltipBuyCurrency(GameCurrencies type)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, _popupAnimationSpeed);
        gameObject.SetActive(true);
        itemNameText.text = type.ToString();
        tooltipText.text = "Not enough " + type.ToString() + ".";
        itemImage.sprite = GetSpriteByCurrencyType(type);
    }

    public void ShowTooltipNotEnpughtEnergy()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, _popupAnimationSpeed);
        gameObject.SetActive(true);
        itemNameText.text = GameCurrencies.Energy.ToString();
        tooltipText.text = "Your energy has run out, go to the store to continue.";
        itemImage.sprite = currencySprites[2];
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        statsText.text = "";
        tooltipText.text = "";
        itemNameText.text = "";
    }
    private void UpdateRarityStars(BaseItem item)
{
    if (_rarityStars != null)
    {
        _rarityStars.StarsSetActive(item.Rare);
    }
}


    private Sprite GetSpriteByCurrencyType(GameCurrencies currencyType)
    {
        switch (currencyType)
        {
            case GameCurrencies.Gem:
                return currencySprites[0];
            case GameCurrencies.Gold:
                return currencySprites[1];
            case GameCurrencies.Energy:
                return currencySprites[2];
            case GameCurrencies.Key:
                return currencySprites[3];
            default:
                return currencySprites[0];
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        DOTween.KillAll();
    }

    private void DisplaySkills(BaseItem item)
    {
        if (!_displayedSkills.ContainsKey(item))
        {
            List<BaseSkillModel> itemSkills = item.skillModels;
            List<SkillPopupView> skillViews = new List<SkillPopupView>();
            Debug.Log($"Item skills count: {itemSkills.Count}");
            foreach (BaseSkillModel skill in itemSkills)
            {
                SkillPopupView skillView = Instantiate(_skillViewPrefab, _skillsInfoContainer);
                skillView.Display(skill);
                skillViews.Add(skillView);
            }

            _displayedSkills[item] = skillViews;
        }
        else
        {
            foreach (SkillPopupView skillView in _displayedSkills[item])
            {
                skillView.gameObject.SetActive(true);
            }
        }
    }
    private void ClearDisplayedSkills()
    {
        foreach (List<SkillPopupView> skillViews in _displayedSkills.Values)
        {
            foreach (SkillPopupView skillView in skillViews)
            {
                skillView.gameObject.SetActive(false);
            }
        }
    }
    private string CalculateTitle(BaseItem item)
    {
        string itemType = string.Empty;
        string skillSetName = string.Empty;
        string itemSetName = string.Empty;

        if (item != null)
        {
            itemType = CalculateItemTypeString(item);

            if (item.ItemsSet != null)
            {
                if (item.ItemsSet.SkillSet != null)
                {
                    skillSetName = item.ItemsSet.SkillSet.Name;
                }

                itemSetName = item.ItemsSet.SetName;
            }
        }

        return $"{skillSetName} {itemSetName} {itemType}";
    }
    private string CalculateItemTypeString(BaseItem item)
    {
        return item switch
        {
            ArmorItem armorItem => armorItem.Slot.ToString(),
            WeaponItem weaponItem => weaponItem.weaponType.ToString(),
            ArmletItem => "Armlet",
            RingItem => "Ring",
            AmuletItem => "Amulet",

            _ => "Item"
        };
    }
}

[Serializable]
public class ItemStatsViewMore
{
    [field: SerializeField] public GameObject DamageContainer { get; private set; }
    [field: SerializeField] public TMP_Text Damage { get; private set; }

    [field: SerializeField] public GameObject CriticalChanceContainer { get; private set; }
    [field: SerializeField] public TMP_Text CriticalChance { get; private set; }

    [field: SerializeField] public GameObject ArmorContainer { get; private set; }
    [field: SerializeField] public TMP_Text Armor { get; private set; }

    [field: SerializeField] public GameObject HealthPointsContainer { get; private set; }
    [field: SerializeField] public TMP_Text HealthPoints { get; private set; }

    [field: SerializeField] public GameObject MagicDamageBuffContainer { get; private set; }
    [field: SerializeField] public TMP_Text MagicDamageBuff { get; private set; }

    [field: SerializeField] public GameObject ManaCostReductionContainer { get; private set; }
    [field: SerializeField] public TMP_Text ManaCostReduction { get; private set; }

    [field: SerializeField] public GameObject ManaRegenerationContainer { get; private set; }
    [field: SerializeField] public TMP_Text ManaRegeneration { get; private set; }

    [field: SerializeField] public GameObject DodgeChanceContainer { get; private set; }
    [field: SerializeField] public TMP_Text DodgeChance { get; private set; }

    [field: SerializeField] public GameObject AttackAccuracyContainer { get; private set; }
    [field: SerializeField] public TMP_Text AttackAccuracy { get; private set; }

    [field: SerializeField] public GameObject SpeedBuffContainer { get; private set; }
    [field: SerializeField] public TMP_Text SpeedBuff { get; private set; }

    public void DisplayStats(BaseItem item)
    {
        HideAllStats();

        switch (item)
        {
            case WeaponItem weaponItem:
                DamageContainer.SetActive(true);
                Damage.text = $"{weaponItem.minDamage}-{weaponItem.maxDamage}";

                CriticalChanceContainer.SetActive(weaponItem.CriticalChance > 0);
                CriticalChance.text = $"{weaponItem.CriticalChance}";

                ArmorContainer.SetActive(weaponItem.Armor > 0);
                Armor.text = $"{weaponItem.Armor}";

                HealthPointsContainer.SetActive(weaponItem.Health > 0);
                HealthPoints.text = $"{weaponItem.Health}";

                DodgeChanceContainer.SetActive(weaponItem.BlockChance > 0);
                DodgeChance.text = $"{weaponItem.BlockChance}";
                break;

            case ArmorItem armorItem:
                ArmorContainer.SetActive(armorItem.Armor > 0);
                Armor.text = $"{armorItem.Armor}";

                HealthPointsContainer.SetActive(armorItem.Health > 0);
                HealthPoints.text = $"{armorItem.Health}";

                DodgeChanceContainer.SetActive(armorItem.DodgeChance > 0);
                DodgeChance.text = $"{armorItem.DodgeChance}";
                break;

            case RingItem ringItem:
                MagicDamageBuffContainer.SetActive(ringItem.MagicDamageBuff > 0);
                MagicDamageBuff.text = $"{ringItem.MagicDamageBuff}";

                ManaCostReductionContainer.SetActive(ringItem.ManaCostReduction > 0);
                ManaCostReduction.text = $"{ringItem.ManaCostReduction}";
                break;

            case AmuletItem amuletItem:
                CriticalChanceContainer.SetActive(amuletItem.CriticalChance > 0);
                CriticalChance.text = $"{amuletItem.CriticalChance}";

                DodgeChanceContainer.SetActive(amuletItem.DodgeChance > 0);
                DodgeChance.text = $"{amuletItem.DodgeChance}";

                AttackAccuracyContainer.SetActive(amuletItem.AttackAccuracy > 0);
                AttackAccuracy.text = $"{amuletItem.AttackAccuracy}";

                SpeedBuffContainer.SetActive(amuletItem.MovementSpeedBuff > 0);
                SpeedBuff.text = $"{amuletItem.MovementSpeedBuff}";
                break;
        }
    }

    private void HideAllStats()
    {
        DamageContainer.SetActive(false);
        CriticalChanceContainer.SetActive(false);
        ArmorContainer.SetActive(false);
        HealthPointsContainer.SetActive(false);
        MagicDamageBuffContainer.SetActive(false);
        ManaCostReductionContainer.SetActive(false);
        ManaRegenerationContainer.SetActive(false);
        DodgeChanceContainer.SetActive(false);
        AttackAccuracyContainer.SetActive(false);
        SpeedBuffContainer.SetActive(false);
    }
}
