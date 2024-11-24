using System;
using Cysharp.Threading.Tasks;

namespace Data.Resources.AddressableManagement.Interfaces
{
    public interface IDataLoadingProgressTracker
    {
        void SetProgressValue(float value);
        float GetProgress();
        void SetLoadingCallbacks(Action<float> progressCallback);
        UniTask LoadWithProgress(Func<UniTask> assetOperation);
        UniTask SmoothProgressUpdate();
    }
}