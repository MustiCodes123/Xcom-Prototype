using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class BaseGroupInfo
{
    [JsonIgnore]
    public Action<BaseCharacterModel> CharacterMovedToStorage;

    public string GroupName;
    public int MaxGroupSize;

    [JsonProperty]
    private List<BaseCharacterModel> BattleCharacters = new List<BaseCharacterModel>();
    [JsonProperty]
    private List<BaseCharacterModel> NotAsignedCharacters = new List<BaseCharacterModel>();
    [JsonProperty]
    private List<BaseCharacterModel> StorageCharacters = new List<BaseCharacterModel>();

    public void ResetBattleCharacters()
    {
        NotAsignedCharacters.AddRange(BattleCharacters);
        BattleCharacters.Clear();
    }

    public bool IsCharacterInGroup(BaseCharacterModel character)
    {
        return BattleCharacters.Contains(character);
    }

    public void AddCharacterToBattleGroup(BaseCharacterModel character)
    {
         BattleCharacters.Add(character);
    }

    public void RemoveCharacterFromBattleGroup(BaseCharacterModel character)
    {
        if (BattleCharacters.Contains(character))
            BattleCharacters.Remove(character);
    }

    public BaseCharacterModel[] GetCharactersFromBatleGroup()
    {
        return BattleCharacters.ToArray();
    }

    public void AddCharacterToNotAsignedGroup(BaseCharacterModel character)
    {
        if(GetCharactersFromBothGroup().Count >= MaxGroupSize)
        {
            if(!StorageCharacters.Contains(character))
                StorageCharacters.Add(character);

            CharacterMovedToStorage?.Invoke(character);

            return;
        }

        NotAsignedCharacters.Add(character);
    }

    public void RemoveCharacterFromNotAsignedGroup(BaseCharacterModel character)
    {
        if (NotAsignedCharacters.Contains(character))
            NotAsignedCharacters.Remove(character);

        else
            Debug.LogError($"List does not contain {character.Name} character instance");
    }

    public void DeleteCharacer(BaseCharacterModel characterModel)
    {
        if (NotAsignedCharacters.Contains(characterModel))
        {
            NotAsignedCharacters.Remove(characterModel);
            return;
        }
        if (BattleCharacters.Contains(characterModel))
        {
            BattleCharacters.Remove(characterModel);
        }

    }

    public List<BaseCharacterModel> GetCharactersFromNotAsignedGroup()
    {
        return NotAsignedCharacters;
    }

    public List<BaseCharacterModel> GetCharactersFromBothGroup()
    {
        List<BaseCharacterModel> characters = new List<BaseCharacterModel>();
        characters.AddRange(BattleCharacters);
        characters.AddRange(NotAsignedCharacters);
        return characters;
    }

    public List<BaseCharacterModel> GetStorageCharacters() => StorageCharacters;

    public void RemoveFromStorage(BaseCharacterModel characterToRemove)
    {
        if (StorageCharacters.Contains(characterToRemove))
        {
            StorageCharacters.Remove(characterToRemove);
        }
    }
}