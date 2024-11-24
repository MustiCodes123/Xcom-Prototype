using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using System;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private RectTransform _loadingIcon;

    public async Task ShowLoadingScreenAsync(Func<Task> operation)
    {
        _loadingScreen.SetActive(true);
        _loadingIcon.DORotate(new Vector3(0, 0, -360), 2f, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        try
        {
            await operation();
        }
        finally
        {
            _loadingScreen.SetActive(false);
            _loadingIcon.DOKill();
        }
    }
}
