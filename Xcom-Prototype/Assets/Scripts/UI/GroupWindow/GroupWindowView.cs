using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GroupWindowView : UIWindowView
{
    [Inject] private CharacterHandler characterHandler;

    [SerializeField] private Transform characterCardContainer;
    [SerializeField] private CharacterCardView characterCardView;
    [SerializeField] private List<CharacterCardView> allcharacterCardViews = new();
    [SerializeField] private TextMeshProUGUI charactersInTeamText;

    public override void Show()
    {
        base.Show();
      
        SetGroupInfo(playerData.PlayerGroup);
    }

    public void SetGroupInfo(BaseGroupInfo groupInfo)
    {
        foreach (var characterCardView in allcharacterCardViews)
        {
            Destroy(characterCardView.gameObject);
        }
        allcharacterCardViews.Clear();

        foreach (var character in groupInfo.GetCharactersFromBothGroup())
        {
            var newCharacterCard = Instantiate(characterCardView, characterCardContainer);
            bool isSelected = groupInfo.IsCharacterInGroup(character);
            newCharacterCard.SetCharacterInfo(character, isSelected, characterHandler, OnCharacterSelected, null, _resourceManager);
            newCharacterCard.gameObject.SetActive(true);
            allcharacterCardViews.Add(newCharacterCard);

        }
        characterCardView.gameObject.SetActive(false);
    }

    private void OnCharacterSelected(BaseCharacterModel characterInfo,CharacterCardView card , bool isSelected)
    {
        if(isSelected && playerData.PlayerGroup.GetCharactersFromBatleGroup().Length >= playerData.PlayerGroup.MaxGroupSize)
        {
            Debug.Log("Max group size");
            return;
        }
        card.SetSelected(isSelected);

        characterHandler.SetCharacterToGroup(characterInfo, isSelected ? GroupType.Battle : GroupType.None);
        SetTeamCountText();
    }

    private void SetTeamCountText()
    {
        charactersInTeamText.text = $"Fighters Select {playerData.PlayerGroup.GetCharactersFromBatleGroup().Length}/{playerData.PlayerGroup.MaxGroupSize}";
    }

}
