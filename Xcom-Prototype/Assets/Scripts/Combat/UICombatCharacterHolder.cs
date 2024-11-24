using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class UICombatCharacterHolder : MonoBehaviour
{
    [SerializeField] private BossSliderView _bossSliderView;
    [SerializeField] private GroupProgressIcon[] _groupProgressIcons;
    [SerializeField] private CharacterCardView _characterCardViewPrefab;
    [SerializeField] private CharacterCardView _pvpCharacterCardViewPrefab;
    [SerializeField] private SkillViewManager _skillView;
    [SerializeField] private Transform _pvpCharactersParent;

    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private ResourceManager _resourceManager;

    private List<BaseCharacerView> _characters = new List<BaseCharacerView>();

    private BaseCharacerView _currentCharacter;
    
    public void CreateCharactersCards()
    { 
        _characters = _temploaryInfo.CreatedCharacters;

        Extension.DestroyChilds(transform);

        for (int i = 0; i < _characters.Count; i++)
        {
            var character = _characters[i];
            var card = Instantiate(_characterCardViewPrefab, transform);
            character.SetBattleCard(card);
            _skillView.UpDateCharracter(character);
            card.SetCharacterInfo(character.characterData, false, null, (x, y, z) =>
            {
                if(IsReadyAndNotSelected(character))
                {                
                _currentCharacter = character;
                Unselect();
                _currentCharacter.Select();
                _skillView.UpDateCharracter(_currentCharacter);
                }
            }, _temploaryInfo, _resourceManager);
        }
    }

    public void CreateBossSlider()
    {
        _temploaryInfo.EnemiesCharacters[0].SetBattleCard(_bossSliderView);
        _bossSliderView.SetBossCharacterInfo(_temploaryInfo.EnemiesCharacters[0].characterData);
        _bossSliderView.FillSlider();
    }

    public void SetNextSliderColors()
    {
        _bossSliderView.ChangeSliderColors();
    }

    public void InitializeGroupProgressIcons(Dictionary<int, List<BaseCharacterModel>> selectedCharacters)
    {
        for (int i = 0; i < selectedCharacters.Count; i++)
        {
            if (i == selectedCharacters.Count - 1)
            {
                _groupProgressIcons[i].Initialize(selectedCharacters[i].Count, true);
            }
            else
            {
                _groupProgressIcons[i].Initialize(selectedCharacters[i].Count);
            }
        }
    }

    public void RefreshGroupProgress(int index)
    {
        _groupProgressIcons[index].Activate();
        if (index > 0)
            _groupProgressIcons[index - 1].Deactivate();
    }

    public void UpdateGroupSlider(int index)
    {
        _groupProgressIcons[index].ChangeSliderValue();
    }

    public void CreatePvPCharactersCard()
    {
        if (_temploaryInfo.CurrentMode.GameMode == GameMode.PvP)
        {
            var characters = _temploaryInfo.EnemiesCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                var card = Instantiate(_pvpCharacterCardViewPrefab, _pvpCharactersParent);
                characters[i].SetBattleCard(card);
                card.SetEnemyCharacterInfo(_temploaryInfo.FakeLeader, characters[i].characterData, _resourceManager);
            }
        }
    }

    private bool IsReadyAndNotSelected(BaseCharacerView character)
    {
        return character.BattleCharacterCard().IsActive && !character.IsSelected;
    }

    private void Unselect()
    {
        _characters.ForEach(x => x.Unselect());
    }  
}
