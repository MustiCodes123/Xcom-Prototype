using UnityEngine;

[System.Serializable]
public struct BossIconData
{
    [field: SerializeField] public BossName BossName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}

[CreateAssetMenu(fileName = "BossIcons", menuName = "IconsData/BossIcons")]
public class BossIconsData : ScriptableObject
{
    [field: SerializeField] public BossIconData[] BossIcons { get; private set; }

    public Sprite GetIcon(BossName bossName)
    {
        foreach (BossIconData icon in BossIcons)
        {
            if (icon.BossName == bossName)
                return icon.Icon;
        }

        Debug.LogError($"Icon for boss {bossName} does not exist in the array");

        return null;
    }
}
