using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class DMUnlockLevelToggle : MonoBehaviour
{
    [Inject] private BattleCampInfo _battleCampInfo;
    [Inject] private PlayerData _playerData;

    [SerializeField] private Toggle _unlockToggle;


    private void OnEnable()
    {
        _unlockToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDisable()
    {
        _unlockToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            UnlockAllLevels();

            SceneManager.LoadScene("Authentication");
        }
    }

    private void UnlockAllLevels()
    {
        foreach (var stage in _battleCampInfo.Stages)
        {
            foreach (var level in stage.Levels)
            {
                _playerData.AddFinishedLevel(stage, level, 3);
            }
        }
    }
}
