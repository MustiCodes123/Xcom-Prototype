using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PortalWindow : UIWindowView
{
    [SerializeField] private Button summonButton;
    [SerializeField] private SmalCharacterCard characterCard;

    [SerializeField] private Transform characterCardParent;

    [SerializeField] private CristalTooggle commonCristalCount;
    [SerializeField] private CristalTooggle rareCristalCount;
    [SerializeField] private CristalTooggle epicCristalCount;
    [SerializeField] private CristalTooggle legendaryCristalCount;

    [SerializeField] private PortalCristalHint cristalHint;

    private List<SmalCharacterCard> charactersCards = new List<SmalCharacterCard>();

    [Inject] private CameraHolder cameraHolder;
    [Inject] private ItemsDataInfo itemsData;
    [Inject] private SkillsDataInfo skillsData;
    [Inject] private SaveManager saveManager;

    private SummonCristalsEnum currentCristal;

    private void Start()
    {
        summonButton.onClick.AddListener(OnSummonButtonClick);

        SetupCristals();

        SelectCristal( SummonCristalsEnum.Common);
    }

    private void SetupCristals()
    {
        commonCristalCount.SetData(itemsData.CristalsInfo[0].Name, itemsData.CristalsInfo[0].Description, playerData.CommonSummonCristal.ToString(), itemsData.CristalsInfo[0].Sprite, SelectCristal);
        rareCristalCount.SetData(itemsData.CristalsInfo[1].Name, itemsData.CristalsInfo[1].Description, playerData.RareSummonCristal.ToString(), itemsData.CristalsInfo[1].Sprite, SelectCristal);
        epicCristalCount.SetData(itemsData.CristalsInfo[2].Name, itemsData.CristalsInfo[2].Description, playerData.EpicSummonCristal.ToString(), itemsData.CristalsInfo[2].Sprite, SelectCristal);
        legendaryCristalCount.SetData(itemsData.CristalsInfo[3].Name, itemsData.CristalsInfo[3].Description, playerData.LegendarySummonCristal.ToString(), itemsData.CristalsInfo[3].Sprite, SelectCristal);

        cristalHint.Refresh();
    }

    private void OnSummonButtonClick()
    {
        if (playerData.GetCristalCount(currentCristal) > 0)
        {
            playerData.RemoveCristal(currentCristal);
            SummonCharacter();
            SetupCristals();
            saveManager.SaveGame();
        }
        
    }

    private void SummonCharacter()
    {
       var cristalData = itemsData.CristalsInfo.FirstOrDefault(x => x.CristalEnum == currentCristal);

        var random = UnityEngine.Random.Range(0f, 1f);

        float currentChance = 0;

        for (int i = 0; i < cristalData.SummonChances.Length; i++)
        {
            currentChance += cristalData.SummonChances[i];

            if (random <= currentChance)
            {
                CharacterPreset character = null;
                List<CharacterPreset> characters = new List<CharacterPreset>();

                for (int x = 0; x < skillsData.CharacterPresets.Length; x++)
                {
                    var item = skillsData.CharacterPresets[x];
                     if (item.Rare == cristalData.SummonedEnums[i])
                    {
                        characters.Add(item);
                    }
                }
                if(characters.Count == 0)
                {
                    characters = skillsData.CharacterPresets.ToList();
                }

                character = characters[UnityEngine.Random.Range(0, characters.Count)];
              

                var characterData = character.CreateCharacter();

                SmalCharacterCard characterCard = null;
                
                // *** FOR SHOW CHARACTER CARD ***//
                /*if (charactersCards.Count == 0)
                {
                    characterCard = Instantiate(this.characterCard, characterCardParent);
                }
                else
                {
                    characterCard = charactersCards[0];
                }

                characterCard.SetCharacterData(characterData);
                charactersCards.Add(characterCard);*/
                playerData.PlayerGroup.AddCharacterToNotAsignedGroup(characterData);
                break;
            }
        }
    }

    public void SelectCristal(SummonCristalsEnum cristal)
    {
        commonCristalCount.SetSelected(false);
        rareCristalCount.SetSelected(false);
        epicCristalCount.SetSelected(false);
        legendaryCristalCount.SetSelected(false);

        currentCristal = cristal;

        switch (cristal)
        {
            case SummonCristalsEnum.Common:
                commonCristalCount.SetSelected(true);
                break;
            case SummonCristalsEnum.Rare:
                rareCristalCount.SetSelected(true);
                break;
            case SummonCristalsEnum.Epic:
                epicCristalCount.SetSelected(true);
                break;
            case SummonCristalsEnum.Legendary:
                legendaryCristalCount.SetSelected(true);
                break;
        }
    }

    public override void Show()
    {
        base.Show();
        cameraHolder.MoveCameraToPortal(() => { gameObject.SetActive(true); });
    }

    public override void Hide()
    {
        base.Hide();
        // gameObject.SetActive(false);
        cameraHolder.MoveCameraToTopDown(null);
    }

}
