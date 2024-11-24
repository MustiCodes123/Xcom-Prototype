using Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseMenuButton : MonoBehaviour
{
    private UIWindowManager _uiWindowManager;
    private SignalBus _signalBus;
    [Inject] private void Construct(UIWindowManager uiWindowManager, SignalBus signalBus)
    {
        _uiWindowManager = uiWindowManager;
        _signalBus = signalBus;
    }

    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _pauseButton;

    private void Start()
    {
        _menuButton.onClick.AddListener(() =>
        {
            _uiWindowManager.ShowWindow(WindowsEnum.PauseMenuWindow);
            _signalBus.Fire(new ChangeGameStateSignal(GameState.Pause));
            _pauseButton.interactable = false;
        });
    }
}