using UnityEngine;
using DG.Tweening;
using System.Collections;

public class UiDestinationPoint : BaseDecale
{
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _displayDuration = 5f;
    [SerializeField] private float _disappearDuration = 0.5f;

    private bool isDisappearing = false;

    private void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        if (!isDisappearing && gameObject.activeSelf)
        {
            StartCoroutine(StartDisappearWithDelay());
        }
    }

    private IEnumerator StartDisappearWithDelay()
    {
        isDisappearing = true;

        yield return new WaitForSeconds(_displayDuration);

        transform.DOScale(Vector3.zero, _disappearDuration)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                isDisappearing = false;
            });
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        isDisappearing = false;
    }
}