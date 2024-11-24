using DG.Tweening;
using Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CustomGameTimeScaleHandler : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Image pauseImage;
    [SerializeField] private Sprite pauseIcon;
    [SerializeField] private Sprite startIcon;
    [SerializeField] private Toggle AutoBattleButton;
    [SerializeField] private Toggle x2ToggleButton;

    [Inject] private TemploaryInfo temploaryInfo;
    [Inject] private SignalBus _signalBus;

    private int _originalTimeScale = 1;
    private int _doubleSpeed = 2;
    private ParticleSystem[] _particleSystems;

    public bool IsPaused { get; private set; }

    void Start()
    {
        BindPauseButton();
        Bindx2SpeedButon();

        AutoBattleButton.isOn = temploaryInfo.Autobattle;
        temploaryInfo.UpdateAutoBattle(AutoBattleButton.isOn);
        x2ToggleButton.onValueChanged.AddListener(temploaryInfo.OnToggleValueChanged);
        AutoBattleButton.onValueChanged.AddListener(x => { temploaryInfo.ChangeAutoBAttle(); });

        x2ToggleButton.isOn = temploaryInfo.IsGameSpeedDouble;
    }

  
    private void BindPauseButton()
    {
        pauseButton.onClick.AddListener(delegate 
        {
            if (IsPaused)
            {
                _signalBus.Fire(new ChangeGameStateSignal(GameState.Gameplay));
                DOTween.PlayAll();
                pauseImage.sprite = pauseIcon;
                IsPaused = false;
                _menuButton.interactable = true;
            }
            else
            {
                _signalBus.Fire(new ChangeGameStateSignal(GameState.Pause));
                DOTween.PauseAll();
                pauseImage.sprite = startIcon;
                IsPaused = true;
                _menuButton.interactable = false;
            }
        });
    }

    private void Bindx2SpeedButon()
    {
        x2ToggleButton.onValueChanged.RemoveAllListeners();
        
        x2ToggleButton.onValueChanged.AddListener(delegate
        {
            if (x2ToggleButton.isOn)
            {
                _signalBus.Fire(new ChangeGameSpeedSignal(_doubleSpeed));
                SpeedUpDOTWeenTimeScale();
            }
            else
            {
                _signalBus.Fire(new ChangeGameSpeedSignal(_originalTimeScale));
                RestoreDOTWeenTimeScale();
            }
        });
    }

    public void SpeedUpAllComponents(BaseCharacerView characterView)
    {
        characterView.NavMeshAgent.speed *= _doubleSpeed;
        DOTween.timeScale *= _doubleSpeed;
        _particleSystems = characterView.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in _particleSystems)
        {
            var main = ps.main;
            main.simulationSpeed *= _doubleSpeed;
        }
        characterView.Animator.speed *= _doubleSpeed;
    }

    public void RestoreAllComponents(BaseCharacerView characterView)
    {
        characterView.NavMeshAgent.speed = characterView.OriginalNavMeshSpeed;
        DOTween.timeScale = _originalTimeScale;
        _particleSystems = characterView.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in _particleSystems)
        {
            var main = ps.main;
            main.simulationSpeed = _originalTimeScale;
        }
        characterView.Animator.speed = characterView.OriginalAnimatorSpeed;
    }


    private void SpeedUpDOTWeenTimeScale() => DOTween.timeScale *= _doubleSpeed;

    private void RestoreDOTWeenTimeScale() => DOTween.timeScale = _originalTimeScale;

    private void OnDestroy()
    {
        if (temploaryInfo.CurrentMode.GameMode != GameMode.Default)
        {
            temploaryInfo.Autobattle = false;
            temploaryInfo.IsGameSpeedDouble = false;
        }
        
        temploaryInfo.BattleEnd();
        pauseButton.onClick.RemoveAllListeners();
        AutoBattleButton.onValueChanged.RemoveAllListeners();
        x2ToggleButton.onValueChanged.RemoveAllListeners();
    }
}



