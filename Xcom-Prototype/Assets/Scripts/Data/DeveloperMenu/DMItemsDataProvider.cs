using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DMItemsDataProvider
{
    [Inject] private PlayerData _playerData;

    private List<ItemTemplate> _allItems = new List<ItemTemplate>();
    private List<CharacterPreset> _allCharacters = new List<CharacterPreset>();
    private List<BaseSkillTemplate> _allSkills = new List<BaseSkillTemplate>();

    private ItemsDataInfo _itemsDataInfo;
    private SkillsDataInfo _skillsDataInfo;

    public DMItemsDataProvider()
    {
        _itemsDataInfo = ItemsDataInfo.Instance;
        _skillsDataInfo = SkillsDataInfo.Instance;

        Initialize();
    }

    #region Initialization
    private void Initialize()
    {
        InitializeItems();
        InitializeCharacters();
        InitializeSkills();
        InitializeSkills();
    }

    private void InitializeItems()
    {
        AddItemsToAllItemsList(_itemsDataInfo.Weapons);
        AddItemsToAllItemsList(_itemsDataInfo.Armors);
        AddItemsToAllItemsList(_itemsDataInfo.Rings);
        AddItemsToAllItemsList(_itemsDataInfo.Amulets);
        AddItemsToAllItemsList(_itemsDataInfo.Armlets);
        AddItemsToAllItemsList(_itemsDataInfo.CraftItems);
        AddItemsToAllItemsList(_itemsDataInfo.Recipes);
    }

    private void InitializeCharacters()
    {
        _skillsDataInfo = SkillsDataInfo.Instance;

        AddCharactersToAllCharactersList(_skillsDataInfo.CharacterPresets);

        _allCharacters.Sort((character1, character2) => string.Compare(character1.PresetName, character2.PresetName, StringComparison.OrdinalIgnoreCase));
    }

    private void InitializeSkills()
    {
        BaseSkillTemplate[] skills = _skillsDataInfo.BaseSkillModels;

        foreach (var s in skills)
        {
            _allSkills.Add(s);
        }

        _allSkills.Sort((skill1, skill2) => string.Compare(skill1.Name, skill2.Name, StringComparison.OrdinalIgnoreCase));
    }

    private void AddItemsToAllItemsList<T>(List<T> items) where T : ItemTemplate
    {
        foreach (var item in items)
        {
            _allItems.Add(item);
        }
    }

    private void AddCharactersToAllCharactersList(CharacterPreset[] allCharactersPresets)
    {
        foreach (var characterPreset in allCharactersPresets)
        {
            _allCharacters.Add(characterPreset);
        }
    }
    #endregion

    #region Getters
    public BaseItem GetItem(string itemName)
    {
        ItemTemplate foundTemplate = FindItem(itemName);
        BaseItem item = ConvertItem(foundTemplate);

        Debug.Log($">>>Created new item {item.itemName}");

        return item;
    }

    public BaseCharacterModel GetCharacter(string characterUniqueName)
    {
        CharacterPreset characterPreset = FindCharacter(characterUniqueName);
        BaseCharacterModel character = ConvertPresetToCharacter(characterPreset);

        Debug.Log($">>>Created new character {character.Name}");

        return character;
    }

    public BaseSkillModel GetSkill(string skillUniqueName)
    {
        BaseSkillTemplate skillTemplate = FindSkill(skillUniqueName);
        BaseSkillModel skill = CastTemplateToSkill(skillTemplate);

        Debug.Log($">>>Created new skill {skill.Name}");

        return skill;
    }
    #endregion

    #region Save
    public void Save(IDMData data)
    {
        if (data is BaseItem)
        {
            SaveItem(data as BaseItem);
        }
        else if (data is BaseCharacterModel)
        {
            SaveCharacter(data as BaseCharacterModel);
        }
        else if (data is BaseSkillModel)
        {
            SaveSkill(data as BaseSkillModel);
        }
    }

    private void SaveItem(BaseItem item)
    {
        _playerData.AddItemToInventory(item);

        Debug.Log($">>>Saved {item.itemName} to inventory ms.{DateTime.Now.Millisecond}");
    }

    private void SaveCharacter(BaseCharacterModel character)
    {
        _playerData.PlayerGroup.AddCharacterToNotAsignedGroup(character);

        Debug.Log($">>>Saved {character.Name} to inventory ms.{DateTime.Now.Millisecond}");
    }

    private void SaveSkill(BaseSkillModel skill)
    {
        _playerData.PlayerAvailablesSkills.Add(skill);

        Debug.Log($">>>Saved {skill.Name} to inventory ms.{DateTime.Now.Millisecond}");
    }
    #endregion

    #region Search
    private CharacterPreset FindCharacter(string characterName)
    {
        return _allCharacters
            .FirstOrDefault(stringToCheck => stringToCheck.PresetName.Contains(characterName));
    }

    private BaseSkillTemplate FindSkill(string skillUniqueName)
    {
        return _allSkills
            .FirstOrDefault(stringToCheck => stringToCheck.Name.Contains(skillUniqueName));
    }

    private ItemTemplate FindItem(string itemUniqueName)
    {
        return _allItems
            .FirstOrDefault(stringToCheck => stringToCheck.itemName.Contains(itemUniqueName));
    }
    #endregion

    #region CastItem
    private BaseItem ConvertItem(ItemTemplate template)
    {
        if (template is ArmorItemTemplate)
            return _itemsDataInfo.ConvertTemplateToItem<ArmorItem>(template);

        else if (template is AmuletItemTemplate)
            return _itemsDataInfo.ConvertTemplateToItem<AmuletItem>(template);

        else if (template is WeaponItemTemaplate)
            return _itemsDataInfo.ConvertTemplateToItem<WeaponItem>(template);

        else if (template is RingItemTemplate)
            return _itemsDataInfo.ConvertTemplateToItem<RingItem>(template);

        else if (template is ArmletItemTemplate)
            return _itemsDataInfo.ConvertTemplateToItem<ArmletItem>(template);

        else if (template is CraftItemTemplate)
            return _itemsDataInfo.ConvertTemplateToItem<CraftItem>(template);

        else if (template is ArtifactRecipeTemplate)
            return _itemsDataInfo.ConvertTemplateToItem<ArtifactRecipeTemplate>(template);

        return null;
    }

    private BaseCharacterModel ConvertPresetToCharacter(CharacterPreset characterPreset)
    {
        return characterPreset.CreateCharacter();
    }

    private BaseSkillModel CastTemplateToSkill(BaseSkillTemplate skillTemplate)
    {
        BaseSkillModel model = skillTemplate.GetSkill();

        return model;
    }
    #endregion
}