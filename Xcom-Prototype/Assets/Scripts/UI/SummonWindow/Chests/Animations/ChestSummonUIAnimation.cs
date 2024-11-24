using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class CanvasData
{
    [field: SerializeField] public CanvasGroup CanvasGroupData { get; set; }
    [field: SerializeField] public List<RectTransform> TopCards { get; set; }
    [field: SerializeField] public List<RectTransform> BottomCards { get; set; }
    [field: SerializeField] public Vector3 TopPanelPosition { get; set; }
    [field: SerializeField] public Vector3 BottomPanelPosition { get; set; }
}

public class ChestSummonUIAnimation : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private List<RectTransform> _topCards;
    [SerializeField] private List<RectTransform> _bottomCards;

    [Header("UI Components")]
    [SerializeField] private SummonCardsView _topCardsView;
    [SerializeField] private SummonCardsView _bottomCardsView;
    [SerializeField] private Button _button;

    [Header("Animation Settings")]
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _cardMoveDuration = 1f;
    [SerializeField] private float _cardFinalAnimationDuration = 0.5f;
    [SerializeField] private float _delayBetweenCards = 0.1f;
    [SerializeField] private float _panelMoveDuration = 1f;

    private bool _isAnimating = false;
    private bool _skipAnimation = false;
    private Coroutine _animationCoroutine;
    private bool _animationCompleted = true;
    private int _clickCounter = 0;
    private Vector2 _initialTopPanelPosition;
    private Vector2 _initialBottomPanelPosition;
    private Vector2 _topPanelTargetPosition;
    private Vector2 _bottomPanelTargetPosition;

    private void Start()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    public void PlayAnimation()
    {
        _clickCounter = 0;

        ResetPanelsToInitialPosition();

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCompleted = false;
        _animationCoroutine = StartCoroutine(AnimationSequence());
    }

    private void ResetPanelsToInitialPosition()
    {
        float screenWidth = _canvasGroup.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.width;
        float offScreenX = screenWidth;

        _initialTopPanelPosition = new Vector2(offScreenX, _topCardsView.RectTransform.anchoredPosition.y);
        _initialBottomPanelPosition = new Vector2(offScreenX, _bottomCardsView.RectTransform.anchoredPosition.y);

        _topCardsView.RectTransform.anchoredPosition = _initialTopPanelPosition;
        _bottomCardsView.RectTransform.anchoredPosition = _initialBottomPanelPosition;

        _topPanelTargetPosition = new Vector2(0, _topCardsView.RectTransform.anchoredPosition.y);
        _bottomPanelTargetPosition = new Vector2(0, _bottomCardsView.RectTransform.anchoredPosition.y);
    }
    private IEnumerator AnimationSequence()
    {
        _isAnimating = true;
        _skipAnimation = false;

        yield return Fade();

        if (!_skipAnimation)
            yield return AnimateCardsAndPanels();

        if (!_skipAnimation)
            yield return PlayFinalCardAnimations();

        _isAnimating = false;
        _animationCoroutine = null;
        _animationCompleted = true;
    }

    private IEnumerator Fade()
    {
        _canvasGroup.gameObject.SetActive(true);
        Tween fadeTween = _canvasGroup.DOFade(1, _fadeDuration).SetEase(Ease.InOutSine);
        yield return fadeTween.WaitForCompletion();
    }

    private IEnumerator AnimateCardsAndPanels()
    {
        Sequence topSequence = DOTween.Sequence();
        Sequence bottomSequence = DOTween.Sequence();

        _topCardsView.RectTransform.anchoredPosition = _initialTopPanelPosition;
        _bottomCardsView.RectTransform.anchoredPosition = _initialBottomPanelPosition;

        topSequence.Append(_topCardsView.RectTransform.DOAnchorPos(_topPanelTargetPosition, _panelMoveDuration));
        bottomSequence.Append(_bottomCardsView.RectTransform.DOAnchorPos(_bottomPanelTargetPosition, _panelMoveDuration));

        AnimateCardSet(_topCardsView.Cards, topSequence);
        AnimateCardSet(_bottomCardsView.Cards, bottomSequence);

        Sequence fullSequence = DOTween.Sequence()
            .Append(topSequence)
            .Join(bottomSequence)
            .SetEase(Ease.OutQuad);

        yield return fullSequence.WaitForCompletion();
    }

    private void AnimateCardSet(List<SingleSummonCardView> cards, Sequence sequence)
    {
        foreach (var card in cards)
        {
            sequence.Join(card.Image.DOFade(1, _cardMoveDuration).From(0));
        }
    }

    private IEnumerator PlayFinalCardAnimations()
    {
        List<SingleSummonCardView> allCards = new List<SingleSummonCardView>();
        allCards.AddRange(_topCardsView.Cards);
        allCards.AddRange(_bottomCardsView.Cards);

        foreach (var card in allCards)
        {
            if (_skipAnimation) break;

            if (!card.isActiveAndEnabled)
                continue;

            Sequence cardSequence = DOTween.Sequence();
            cardSequence.Append(card.RectTransform.DOScale(1.2f, _cardFinalAnimationDuration / 2));
            cardSequence.Append(card.RectTransform.DOScale(1f, _cardFinalAnimationDuration / 2));
            cardSequence.Join(card.RectTransform.DORotate(new Vector3(0, 0, 360), _cardFinalAnimationDuration, RotateMode.FastBeyond360));

            yield return cardSequence.WaitForCompletion();

            if (!_skipAnimation)
                yield return new WaitForSeconds(_delayBetweenCards);
        }
    }

    private void OnButtonClick()
    {
        _clickCounter++;

        if (_clickCounter <= 1)
        {
            SkipAnimation();
        }
        else
        {
            ReturnToBaseView();
        }
    }

    private void SkipAnimation()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
        _skipAnimation = true;

        DOTween.CompleteAll();
        CompleteAnimation();
    }

    private void CompleteAnimation()
    {
        _canvasGroup.alpha = 1;
        _topCardsView.RectTransform.anchoredPosition = _topPanelTargetPosition;
        _bottomCardsView.RectTransform.anchoredPosition = _bottomPanelTargetPosition;

        foreach (var card in _topCardsView.Cards.Concat(_bottomCardsView.Cards))
        {
            card.RectTransform.localScale = Vector3.one;
            card.RectTransform.rotation = Quaternion.identity;
            card.Image.color = new Color(card.Image.color.r, card.Image.color.g, card.Image.color.b, 1);
        }

        _isAnimating = false;
        _animationCompleted = true;
    }

    public void ReturnToBaseView()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isAnimating && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            SkipAnimation();
        }
    }
}