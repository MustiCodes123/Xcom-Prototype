using UnityEngine;

public class UISkillHolder : MonoBehaviour
{
    public SkillView[] SkillObjects;

    private void Awake()
    {
        foreach (SkillView skillObject in SkillObjects)
        {
            skillObject.gameObject.SetActive(false);
        }
    }

    public void UpDateSkillSlot(SkillView skillSlot, BaseSkillBehaviour talentBehaviour)
    {
        skillSlot.gameObject.SetActive(true);
        skillSlot.icon.sprite = talentBehaviour.Skill.Icon;
        skillSlot.skillName.text = talentBehaviour.Skill.Name;
    }
}
