using UnityEngine;
using DG.Tweening;

public class UiEnemyIndicator : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float _displayDuration = 2.0f;
    [SerializeField] private Vector3 _pulsateScale = new Vector3(1.6f, 1.6f, 1.6f);
    
    public void ShowEnemyIndicator()
    {
        AnimateObject();
    }


    private void AnimateObject()
    {
        gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(gameObject.transform.DOScale(_pulsateScale, animationDuration).SetEase(Ease.InOutQuad))
            .Append(gameObject.transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.InOutQuad))
            .SetLoops(Mathf.RoundToInt(_displayDuration / (animationDuration * 2)), LoopType.Restart) 
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                
                gameObject.transform.localScale = Vector3.one;
            });
    }

}