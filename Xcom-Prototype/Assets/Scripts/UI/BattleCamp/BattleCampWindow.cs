using DG.Tweening.Core.Easing;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;


public class BattleCampWindow : UIWindowView
{

    [SerializeField] private StageCard stageCard;
    [SerializeField] private CampLevelCard levelCard;

    [SerializeField] private GameObject stages;
    [SerializeField] private GameObject levels;
    
    [SerializeField] private Transform stagesParent;
    [SerializeField] private Transform levelsParent;

    [SerializeField] private TextMeshProUGUI stageName;

    [SerializeField] private Button backButton;
    [SerializeField] private Image stageBackground;

    [SerializeField] private SmalCharacterCard characterCard;
    [SerializeField] private Transform characterCardParent;

    private List<SmalCharacterCard> charactersCards = new List<SmalCharacterCard>();
    private List<CampLevelCard> campLevelCards = new List<CampLevelCard>();

    [Inject] private BattleCampInfo battleCampInfo;
    [Inject] private TemploaryInfo temploaryInfo;
    [Inject] private UIWindowManager  uIWindowManager;
    [Inject] private CameraHolder cameraHolder;

    private void Start()
    {
        CreateStages();

        backButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnBackButtonClick()
    {
        stages.SetActive(true);
        levels.SetActive(false);
    }

  
    override public void Show()
    {
        transform.DOKill(true);
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(1, animationDuration);
        cameraHolder.MoveCameraToBattleMap(() => { gameObject.SetActive(true); });


    }

    override public void Hide()
    {
        transform.DOKill(true);
        transform.DOScale(0, animationDuration).OnComplete(() => { gameObject.SetActive(false); });
        cameraHolder.MoveCameraToTopDown(null);
    }

    public void CreateStages()
    {
        stages.SetActive(true);
        levels.SetActive(false);

        for (int i = 0; i < battleCampInfo.Stages.Length; i++)
        {
            StageCard stage = Instantiate(stageCard, stagesParent);

            bool previousStageFinished = false;
            if(i == 0)
            {
                previousStageFinished = true;
            }
            else
            {
                var prevStage = battleCampInfo.Stages[i - 1];
                var progress = playerData.GetCompanyProgres();

                if (progress.ContainsKey(prevStage.Name))
                {
                    var levelsProgress = progress[prevStage.Name];
                    if(levelsProgress.Count == prevStage.Levels.Length)
                    {
                        previousStageFinished = true;
                    }
                }
            }

            stage.SetStage(battleCampInfo.Stages[i], (x) => { SetupLevels(x, previousStageFinished);});
        }
    }

    private void SetupLevels(Stage stage, bool previousStageFinished)
    {
        temploaryInfo.CurrentStage = stage;
        stageBackground.sprite = stage.Sprite;
        stages.SetActive(false);
        levels.SetActive(true);

        stageName.text = stage.Name;

        var progress = playerData.GetCompanyProgres();
        List<int> levelsProgress = new List<int>();

        if (progress.ContainsKey(stage.Name))
        {
            levelsProgress = progress[stage.Name];
        }

        for (int i = 0; i < campLevelCards.Count; i++)
        {
            campLevelCards[i].gameObject.SetActive(false);
        }

        SetupCharacters(stage);

        bool isFirstUncompletedLevel = previousStageFinished;

        for (int i = 0; i < stage.Levels.Length; i++)
        {
            int stars = 0;
            if (levelsProgress.Count > i)
            {
                stars = levelsProgress[i];
            }

            if (campLevelCards.Count <= i)
            {
                CampLevelCard level = Instantiate(levelCard, levelsParent);
                level.SetLevel(stage.Levels[i], (x) => { StartBattle(x); }, stars, isFirstUncompletedLevel);
                campLevelCards.Add(level);
            }
            else
            {
                campLevelCards[i].SetLevel(stage.Levels[i], (x) => { StartBattle(x); }, stars, isFirstUncompletedLevel);
            }
           
            if(stars == 0)
            {
                isFirstUncompletedLevel = false;
            }
        }
    }

    private void SetupCharacters(Stage stage)
    {
        for (int i = 0; i < charactersCards.Count; i++)
        {
            charactersCards[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < stage.DropCharacters.Length; i++)
        {
            if (charactersCards.Count <= i)
            {
                SmalCharacterCard character = Instantiate(characterCard, characterCardParent);
                character.SetCharacterData(stage.DropCharacters[i].Character.CreateCharacter(), false, _resourceManager);
                charactersCards.Add(character);
            }
            else
            {
                charactersCards[i].SetCharacterData(stage.DropCharacters[i].Character.CreateCharacter(), false, _resourceManager);
            }
        }
    }


    private void StartBattle(CampLevel x)
    {
        temploaryInfo.LevelInfo = x;

        uIWindowManager.ShowWindow(WindowsEnum.PreBattleWindow);
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();
    }

}
