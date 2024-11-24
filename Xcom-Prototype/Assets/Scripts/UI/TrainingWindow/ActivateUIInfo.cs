using DG.Tweening;
using UnityEngine;

public class ActivateUIInfo : MonoBehaviour
{
    [SerializeField] private GameObject[] _uiElements;
    [SerializeField] private GameObject _infoText;

    public void SetActiveUIWithoutAnim(bool value)
    {
        if (value)
        {
            foreach (var uiElement in _uiElements)
            {
                _infoText.SetActive(false);
                uiElement.SetActive(true);
                uiElement.transform.localScale = Vector3.one;
            }
        }
        else
        {
            foreach (var uiElement in _uiElements)
            {
                uiElement.SetActive(false);
                _infoText.SetActive(true);
            }
        }
    }

    public void SetActiveUIwithAnim(bool value)
    {
        if (value)
        {
            foreach (var uiElement in _uiElements)
            {
                _infoText.SetActive(false);
                uiElement.SetActive(true);
                uiElement.transform.localScale = Vector3.zero;
                uiElement.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        }
        else
        {
            foreach (var uiElement in _uiElements)
            {
                uiElement.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    uiElement.SetActive(false);
                    _infoText.SetActive(true);
                });
            }
        }
    }
    
    public void SetActiveUI(bool value)
    {
        foreach (var uiElement in _uiElements)
        {
            uiElement.SetActive(value);
        }

        _infoText.SetActive(!value);
    }
}