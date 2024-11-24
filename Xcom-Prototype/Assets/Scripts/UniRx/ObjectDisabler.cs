using UnityEngine;
using System;
using UniRx;

public class ObjectDisabler
{
    private IDisposable _disableSubscription;

    public void DisableAfterDelay(BaseCharacerView target, float delayInSeconds)
    {
        CancelPendingDisable();

        _disableSubscription = Observable
            .Timer(TimeSpan.FromSeconds(delayInSeconds))
            .Subscribe(_ => DisableObject(target));
    }

    public void CancelPendingDisable()
    {
        _disableSubscription?.Dispose();
    }

    private void DisableObject(BaseCharacerView target)
    {
        if (target != null)
        {
            target.gameObject.SetActive(false);

            if (target.OnDestroyAction != null)
            {
                target.OnDestroyAction.Invoke();
            }
        }
    }
}