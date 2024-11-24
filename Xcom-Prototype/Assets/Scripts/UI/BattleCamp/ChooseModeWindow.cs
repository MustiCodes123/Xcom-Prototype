using UnityEngine;
using Zenject;

public class ChooseModeWindow : UIWindowView
{
    [SerializeField] private ModeCard _modeCard;

    [SerializeField] private Transform _modesParent;

    [Inject] private UIWindowManager  _uIWindowManager;
    [Inject] private ModeInfo _modeInfo;
    [Inject] private TemploaryInfo _temploaryInfo;

    private void Start()
    {
        CreateModes();
    }
  
    override public void Show()
    {
        base.Show();
        // gameObject.SetActive(true);
    }

    override public void Hide()
    {
        base.Hide();
        // gameObject.SetActive(false);
    }

    private void CreateModes()
    {
        for (int i = 0; i < _modeInfo.Modes.Length; i++)
        {
            ModeCard card = Instantiate(_modeCard, _modesParent);

            card.SetupCard(_modeInfo.Modes[i]);
            card.Button.onClick.AddListener(() =>
            {
                _uIWindowManager.ShowWindow(card.Mode.Window);
                _temploaryInfo.CurrentMode = card.Mode;
            });
        }
    }
}
