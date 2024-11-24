using UnityEngine;
using UnityEngine.UI;

public class CharacterBattleSlotButton : CharacterBattleSlot
{
    public BaseCharacterModel Model => _model;

    [SerializeField] private Button _button;

    private BaseCharacterModel _model;

    private bool _isSelected;

    public void OnCreate(TestOfStrenghtWindow window, PlayerContent playerContent, BaseCharacterModel model,  bool isSelected)
    {
        _isSelected = isSelected;
        _model = model;

        _button.onClick.AddListener(() =>
        {
            if (_isSelected)
            {
                playerContent.RemoveCharacter(_model);
                window.DeselectBattleSlot(this);
                _isSelected = false;
            }
            else if (_isSelected == false && playerContent.CanAddCharacter(_model))
            {
                _isSelected = true;
                window.SelectBattleSlot(this);
            }
        });
    }
}
