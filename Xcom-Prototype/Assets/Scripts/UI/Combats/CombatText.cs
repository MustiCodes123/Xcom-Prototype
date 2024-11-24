using DG.Tweening;
using UnityEngine;
using Zenject;

public class CombatText : MonoBehaviour, IPoolable<string, Transform, IMemoryPool>
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    private IMemoryPool p2;

    public void OnDespawned()
    {
        transform.SetParent(null);
    }

    public void SetColor(Color color)
    {
        if (color == new Color())
        {
            color = Color.white;
        }

        text.color = color;
    }

    public void ChangeRandomPosition()
    {
        transform.position += new Vector3(Random.Range(-0.75f, 0.75f), Random.Range(-2.25f, 0.75f), 0);
    }

    public void DoTransformScale(float targetScaleFrom, float targetScaleTo, float duration = 1f)
    {
        transform.DOScale(Vector3.one*targetScaleFrom, 0.1f).From(Vector3.zero).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOScale(Vector3.one*targetScaleTo, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                text.DOColor(Color.clear, duration).SetEase(Ease.Flash);
            });
                transform.DOLocalMoveY(500, duration).SetEase(Ease.Flash).OnComplete(() =>
                {
                    p2.Despawn(this);
                });
        });
    }

    public void OnSpawned(string p1, Transform parent, IMemoryPool p2)
    {
        DOTween.Kill(transform);
        DOTween.Kill(text);
        this.p2 = p2;
        text.color = Color.white;
        text.text = p1.ToString();
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
        DoTransformScale(1.75f, 0.875f);
    }

    public class Factory : PlaceholderFactory<string, Transform, CombatText>
    {

    }

    private void OnDestroy()
    {
        //DOTween.Kill(transform);
    }
}
