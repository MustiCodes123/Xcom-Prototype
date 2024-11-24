using System;
using UnityEngine;
using Zenject;

public class UIBattleWindow : UIWindowView
{
    [SerializeField] private Transform _waveCardContainer;
    [SerializeField] private CombatSpawner _spawner;
    [SerializeField] private WaveCard[] _waveCards;

    [Inject] private TemploaryInfo _temploaryInfo;

    private WaveCard _curentWaveCard;

    private void Start()
    {
        if (_temploaryInfo.CurrentMode.GameMode != GameMode.Default) return;
        SetWaves();
    }

    public override void HideImmediate()
    {
        gameObject.SetActive(false);
    }

    public void UpdateWaveBar(int enemyCount)
    {
        if (_temploaryInfo.CurrentMode.GameMode != GameMode.Default) return;
        _curentWaveCard = _waveCards[_spawner.CurrentWave - 1];
        _curentWaveCard.SetSliderValue(enemyCount);

        if (_curentWaveCard.IsComplete())
        {
            _waveCards[_spawner.CurrentWave].SetActiveFrame();
        }
    }

    private void SetWaves()
    {
        foreach (WaveCard card in _waveCards)
        {
            card.gameObject.SetActive(false);
            
        }

        for (int i = 0; i < _temploaryInfo.LevelInfo.Waves.Length; i++)
        {
            _waveCards[i].gameObject.SetActive(true);
            _waveCards[i].SetSliderMaxValue(_temploaryInfo.LevelInfo.Waves[i].Enemie.Length);

            if (i == _temploaryInfo.LevelInfo.Waves.Length - 1)
            {
                _waveCards[i].ActiveSlider(false);
            }
        }
    }

    private void OnDisable()
    {
        foreach (WaveCard card in _waveCards)
        {
            card.ResetSliderValue();
        }
    }
}