using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BattleButtonViewConfig
{
    [field: SerializeField] public Sprite Frame { get; private set; }
    [field: SerializeField] public Sprite OrnamentLeft { get; private set; }
    [field: SerializeField] public Sprite OrnamentRight { get; private set; }
    [field: SerializeField] public Sprite TitleLine { get; private set; }
    [field: SerializeField] public Color TitleTMPColor { get; private set; }
    [field: SerializeField] public bool IsLocked { get; private set; }
}

public class BattleModeButtonView : MonoBehaviour
{
    public BattleButtonViewConfig SelectedConfig;
    public BattleButtonViewConfig UnselectedConfig;
    public BattleButtonViewConfig LockedConfig;

    [SerializeField] private GameObject _locked;
    [SerializeField] private TMP_Text _titleTMP;
    [SerializeField] private TMP_Text _lockedMessageTMP;
    [SerializeField] private Image _frame;
    [SerializeField] private Image _ornamentLeft;
    [SerializeField] private Image _ornamentRight;
    [SerializeField] private Image _titleLine;
    [SerializeField] private Image _foreground;

    [SerializeField] private GameObject[] _stars = new GameObject[3];

    private IModeButtonViewState _currentState;

    public void Initialize(Stage stage)
    {
        _titleTMP.text = stage.Name;
        _foreground.sprite = stage.Sprite;
    }

    public void ChangeView(BattleButtonViewConfig config)
    {
        _frame.sprite = config.Frame;
        _ornamentLeft.sprite = config.OrnamentLeft;
        _ornamentRight.sprite = config.OrnamentRight;
        _titleLine.sprite = config.TitleLine;
        _titleTMP.color = config.TitleTMPColor;
        _locked.SetActive(config.IsLocked);
    }


    public void SetUnselectedState()
    {
        SetState(new UnselectedStageButtonState());
    }

    public void SetSelectedState()
    {
        SetState(new SelectedStageButtonState());
    }

    public void SetLockedState(string message = "")
    {
        if(_lockedMessageTMP != null)
            _lockedMessageTMP.text = message;

        SetState(new LockedStageButtonState());
    }

    private void SetState(IModeButtonViewState newState)
    {
        _currentState = newState;
        _currentState.ApplyState(this);
    }
}