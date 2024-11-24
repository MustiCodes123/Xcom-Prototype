using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HornAnimator : MonoBehaviour
{
    [SerializeField] private WeaponUpgradeWindow window;
    [SerializeField] private LavaLine[] lines;
    [SerializeField] private Image[] linesFillRect;
    [SerializeField] private float duration;

    [Header("Animation of weapon image after upgrading")]
    [SerializeField] private RectTransform _currentWeaponSlot;
    [SerializeField] private List<Vector2> _anchoredPositionOfCurrentWeaponSlotOffsets = new();
    [SerializeField] private float _oneDisplacingDuration = 0.5f;
    [SerializeField] private GameObject _fireOrbs;
    [SerializeField] private Vector3 _smallerScale = new(0.8f, 0.8f, 0.8f);
    [SerializeField] private Vector3 _biggerScale = new(1.5f, 1.5f, 1.5f);
    [SerializeField] private float _transitionToSmallerScaleDuration = 0.3f;
    [SerializeField] private float _transitionToBiggerScaleDuration = 0.6f;
    [SerializeField] private Image _flashLight;
    [SerializeField] private float _flashLightTargetTransparency = 0.184f;
    [SerializeField] private Image _failImage;
    [SerializeField] private float _failImageTargetTransparency = 0.1f;
    [SerializeField] private float _failImageFadingDuration = 1;
    [SerializeField] private Image _successImage;
    [SerializeField] private float _successImageTargetTransparency = 0.035f;
    [SerializeField] private float _successImageFadingDuration = 1;
    [SerializeField] private Sprite _startRuinSign;
    [SerializeField] private Sprite _finishRuinSign;
    [SerializeField] private float _ruinSignsFadingDuration = 0.5f;
    [SerializeField] private string tagToDisable = "cancel";

    private Tweener _currentWeaponSlotTweener;
    private Tweener _flashLightTweener;
    private Tweener _failImageTweener;
    private Tweener _successImageTweener;

    public bool IsPlayingAnimationOfWeaponImageAfterUpgrading { get; private set; }
    public bool ShouldShowSuccessAnimation { get; set; } = false;
    public Dictionary<Transform, ImageForFailAnimation> EmptyImagesForFailAnimation { get; private set; } = new();
    public List<SpriteAndParentOfWeaponImageToBeShownInFailAnimation> SpritesDataForFailAnimation { get; private set; } = new();

    private void Start()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            linesFillRect[i] = lines[i].GetComponent<Image>();
        }
    }

    public void ActivateHorn()
    {
        StartCoroutine(HornCorutine());
    }

    public void ActivateLines(int count)
    {
        for (int i = 0; i < count; i++)
        {
            lines[i].isActivated = true;
        }
    }

    private void AnimateAllLines()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].isActivated)
            {
                linesFillRect[i].fillAmount += 0.01f;
            }
        }
    }

    private void RefreshHorn()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            linesFillRect[i].fillAmount = 0;
            lines[i].isActivated = false;
        }

    }

    private IEnumerator HornCorutine()
    {
        window.UpgradeButtonGameObject.SetActive(false);
        GameObject[] cancelButton = GameObject.FindGameObjectsWithTag(tagToDisable);

        foreach(GameObject obj in cancelButton)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        while (linesFillRect[0].fillAmount != 1)
        {
            AnimateAllLines();

            yield return new WaitForSeconds(0.01f);
        }

        window.TryToUpgradeItem();
        RefreshHorn();

        window.UpgradeButtonGameObject.SetActive(true);
    }

    public void ShowWeaponUpgradingAnimation()
    {
        if (_currentWeaponSlot == null || IsPlayingAnimationOfWeaponImageAfterUpgrading)
        {
            return;
        }

        IsPlayingAnimationOfWeaponImageAfterUpgrading = true;
        _fireOrbs.transform.SetAsLastSibling();
        _fireOrbs.SetActive(true);
        Vector2 AnchoredPositionOfCurrentWeaponSlot = _currentWeaponSlot.anchoredPosition;
        int IndexOfCurrentOffset = 0;
        DisplaceToCurrentOffset();

        void DisplaceToCurrentOffset()
        {
            Vector2 FinalPosition;
            TweenCallback AfterDisplacing;
            if (IndexOfCurrentOffset < _anchoredPositionOfCurrentWeaponSlotOffsets.Count)
            {
                FinalPosition = _currentWeaponSlot.anchoredPosition + _anchoredPositionOfCurrentWeaponSlotOffsets[IndexOfCurrentOffset];
                AfterDisplacing = () =>
                {
                    IndexOfCurrentOffset++;
                    DisplaceToCurrentOffset();
                };
            }
            else
            {
                FinalPosition = AnchoredPositionOfCurrentWeaponSlot;
                AfterDisplacing = () =>
                {
                    IsPlayingAnimationOfWeaponImageAfterUpgrading = false;
                    _fireOrbs.SetActive(false);
                    if (ShouldShowSuccessAnimation)
                    {
                        ShowSuccessUpgradingAnimation();
                    }
                    else
                    {
                        ShowFailUpgradingAnimation();
                    }
                };
            }
            _currentWeaponSlot.DOAnchorPos(FinalPosition, _oneDisplacingDuration).OnComplete(AfterDisplacing);
        }
    }

    public void ShowSuccessUpgradingAnimation()
    {
        _successImageTweener = _successImage.DOFade(_successImageTargetTransparency, _successImageFadingDuration);
        _flashLight.transform.SetAsLastSibling();
        _currentWeaponSlot.DOScale(_smallerScale, _transitionToSmallerScaleDuration).OnComplete(() =>
        {
            _currentWeaponSlotTweener = _currentWeaponSlot.DOScale(_biggerScale, _transitionToBiggerScaleDuration);
            _flashLightTweener = _flashLight.DOFade(_flashLightTargetTransparency, _transitionToBiggerScaleDuration);
        });
    }

    public void ShowFailUpgradingAnimation()
    {
        _failImageTweener = _failImage.DOFade(_failImageTargetTransparency, _failImageFadingDuration);
        for (int i = 0; i < SpritesDataForFailAnimation.Count; i++)
        {
            Sprite WeaponSprite = SpritesDataForFailAnimation[i].WeaponSprite;
            ImageForFailAnimation ImagesSet = EmptyImagesForFailAnimation[SpritesDataForFailAnimation[i].ImageParent];
            ImagesSet.ImageWithWeaponSprite.gameObject.SetActive(true);
            ImagesSet.ImageWithWeaponSprite.sprite = WeaponSprite;
            ImagesSet.ImageWithRuinSign.sprite = _startRuinSign;
            ImagesSet.ImageWithRuinSign.color = new(ImagesSet.ImageWithRuinSign.color.r, ImagesSet.ImageWithRuinSign.color.g, ImagesSet.ImageWithRuinSign.color.b, 0);
            ImagesSet.ImageWithRuinSign.DOFade(1, _ruinSignsFadingDuration).OnComplete(() =>
            {
                ImagesSet.ImageWithRuinSign.sprite = _finishRuinSign;
                ImagesSet.ImageWithRuinSign.DOFade(0, _ruinSignsFadingDuration).OnComplete(() =>
                {
                    ImagesSet.ImageWithWeaponSprite.gameObject.SetActive(false);
                });
            });
        }
    }

    public void RestoreParametersAfterAnimation()
    {
        _currentWeaponSlotTweener?.Kill();
        _flashLightTweener?.Kill();
        _failImageTweener?.Kill();
        _successImageTweener?.Kill();
        _flashLight.color = new(_flashLight.color.r, _flashLight.color.g, _flashLight.color.b, 0);
        _failImage.color = new(_failImage.color.r, _failImage.color.g, _failImage.color.b, 0);
        _successImage.color = new(_successImage.color.r, _successImage.color.g, _successImage.color.b, 0);
        _currentWeaponSlot.transform.localScale = Vector3.one;
    }
}
