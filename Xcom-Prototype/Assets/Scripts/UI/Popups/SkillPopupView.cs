using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPopupView : MonoBehaviour
{
    [SerializeField] private Image _skillIcon;
    [SerializeField] private TMP_Text _description;

    public void Display(BaseSkillTemplate skill)
    {
        _skillIcon.sprite = skill.Icon;
        _description.text = skill.Name;
    }

    public void Display(BaseSkillModel skill)
    {
        _skillIcon.sprite = skill.Icon;
        _description.text = skill.Name;
    }
}