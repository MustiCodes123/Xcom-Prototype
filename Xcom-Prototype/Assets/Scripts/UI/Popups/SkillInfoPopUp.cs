using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SkillInfoPopUp : MonoBehaviour
{
    public static SkillInfoPopUp Instance;

    [SerializeField] private Image _skillIcon;
    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TextMeshProUGUI _skillDamage;
    [SerializeField] private TextMeshProUGUI _skillMana;
    [SerializeField] private Button _closeButton;

    [Inject]
    private void Constructor()
    {
        Instance = this;
    }
    
    private void Awake()
    {
        _closeButton.onClick.AddListener(Hide);
    }

    public void Show(BaseSkillTemplate skill)
    {
        _skillIcon.sprite = skill.Icon;
        _skillName.text = skill.Id.ToString();
        _skillDamage.text = skill.Value.ToString();
        _skillMana.text = skill.ManaCost.ToString();
        _description.text = skill.Description;

        gameObject.SetActive(true);
    }

    public void Show(BaseSkillModel skill)
    {
        _skillIcon.sprite = skill.Icon;
        _skillName.text = skill.Id.ToString();
        _skillDamage.text = skill.Value.ToString();
        _skillMana.text = skill.ManaCost.ToString();
        _description.text = skill.Description;

        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(Hide);
    }

    public void Show(object skill)
    {
        throw new System.NotImplementedException();
    }
}