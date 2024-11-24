using UnityEngine;

[System.Serializable]
public struct IconData
{
    [field: SerializeField] public CharacterRace EnemyRace { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}

[CreateAssetMenu(fileName = "EnemiesIcons", menuName = "IconsData/EnemiesIcons")]
public class EnemiesIconsData : ScriptableObject
{
    [field: SerializeField] public IconData[] EnemiesIcons { get; private set; }

    public Sprite GetIcon(CharacterRace race)
    {
        foreach (IconData icon in EnemiesIcons)
        {
            if (icon.EnemyRace == race)
                return icon.Icon;
        }

        Debug.LogError($"Icon for race {race} does not exist in the array");

        return null;
    }
}
