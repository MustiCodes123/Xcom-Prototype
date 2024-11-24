using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeDecorations : MonoBehaviour
{
    [SerializeField] private Image[] _decorations;

    private const int _fadeSpeed = 6;

    public void ShowBlackDecorations()
    {
        foreach (var decoration in _decorations)
        {
            DOTween.Kill(decoration);
            decoration.DOFade(1, 0);
        }
    }

    public void HideBlackDecorations()
    {
        foreach (var decoration in _decorations)
        {
            DOTween.Kill(decoration);
            decoration.DOFade(0, _fadeSpeed);
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
