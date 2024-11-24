using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardDropText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _rewardText;

    private DropItemView _myViewer;
    public void OnEnable()
    {
        ShowText();
    }

    public void SetTetxAndColor(DropItemView viewer, string text, Color color)
    {
        _myViewer = viewer;
        SetColor(color);
        _rewardText.text = text;
    }

    private void SetColor(Color color)
    {
        if (color == new Color())
        {
            color = Color.red;
        }

        _rewardText.color = color;
    }

    public void ShowText()
    {
        transform.DOScale(Vector3.one * 10f, 1f).From(Vector3.zero).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 1f).SetEase(Ease.InOutBounce).OnComplete(() => {
                _rewardText.DOColor(Color.clear, 1f).SetEase(Ease.Flash);
                transform.DOLocalMoveY(500, 1).SetEase(Ease.Flash).OnComplete(() => {
                    Destroy(_myViewer.gameObject);
                });
            });

        });

    }
}
