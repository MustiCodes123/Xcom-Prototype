using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(LevelButtonView))]
public class LevelButton : MonoBehaviour
{
    public Action<LevelButton> Click;

    private Button _button;

    [field: SerializeField] public StartBattleButton StartBattleButton;
    [field: SerializeField] public LevelButtonView ButtonView { get; private set; }
    public CampLevel LevelData { get; private set; }
    public RewardData RewardData { get; private set; }
    public int ButtonOrder { get; set; }
    public bool IsUnlocked { get; private set; }

    public void Initialize(CampLevel levelData)
    {
        LevelData = levelData;
        RewardData = levelData.LevelRewards;

        ButtonView.Initialize(levelData);
        StartBattleButton.Initialize(levelData);
    }

    public void InitializeStars(int starsCount)
    {
        if (IsUnlocked)
        {
            ButtonView.DisplayStars(starsCount);
        }
    }

    public void SetupButtonState(List<int> levelStars, int index)
    {
        IsUnlocked = (index == 0 || (index <= levelStars.Count && levelStars[index - 1] > 0) || (index < levelStars.Count && levelStars[index] > 0));
        ButtonView.SetState(IsUnlocked ? new UnselectedState() : new LockedState());
    }

    private void OnEnable()
    {
        if(_button == null)
            _button = GetComponent<Button>();

        if(ButtonView == null)
            ButtonView = GetComponent<LevelButtonView>();

        _button.onClick.AddListener(() => Click?.Invoke(this));
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
