using UnityEngine;

public class BossIconsFactory : MonoBehaviour
{
    private BossIconsData _bossIconsData;

    public BossIconsFactory(BossIconsData bossIconsData)
    {
        _bossIconsData = bossIconsData;
    }

    public Sprite CreateBossIcon(BossName bossName)
    {
        return _bossIconsData.GetIcon(bossName);
    }

}
