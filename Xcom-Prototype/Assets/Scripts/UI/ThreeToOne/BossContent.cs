using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public abstract class BossContent : MonoBehaviour
{
    public BossData BossData { get; private set; }

    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private Image _bossIcon;

    [SerializeField] private TMP_Text _streghtText;
    [SerializeField] private TMP_Text _agilityText;
    [SerializeField] private TMP_Text _intelligenceText;

    [SerializeField] private TMP_Dropdown _difficultDropdown;

    [Inject] protected ResourceManager _resourceManager;

    protected Sprite _dropdownSprite;
    protected BossWindowView _bossWindow;

    public virtual void Initialize(BossData boss, BossWindowView windowView)
    {
        _dropdownSprite = _difficultDropdown.GetComponent<Image>().sprite;

        BossData = boss;

        _bossNameText.text = boss.BossName.ToString();
        
        _bossIcon.sprite = boss.BossPreset.CharacterSprite;

        _bossWindow = windowView;

        ChangeDifficulty(0);

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < (int)boss.MaxDifficulty + 1; i++)
        {
            if (i == 0)
                options.Add(new TMP_Dropdown.OptionData("EASY", _dropdownSprite));
            if (i == 1)
                options.Add(new TMP_Dropdown.OptionData("RARE", _dropdownSprite));
            if (i == 2)
                options.Add(new TMP_Dropdown.OptionData("EPIC", _dropdownSprite));
            if (i == 3)
                options.Add(new TMP_Dropdown.OptionData("LEGENDARY", _dropdownSprite));
            if (i == 4)
                options.Add(new TMP_Dropdown.OptionData("MYTHICAL", _dropdownSprite));
        }

        _difficultDropdown.AddOptions(options);

        _difficultDropdown.onValueChanged.AddListener(value =>
        {
            ChangeDifficulty(value);
        });
    }

    public virtual void ChangeDifficulty(int value)
    {
        BossData.Difficulty = (Difficulty)value;
        _streghtText.text = (BossData.BossPreset.BossStats[value].Damage.ToString());// * BossData.DifficiltyStatsMultiplier[value]).ToString();
        _agilityText.text = (BossData.BossPreset.BossStats[value].MoveSpeed.ToString()); //* BossData.DifficiltyStatsMultiplier[value]).ToString();
        _intelligenceText.text = (BossData.BossPreset.BossStats[value].HP.ToString()); //* BossData.DifficiltyStatsMultiplier[value]).ToString();
        _bossWindow.CreateRewards();
    }
}
