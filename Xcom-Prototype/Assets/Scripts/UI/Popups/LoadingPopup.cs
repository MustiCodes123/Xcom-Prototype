using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;

public class LoadingPopup : MonoBehaviour
{
    public static LoadingPopup Instance;
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _loadingValueText;
    [SerializeField] private TextMeshProUGUI _loadingStatusText;

    private bool _isHiding = false;

    [Inject]
    private void Init()
    {
        Instance = this;
    }

    public void ShowLoadingScreen()
    {
        gameObject.SetActive(true);
        _loadingSlider.value = 0;
        _loadingValueText.text = "0%";
        _loadingStatusText.text = "Starting...";
        _isHiding = false;
    }

    public void UpdateLoadingProgress(float progress)
    {
        if (_isHiding) return;

        float targetValue = Mathf.Clamp01(progress);
        int percentage = Mathf.RoundToInt(targetValue * 100);

        DOTween.To(() => _loadingSlider.value, x => _loadingSlider.value = x, targetValue, 0.5f).OnUpdate(() => 
        {
            if (_loadingSlider.value >= 1f && !_isHiding)
            {
                _isHiding = true;
                HideLoadingScreen();
            }
        });

        DOTween.To(() => int.Parse(_loadingValueText.text.TrimEnd('%')), x => _loadingValueText.text = $"{x}%", percentage, 0.5f).SetEase(Ease.OutQuad);
    }

    public void HideLoadingScreen()
    {
        DOTween.To(() => _loadingSlider.value, x => _loadingSlider.value = x, 1f, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }

    private void OnDestroy()
    {
        Instance = null;
        DOTween.KillAll();
    }
}


