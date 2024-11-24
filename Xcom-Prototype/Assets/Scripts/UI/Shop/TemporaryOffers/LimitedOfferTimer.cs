using DG.Tweening;
using PlayFab.EconomyModels;
using System;
using TMPro;
using UnityEngine;

public class LimitedOfferTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;

    private DateTime? _startTime;
    private DateTime? _endTime;
    private Tween _timerTween;

    public void Initialize(CatalogItem item)
    {
        Debug.Log($"ItemID {item.Id}");
        Debug.Log($"StartDate: {item.StartDate}, EndDate: {item.EndDate}");

        if (item.StartDate.HasValue && item.EndDate.HasValue)
        {
            _startTime = item.StartDate;
            _endTime = item.EndDate;
            StartTimer();
        }
        else
        {
            Debug.LogError("CatalogItem has invalid start or end date.");
        }
    }

    private void OnDestroy()
    {
        if (_timerTween.IsActive())
        {
            _timerTween.Kill();
        }
    }

    private void StartTimer()
    {
        _timerTween = DOVirtual.DelayedCall(1f, UpdateTimer).SetLoops(-1, LoopType.Restart);
    }

    private void UpdateTimer()
    {
        if (_startTime.HasValue && _endTime.HasValue)
        {
            var currentTime = DateTime.UtcNow;
            var timeLeft = _endTime.Value - currentTime;
            var timeStarted = currentTime - _startTime.Value;

            if (timeStarted.TotalSeconds < 0)
            {
                Debug.LogError("TotalSeconds can't be less than 0");
            }
            else if (timeLeft.TotalSeconds > 0)
            {
                _timerText.text = $"{timeLeft.Days} d {timeLeft.Hours} h left";
            }
            else
            {
                _timerText.text = "Offer expired";
                _timerTween.Kill();
            }
        }
    }
}
