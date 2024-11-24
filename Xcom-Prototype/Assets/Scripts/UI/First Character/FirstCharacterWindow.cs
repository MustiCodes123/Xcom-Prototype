using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FirstCharacterWindow : UIWindowView
{
    [SerializeField] Button selectButton;

    [SerializeField] private Transform parentForCaArds;
    [SerializeField] private CharacterCardView  caracterCardPrefab;

    [SerializeField] private List<CharacterCardView> characterCardViews;

    [Inject] private SkillsDataInfo skillsDataInfo;
    [Inject] private UIWindowManager uIWindowManager;

    private BaseCharacterModel currentCharacter;

    public void OnCharacterSelected (BaseCharacterModel model, CharacterCardView view, bool isTrue)
    {
        for (int i = 0; i < characterCardViews.Count; i++)
        {
            characterCardViews[i].SetSelected(false);
        }

        view.SetSelected(isTrue);
        currentCharacter = model;
        playerData.PlayerIconPath = model.AvatarId;
    }

    public void Start()
    {
        selectButton.onClick.AddListener(SelectButtonClick);

        if(characterCardViews != null && characterCardViews.Count > 0)
        {
            OnCharacterSelected(characterCardViews[0].CharacterModel, characterCardViews[0], false);
        }
    }

    private void SelectButtonClick()
    {
        playerData.PlayerGroup.AddCharacterToNotAsignedGroup(currentCharacter);
        uIWindowManager.HideAll();
    }

    private void SpawnCards()
    {
        for (int i = 0; i < skillsDataInfo.CharacterPresets.Length; i++)
        {
            if (skillsDataInfo.CharacterPresets[i].Rare <= RareEnum.Rare)
            {
                CharacterCardView card = Instantiate(caracterCardPrefab, parentForCaArds);
                card.SetCharacterInfo(skillsDataInfo.CharacterPresets[i].CreateCharacter(), false, null, OnCharacterSelected, null, _resourceManager);
                characterCardViews.Add(card);
            }
        }
    }

    override protected void OnDestroy()
    {
        selectButton.onClick.RemoveAllListeners();
    }


    override public void Show()
    {
        SpawnCards();
        gameObject.SetActive(true);
    }

    override public void Hide()
    {
        gameObject.SetActive(false);
    }

}
