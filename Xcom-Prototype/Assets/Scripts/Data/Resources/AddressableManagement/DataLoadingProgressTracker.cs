using System;
using Cysharp.Threading.Tasks;
using Data.Resources.AddressableManagement.Interfaces;
using UnityEngine;

public class DataLoadingProgressTracker : IDataLoadingProgressTracker
{
    private Action<float> _updateProgressCallback;
    private float _currentProgress;
    private float _targetProgress;
    private const float ProgressUpdateInterval = 0.05f;
    private const float SmoothingFactor = 0.1f;
    private const float MinProgressIncrement = 0.001f;
    private const float MaxProgress = 0.99f; 

    public void SetProgressValue(float value)
    {
        _targetProgress = Mathf.Clamp(value, 0f, MaxProgress);
        _updateProgressCallback?.Invoke(_currentProgress);
    }

    public float GetProgress() => _currentProgress;

    public void SetLoadingCallbacks(Action<float> progressCallback)
    {
        _updateProgressCallback = progressCallback;
    }

    public async UniTask LoadWithProgress(Func<UniTask> assetOperation)
    {
        float initialProgress = _currentProgress;
        float progressIncrement = 0.15f;
        SetProgressValue(initialProgress + progressIncrement);

        var loadTask = assetOperation();
        
        while (!loadTask.Status.IsCompleted())
        {
            await SmoothProgressUpdate();
            await UniTask.Delay(TimeSpan.FromSeconds(ProgressUpdateInterval));
            
            SetProgressValue(_targetProgress + MinProgressIncrement);
        }

        await loadTask;
        
        SetProgressValue(initialProgress + progressIncrement);
        await SmoothProgressUpdate();
    }

    public async UniTask SmoothProgressUpdate()
    {
        while (Mathf.Abs(_currentProgress - _targetProgress) > 0.001f)
        {
            _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, SmoothingFactor);
            _currentProgress = Mathf.Min(_currentProgress, _targetProgress);
            _updateProgressCallback?.Invoke(_currentProgress);
            await UniTask.Yield();
        }
    }
}