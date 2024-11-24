using UnityEngine;
using TMPro;

public class BossThreeToOneContent : BossContent
{
    [SerializeField] private TMP_Text _bossHealthText;

    public override void ChangeDifficulty(int value)
    {
        base.ChangeDifficulty(value);
        _bossHealthText.text = (BossData.BossPreset.BossStats[value].HP).ToString();

        if (_bossWindow is ThreeToOneWindow window)
        {
            window.InitializeTeamMembers(BossData);
        }
    }
}
