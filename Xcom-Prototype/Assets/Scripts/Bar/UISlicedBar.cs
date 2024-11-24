using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlicedBar : MonoBehaviour
{
    public UIShieldIcon UIShieldIcon => _shieldIkon;

    [SerializeField] private Color _allyHpColor = new Color(0, 1, 0, 1);
    [SerializeField] private Color _enemyHpColor = new Color(1, 0, 0, 1);
    [SerializeField] private Image _hp;
    [SerializeField] private Slider _HPImage;
    [SerializeField] private Slider _MagicArmorImage;
    [SerializeField] private Slider _ManaImage;
    [SerializeField] private UIShieldIcon _shieldIkon;
    [SerializeField] private Image[] _Images;
    [SerializeField] private BuffIconContainer _buffIcons;
    [SerializeField] private Image _Avatar;
    [SerializeField] private TextMeshProUGUI _LevelText;

    [SerializeField] private GameObject separatorPrefab;
    [SerializeField] private Transform hpSeparatorParent;
    [SerializeField] private Transform manaSeparatorParent;

    private BaseCharacerView _myCharacterView;

    private readonly int _separatorCount = 10000;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _myCharacterView.OnTakeDamage -= OnTakeDamage;
    }

    private void OnTakeDamage(int dmg)
    {
        if(dmg >= 0)
            gameObject.SetActive(true);
    }

    public void SetTeam(Team team, BaseCharacerView character)
    {
        _myCharacterView = character;
        gameObject.SetActive(false);

        _myCharacterView.OnTakeDamage += OnTakeDamage;

        foreach (var image in _Images)
        {
            image.color = team == Team.Allies ? Color.blue : Color.red;
        }
        if (team == Team.Allies)
            _hp.color = _allyHpColor;
        else
            _hp.color = _enemyHpColor;
    }

    public void SetLevel(int level)
    {
        _LevelText.text = (level).ToString();
    }

    public void SetArmor(float value)
    {
        if (float.IsInfinity(value)) value = 0;
        _MagicArmorImage.value = value;
    }
    
    public void SetMaxHP(int value)
    {
        int hpChildrens = hpSeparatorParent.childCount;
        for (int i = 0; i < hpChildrens; i++)
        {
            hpSeparatorParent.GetChild(i).gameObject.SetActive(false);
        }

        int maxCount = value / _separatorCount;
        for (int i = 0; i < maxCount; i++)
        {
            if (i < hpChildrens)
            { 
                hpSeparatorParent.GetChild(i).gameObject.SetActive(!_myCharacterView.IsDead); 
            }
            else
            {
                var separator = Instantiate(separatorPrefab, hpSeparatorParent);
            }
        }
    }

    public void SetMaxMana(int value)
    {
        int hpChildrens = manaSeparatorParent.childCount;
        for (int i = 0; i < hpChildrens; i++)
        {
            manaSeparatorParent.GetChild(i).gameObject.SetActive(false);
        }

        int maxCount = value / _separatorCount;
        for (int i = 0; i < maxCount; i++)
        {
            if (i < hpChildrens)
            {
                manaSeparatorParent.GetChild(i).gameObject.SetActive(!_myCharacterView.IsDead);
            }
            else
            {
                var separator = Instantiate(separatorPrefab, manaSeparatorParent);
            }
        }
    }

    public void SetHPValue(float value)
    {
        if (float.IsInfinity(value)) value = 0;

        if(_myCharacterView != null)
            gameObject.SetActive(!_myCharacterView.IsDead);

        _HPImage.value = value;
        if (value <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetManaValue(float value)
    {
        _ManaImage.value = value;

        if (_myCharacterView != null)
            gameObject.SetActive(!_myCharacterView.IsDead);
    }

    public void SetBuffOnBar(Sprite icon)
    {
        if (_myCharacterView != null)
            gameObject.SetActive(!_myCharacterView.IsDead);

        _buffIcons.SetBuff(icon);
    }
    public void RemoveBuufOnBar(Sprite icon)
    {
        _buffIcons.ReleveIcon(icon);
    }
    public void SetBigBuff(Sprite icon)
    {
        if (_myCharacterView != null)
            gameObject.SetActive(!_myCharacterView.IsDead);

        _buffIcons.SetBigIcon(icon);
    }
    public void RemoveBigBuff()
    {
        _buffIcons.ReleaveBigIcon();
    }

    public void SetShieldIcon()
    {
        _shieldIkon.gameObject.SetActive(false);
    }
}
