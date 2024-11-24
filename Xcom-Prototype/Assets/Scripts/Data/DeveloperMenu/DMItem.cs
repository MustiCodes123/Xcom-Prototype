using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class DMItem : MonoBehaviour
{
    [Inject] private DMItemsDataProvider _itemsDataProvider;
    [SerializeField] private TMP_Text _infoText;

    private BaseItem _currentItem;
    private BaseCharacterModel _currentCharacter;
    private BaseSkillModel _currentSkill;

    public IDMData Data { get; private set; }

    public void ShowInfo(BaseItem item)
    {
        _currentItem = item;
        Data = _currentItem;

        _infoText.text = ToString(_currentItem);
    }

    public void ShowInfo(BaseCharacterModel character)
    {
        _currentCharacter = character;
        Data = _currentCharacter;

        _infoText.text = ToString(_currentCharacter);
    }

    public void ShowInfo(BaseSkillModel skill)
    {
        _currentSkill = skill;
        Data = _currentSkill;

        _infoText.text = ToString(_currentSkill);
    }

    public void AddToInventory(IDMData data)
    {
        _itemsDataProvider.Save(data);
    }

    private string ToString(BaseItem item)
    {
        var sb = new StringBuilder();

        sb.Append("Name: ").Append(item.itemName).Append("\n")
            .Append("itemID: ").Append(item.itemID).Append("\n")
            .Append("itemPrice: ").Append(item.itemPrice).Append("\n");

        return sb.ToString();
    }

    private string ToString(BaseCharacterModel character)
    {
        var sb = new StringBuilder();

        sb.Append("Name: ").Append(character.Name).Append("\n")
            .Append("Rare: ").Append(character.Rare).Append("\n");

        return sb.ToString();
    }

    private string ToString(BaseSkillModel skill)
    {
        var sb = new StringBuilder();

        sb.Append("Name: ").Append(skill.Name).Append("\n")
            .Append("ID: ").Append(skill.Id).Append("\n")
            .Append("Cost: ").Append(skill.Cost).Append("\n");

        return sb.ToString();
    }
}

