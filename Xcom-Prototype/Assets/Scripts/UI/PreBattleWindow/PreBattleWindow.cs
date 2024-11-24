using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class PreBattleWindow : UIWindowView
{
    [SerializeField] private SmalCharacterCard smalCharacterCard;
    [SerializeField] private List<SmalCharacterCard> smalCharacterCards = new List<SmalCharacterCard>();
    [SerializeField] private Transform characterParent;

    [SerializeField] private List<SmalCharacterCard> selectedCharacterCards;

    [SerializeField] private Button startBattleButton;

    [SerializeField] private Transform[] characterPositions;

    [Inject] private CameraHolder cameraHolder;

    [Inject] private TemploaryInfo temploaryInfo;
    [Inject] private PvPBattleData _battleData;

    private bool isCharacterSelected;

    private void Start()
    {
        startBattleButton.onClick.AddListener(OnStartButtonClicked);
        foreach (var item in selectedCharacterCards)
        {
            item.SetCharacterData();
            item.SubscribeToClick((x) =>
            {
                OnHeroRemovedClick(x);
                item.SetCharacterData();
            });
        }
    }

    protected override void OnCloseClicked()
    {
        windowManager.ShowPreviousWindow();
        this.Hide();
    }

    private void OnHeroRemovedClick(BaseCharacterModel model)
    {
        isCharacterSelected = false;
        for (int i = 0; i < smalCharacterCards.Count; i++)
        {
            if (smalCharacterCards[i].baseCharacterInfo == model)
            {
                if(temploaryInfo.SelectedCharacters.Contains(model))
                {
                    temploaryInfo.SelectedCharacters.Remove(model);
                }
                RemoveCharacter(i);
                smalCharacterCards[i].gameObject.SetActive(true);
                break;
            }
        }

        for (int i = 0; i < smalCharacterCards.Count; i++)
        {
            if (smalCharacterCards[i].baseCharacterInfo != null)
            {
                isCharacterSelected = true;
            }
        }
    }

    private void OnStartButtonClicked()
    {
        Debug.LogError(temploaryInfo.CurrentMode.GameMode);
        if (!isCharacterSelected) return;
        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default)
        {
            if (CanUseEnergy(temploaryInfo.LevelInfo.EnergyCost))
            {
                if (!string.IsNullOrEmpty(temploaryInfo.LevelInfo.SceneName))
                {
                    SceneManager.LoadScene(temploaryInfo.LevelInfo.SceneName);
                }
                else
                {
                    SceneManager.LoadScene(1);
                }
            }
            else
            {
                InfoPopup.Instance.ShowTooltipNotEnpughtEnergy();
                InfoPopup.Instance.ActivateButtons("Go to the store", "Cancel", () =>
                { windowManager.ShowWindow(WindowsEnum.ShopWindow); }, null);
            }
        }
        else if (temploaryInfo.CurrentMode.GameMode == GameMode.PvP)
        {
            if (!CanUseKeys(_battleData.KeysCost)) return;
            playerData.SpendKey(temploaryInfo.CurrentBoss.KeysCost);
            SceneManager.LoadScene(_battleData.Scenes[Random.Range(0, _battleData.Scenes.Length)]);
        }
        else if (temploaryInfo.CurrentMode.GameMode == GameMode.ThreeToOne)
        {
            if (!CanUseKeys(_battleData.KeysCost)) return;
            playerData.SpendKey(temploaryInfo.CurrentBoss.KeysCost);
            SceneManager.LoadScene(temploaryInfo.CurrentBoss.SceneName);
        }
        else if(temploaryInfo.CurrentMode.GameMode == GameMode.TestOfStrenght)
        {
            if (!CanUseKeys(_battleData.KeysCost)) return;
            playerData.SpendKey(temploaryInfo.CurrentBoss.KeysCost);
            SceneManager.LoadScene(temploaryInfo.CurrentBoss.SceneName);
        }
    }

    private void SetupCharacters()
    {
        var characterList = playerData.PlayerGroup.GetCharactersFromNotAsignedGroup();
        for (int i = 0; i < characterList.Count; i++)
        {
            SmalCharacterCard smalCharacterCard = Instantiate(this.smalCharacterCard, characterParent);
            smalCharacterCard.SetCharacterData(characterList[i], false, _resourceManager);
            smalCharacterCard.SubscribeToClick((x) => {
                if (CanAddCharacter())
                {
                    OnCharacterSelected(x);
                    smalCharacterCard.gameObject.SetActive(false);
                }
            });

            smalCharacterCards.Add(smalCharacterCard);
        }
    }

    private bool CanAddCharacter()
    {
        if (temploaryInfo.CurrentMode.GameMode == GameMode.Default && temploaryInfo.SelectedCharacters.Count < GameConstants.CharacterLimit) return true;
        if (temploaryInfo.CurrentMode.GameMode == GameMode.PvP && temploaryInfo.SelectedCharacters.Count < GameConstants.PvPCharacterLimit) return true;
        if (temploaryInfo.CurrentMode.GameMode == GameMode.ThreeToOne && temploaryInfo.SelectedCharacters.Count < GameConstants.ThreeToOneCharacterLimit) return true;
        return false;
    }

    private void CreateCharacterGO(BaseCharacterModel model, int pos)
    {
        CharacterSlotsHolder prefab = _resourceManager.LoadForUISlotHolder(model.Name);
        var currentCharacter = Instantiate<CharacterSlotsHolder>(prefab, characterPositions[pos]);
        currentCharacter.SetupItems(model.EquipedItems, _resourceManager);
    }

    private void RemoveCharacter(int pos)
    {
        if (characterPositions[pos].childCount > 0)
        {
            Destroy(characterPositions[pos].GetChild(0).gameObject);
        }
    }

    private void OnCharacterSelected(BaseCharacterModel model)
    {
        for (int i = 0; i < selectedCharacterCards.Count; i++)
        {
            if (selectedCharacterCards[i].baseCharacterInfo == null)
            {
                isCharacterSelected = true;
                selectedCharacterCards[i].SetCharacterData(model, false, _resourceManager);

                temploaryInfo.SelectedCharacters.Add(model);

                CreateCharacterGO(model, i);
                break;
            }
        }
    }

    public override void Show()
    {
        base.Show();
        SetupCharacters();
        // gameObject.SetActive(true);
        //cameraHolder.MoveCameraToBattlePreStart(null);
        cameraHolder.MoveCameraToBattlePreStart(() => { gameObject.SetActive(true); });
    }

    public override void Hide()
    {
        base.Hide();
    
        cameraHolder.MoveCameraToTopDown(null);
    }

}
