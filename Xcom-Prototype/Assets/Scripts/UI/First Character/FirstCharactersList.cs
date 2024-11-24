using UnityEngine;

[CreateAssetMenu(fileName = "FirstCharactersList", menuName = "Data/FirstCharactersList")]
public class FirstCharactersList : ScriptableObject
{
    [field: SerializeField] public FirstCharacterDataContainer[] FirstCharacters = new FirstCharacterDataContainer[5];
}
