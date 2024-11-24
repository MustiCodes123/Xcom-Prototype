using System.Diagnostics;
using Zenject;
using UnityEngine;

public class CharacterHandler
{
    readonly SignalBus _signalBus;
    readonly PlayerData _playerData;

    private BaseCharacterModel currentCharacterInfo;

    public CharacterHandler(SignalBus signalBus, PlayerData playerData)
    {
        _playerData = playerData;
        _signalBus = signalBus;

        _signalBus.Subscribe<CharacterSelectSignal>(OnCharacterChanged);
    }

    public BaseCharacterModel GetCurrentCharacterInfo()
    {
        return currentCharacterInfo;
    }

    public void SetActiveCharacter(BaseCharacterModel characterInfo)
    {
        currentCharacterInfo = characterInfo;
    }

    private void OnCharacterChanged(CharacterSelectSignal characterSelectSignal)
    {
        currentCharacterInfo = characterSelectSignal.CharacterInfo;
    }

    public void ChangeCharacter(BaseCharacterModel characterInfo)
    {
        _signalBus.Fire(new CharacterSelectSignal() { CharacterInfo = characterInfo });
    }

    public void LevelUpCharacter(BaseCharacterModel characterInfo)
    {
        characterInfo.LevelUp();
        _signalBus.Fire(new CharacterLevelpSignal() { CharacterInfo = characterInfo });
        _signalBus.Fire(new CharacterSelectSignal() { CharacterInfo = characterInfo });
    }

    public void SetCharacterToGroup(BaseCharacterModel characterInfo, GroupType group)
    {
        _signalBus.Fire(new CharacterAddedToGroupSignal { CharacterInfo = characterInfo, GroupType = group });
        _playerData.MoveCharacterToGroup(characterInfo, group);
    }

    public void EquipItem(BaseItem baseItem)
    {
        _playerData.MoveItemToCharacterInventory(currentCharacterInfo, baseItem);
        _signalBus.Fire(new CharacterEquipSignal() { baseCharacterInfo = currentCharacterInfo, baseItem = baseItem });
        _signalBus.Fire(new CharacterSelectSignal() { CharacterInfo = currentCharacterInfo });
    }

    public void UnequipItem(BaseItem baseItem)
    {
        if (!IsItemEquiped(baseItem)) return;
        _playerData.MoveItemFromCharacterToInventory(currentCharacterInfo, baseItem);
        _signalBus.Fire(new CharacterSelectSignal() { CharacterInfo = currentCharacterInfo });
    }

    public bool IsItemEquiped(BaseItem baseItem)
    {
        if (currentCharacterInfo == null) SetActiveCharacter(_playerData.PlayerGroup.GetCharactersFromBothGroup()[0]);
        return _playerData.IsItemEquiped(currentCharacterInfo, baseItem);
    }
}