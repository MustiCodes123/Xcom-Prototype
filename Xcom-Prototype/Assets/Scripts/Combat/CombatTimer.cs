using System;
using Signals;
using TMPro;
using UnityEngine;
using Zenject;

public class CombatTimer : MonoBehaviour
{
    public TextMeshProUGUI BestTime;

    [SerializeField] private int delta;
    [SerializeField] private TextMeshProUGUI[] timerTMP;

    [Inject] private SignalBus _signalBus;
    private int _sec;
    private int _min;
    private bool _isActive = false;
    private float _timer = 0f;

    private void Start()
    {
        _signalBus.Subscribe<ChangeGameStateSignal>(OnChangeGameState);
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        _timer += Time.fixedDeltaTime;
        if (!(_timer >= 1f)) return;
        _timer = 0f;
        _sec += delta;

        if (_sec >= 60)
        {
            _min += _sec / 60;
            _sec %= 60;
        }

        SetAllTimers(_min.ToString("D2") + ":" + _sec.ToString("D2"));
    }

    public void Activate()
    {
        _timer = 0;
        _sec = 0;
        _min = 0;
        _isActive = true;
    }

    public void Deactivate()
    {
        SetAllTimers(_min.ToString("D2") + ":" + _sec.ToString("D2"));
        _isActive = false;
    }

    public int GetFullTime()
    {
        return _min * 60 + _sec;
    }

    public int GetTime()
    {
        return _sec;
    }

    public string TranslateToTimeText(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return minutes.ToString("D2") + ":" + remainingSeconds.ToString("D2");
    }

    public int TranslateToSeconds()
    {
        return _min * 60 + _sec;
    }

    private void SetAllTimers(string timeText)
    {
        foreach (var text in timerTMP)
        {
            text.text = timeText;
        }
    }

    private void OnChangeGameState(ChangeGameStateSignal signal)
    {
        if (signal.NewState == GameState.Pause)
        {
            _isActive = false;
        }
        else if (signal.NewState == GameState.Gameplay)
        {
            _isActive = true;
        }
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<ChangeGameStateSignal>(OnChangeGameState);
    }
}