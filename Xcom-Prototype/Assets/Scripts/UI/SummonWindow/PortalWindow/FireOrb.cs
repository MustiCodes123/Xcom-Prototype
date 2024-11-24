using UnityEngine;
using DG.Tweening;

public class FireOrb : MonoBehaviour
{
    [SerializeField] private Vector2 TargetAnchoredPosition = new();
    [SerializeField] private Vector2 TargetLocalScale = new();
    [SerializeField] private float OneIterationMinimalDuration = 0.8f;
    [SerializeField] private float OneIterationMaximalDuration = 1.2f;

    private float OneIterationDuration;
    private RectTransform ThisRectTransform;
    private Vector2 OriginalAnchoredPosition;
    private Vector2 OriginalLocalScale;
    private bool IsItFirstEnabling = true;

    private void OnEnable()
    {
        if(IsItFirstEnabling)
        {
            OneIterationDuration = UnityEngine.Random.Range(OneIterationMinimalDuration, OneIterationMaximalDuration);
            ThisRectTransform = (RectTransform)transform;
            OriginalAnchoredPosition = ThisRectTransform.anchoredPosition;
            OriginalLocalScale = ThisRectTransform.localScale;
            PerformOneIteration();
            IsItFirstEnabling = false;
        }
    }

    private void PerformOneIteration()
    {
        ThisRectTransform.anchoredPosition = OriginalAnchoredPosition;
        ThisRectTransform.localScale = OriginalLocalScale;
        Sequence Sequence = DOTween.Sequence();
        Sequence.Append(ThisRectTransform.DOAnchorPos(TargetAnchoredPosition, OneIterationDuration))
                .Join(ThisRectTransform.DOScale(TargetLocalScale, OneIterationDuration))
                .OnComplete(PerformOneIteration);
    }
}