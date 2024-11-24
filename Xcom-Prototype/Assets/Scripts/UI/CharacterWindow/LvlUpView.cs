using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class LvlUpView : MonoBehaviour
{
    [SerializeField] StatsInfoViewer statsInfoViewerPrefab;
    [SerializeField] Transform statsContainer;

    [SerializeField] private TextMeshProUGUI freePointsText;

    private List<StatsInfoViewer> statsInfoViewers = new List<StatsInfoViewer>();

    [Inject] private PlayerData playerData;
    [Inject] private CharacterHandler characterHandler;


    private void OnEnable()
    {
        SetupStats();
    }

    private void SetupStats()
    {
        var character = characterHandler.GetCurrentCharacterInfo();

        freePointsText.text = character.UnasignedStatPoints.ToString();

        for (int i = 0; i < character.Stats.Length; i++)
        {
            if (statsInfoViewers.Count > i)
            {
                statsInfoViewers[i].SetInfo(character.Stats[i], character.UnasignedStatPoints > 0, OnUpgradeClick);
            }
            else
            {
                var newViewer = Instantiate(statsInfoViewerPrefab, statsContainer);
                newViewer.SetInfo(character.Stats[i], character.UnasignedStatPoints > 0, OnUpgradeClick);
                statsInfoViewers.Add(newViewer);
            }
        }
    }

    private void OnUpgradeClick(StatsEnum stat)
    {
        characterHandler.GetCurrentCharacterInfo().AddPointToStat(stat);

        SetupStats();
    }


}
