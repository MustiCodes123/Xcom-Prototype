using TMPro;
using UnityEngine;
using Zenject;

public class DMWindowView : MonoBehaviour
{
    [Inject] private DMItemsDataProvider _itemsDataProvider;
    [Inject] private PlayerData _playerData;
    [SerializeField] private DMItem _currentItem;
    [SerializeField] private TMP_Text _xpValueView;
    
    public void InitializeItemView(string text, DMItemType itemType)
    {
        switch (itemType)
        {
            case DMItemType.BaseCharacterModel:
                BaseCharacterModel character = _itemsDataProvider.GetCharacter(text);
                _currentItem.ShowInfo(character);

                break;

            case DMItemType.BaseSkillModel:
                BaseSkillModel skillModel = _itemsDataProvider.GetSkill(text);
                _currentItem.ShowInfo(skillModel);

                break;

            case DMItemType.BaseItem:
                BaseItem item = _itemsDataProvider.GetItem(text);
                _currentItem.ShowInfo(item);

                break;
        }   
    }

    public void ShowCurrentXP()
    {
        _xpValueView.text = $"current XP: {_playerData.PlayerXP.ToString()}";
    }
}
