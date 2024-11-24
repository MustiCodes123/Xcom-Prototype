using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CharacterPresetData
{
    [SerializeField] private string _Name;
    public string Name { get => _Name; }

    [SerializeField] private CharacterPreset _CharacterPreset;
    public CharacterPreset CharacterPreset { get => _CharacterPreset; }
}

[CreateAssetMenu(fileName = "CharacterPresetsRegister", menuName = "Registers/CharacterPresetsRegister")]
public class CharacterPresetsRegister : ScriptableObject
{
    [SerializeField] private List<CharacterPresetData> AllCharactersPresetData = new();

    public IEnumerator<CharacterPresetData> GetEnumerator()
    {
        for (int i = 0; i < AllCharactersPresetData.Count; i++)
        {
            yield return AllCharactersPresetData[i];
        }
    }
}