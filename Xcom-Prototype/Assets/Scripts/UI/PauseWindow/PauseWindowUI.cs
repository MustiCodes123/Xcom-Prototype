using Data.Resources.AddressableManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Signals;

public class PauseWindowUI : UIWindowView
{
    private TemploaryInfo _temporaryInfo;

    [Inject] private void Construct(TemploaryInfo temporary, ResourceManager resourceManager)
    {
        _temporaryInfo = temporary;
        _resourceManager = resourceManager;
    }

    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitToMenuButton;
    [SerializeField] private Button _pauseButton;

    private void Start()
    {
        _continueButton.onClick.AddListener(() =>
        {
            Hide();
            OnContinueButtonClick();
            
        });
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _exitToMenuButton.onClick.AddListener(OnToMenuClick);
    }

    private async void OnToMenuClick()
    {
        _temporaryInfo.FirstWinfow = WindowsEnum.None;
        _temporaryInfo.SelectedCharacters.Clear();
        _resourceManager.ShowLoadingScreen();
        
        await _resourceManager.LoadMainMenuSceneAsync();
    }


    private void OnContinueButtonClick()
    {
        _signalBus.Fire(new ChangeGameStateSignal(GameState.Gameplay));
        _pauseButton.interactable = true;
    }
    
    private void OnRestartButtonClick()
    {
        _resourceManager.ShowLoadingScreen();
        _resourceManager.LoadLevelAsync(_resourceManager.GetActiveSceneName());
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }
}
