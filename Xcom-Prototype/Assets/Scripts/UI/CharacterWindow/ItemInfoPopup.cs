using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Zenject;
using System.Collections.Generic;
using System.Linq;

#region View Data Classes
[Serializable]
public class ItemStatsView
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

[Serializable]
public class RarityBackgroundsViewData
{
    [field: SerializeField] public Sprite Common { get; private set; }
    [field: SerializeField] public Sprite Rare { get; private set; }
    [field: SerializeField] public Sprite Epic { get; private set; }
    [field: SerializeField] public Sprite Legendary { get; private set; }
    [field: SerializeField] public Sprite Mythical { get; private set; }

    public Sprite GetBackgroundForRarity(BaseItem item)
    {
        switch (item.Rare)
        {
            case RareEnum.Common:
                return Common;

            case RareEnum.Rare:
                return Rare;

            case RareEnum.Epic:
                return Epic;

            case RareEnum.Legendary:
                return Legendary;

            case RareEnum.Mythical:
                return Mythical;

            default:
                Debug.LogError($"Cannot find sprite for {item.Rare} rarity type. Add sprite for this rarity");
                return null;
        }
    }
}

[Serializable]
public class RarityStars
{
    public const int COUNT = 5;
    public GameObject[] Stars = new GameObject[COUNT];

    public void StarsSetActive(RareEnum rarity)
    {
        for (int i = 0; i < COUNT; i++)
        {
            if (i <= (int)rarity)
            {
                Stars[i].gameObject.SetActive(true);
            }
            else
            {
                Stars[i].gameObject.SetActive(false);
            }
        }
    }
}
#endregion

public class ItemInfoPopup : MonoBehaviour
{
    public static ItemInfoPopup Instance;

    [SerializeField] private ItemStatsView _itemStatsView;
    [SerializeField] private RarityBackgroundsViewData _backgrounds;
    [SerializeField] private RarityStars _rarityStars;

    [SerializeField] private TMP_Text _titleTMP;
    [SerializeField] private TMP_Text _descriptionTMP;

    [SerializeField] private Image _image;

    [SerializeField] private Transform _skillsInfoContainer;
    [SerializeField] private SkillPopupView _skillViewPrefab;

    [SerializeField] private Button _actionButton;
    [SerializeField] private Button _cancelButton;

    private readonly Dictionary<BaseItem, List<SkillPopupView>> _displayedSkills = new Dictionary<BaseItem, List<SkillPopupView>>();
    private readonly Dictionary<ItemTemplate, List<SkillPopupView>> _displayedSkillsTemplate = new Dictionary<ItemTemplate, List<SkillPopupView>>();

    [Inject]
    private void Constructor()
    {
        Instance = this;
    }

    public void Show(BaseItem item)
    {
        ClearDisplayedSkills();

        _titleTMP.text = CalculateTitle(item);
        _descriptionTMP.text = item.itemDescription;

        _image.sprite = item.itemSprite;

        _itemStatsView.DisplayStats(item);
        DisplaySkills(item);
        _rarityStars.StarsSetActive(item.Rare);

        gameObject.SetActive(true);
    }

    public void ActivateButtons(string generalButtonName, Action generalAction, Action cancelAction, bool isGeneralButtonDisabled = false)
    {
        if (isGeneralButtonDisabled)
        {
            _actionButton.gameObject.SetActive(false);

            _actionButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _actionButton.onClick.AddListener(() =>
            {
                ClearDisplayedSkills();
                generalAction();
            });
            _cancelButton.onClick.AddListener(() =>
            {
                ClearDisplayedSkills();
                cancelAction();
            });

            return;
        }

        _actionButton.gameObject.SetActive(true);

        _actionButton.GetComponentInChildren<TMP_Text>().text = generalButtonName;

        _actionButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _actionButton.onClick.AddListener(() =>
        {
            ClearDisplayedSkills();
            generalAction();
        });
        _cancelButton.onClick.AddListener(() =>
        {
            ClearDisplayedSkills();
            cancelAction();
        });
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

        foreach (List<SkillPopupView> skillViews in _displayedSkillsTemplate.Values)
        {
            foreach (SkillPopupView skillView in skillViews)
            {
                skillView.gameObject.SetActive(false);
            }
        }
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