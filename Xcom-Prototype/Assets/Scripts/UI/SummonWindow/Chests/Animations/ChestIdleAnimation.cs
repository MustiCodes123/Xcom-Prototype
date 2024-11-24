using UnityEngine;
using DG.Tweening;

public class ChestIdleAnimation : MonoBehaviour
{
    [field: SerializeField] public float ShakeStrength { get; private set; } = 3f;

    [SerializeField] private float _shakeDuration = 1.5f;
    [SerializeField] private int _shakeVibrato = 12;
    [SerializeField] private float _shakeRandomness = 50f;
    [SerializeField] private float _delayBetweenShakes = 1f;
    [SerializeField] private float _shakeIncreaseDelay = 1f;

    private Sequence _shakeSequence;

    private Vector3 _startLocalPosition;
    private Quaternion _startLocalRotation;
    private Vector3 _startLocalScale;
    private bool _isInitialized = false;

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            _startLocalPosition = transform.localPosition;
            _startLocalRotation = transform.localRotation;
            _startLocalScale = transform.localScale;
            _isInitialized = true;
        }

        StartIdleAnimation();
    }

    private void OnDisable()
    {
        ResetAnimations();
    }

    public void StopIdleAnimation()
    {
        if (_shakeSequence != null)
        {
            _shakeSequence.Kill();
            _shakeSequence = null;
        }
    }

    public void StartIdleAnimation()
    {
        _shakeSequence = DOTween.Sequence();
        _shakeSequence.Append(transform.DOShakeRotation(_shakeDuration, new Vector3(ShakeStrength, 0f, 0f), _shakeVibrato, 0f, false));
        _shakeSequence.AppendInterval(_delayBetweenShakes);
        _shakeSequence.SetLoops(-1, LoopType.Restart);
        _shakeSequence.Play();
    }

    private void UpdateIdleAnimation()
    {
        if (_shakeSequence != null && _shakeSequence.IsPlaying())
        {
            _shakeSequence.Kill();
        }

        _shakeSequence = DOTween.Sequence();
        _shakeSequence.Append(transform.DOShakeRotation(_shakeDuration, new Vector3(ShakeStrength, 0f, 0f), _shakeVibrato, 0f, false));
        _shakeSequence.AppendInterval(_delayBetweenShakes);
        _shakeSequence.SetLoops(-1, LoopType.Restart);
        _shakeSequence.Play();
    }

    public void ResetAnimations()
    {
        StopIdleAnimation();

        transform.localPosition = _startLocalPosition;
        transform.localRotation = _startLocalRotation;
        transform.localScale = _startLocalScale;

        ShakeStrength = 3f;
    }

    public Sequence VibrateChest(float duration, int strengthMultiplier)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOShakeRotation(duration, new Vector3(0f, ShakeStrength, ShakeStrength), (_shakeVibrato * strengthMultiplier), _shakeRandomness, false));
        sequence.OnUpdate(UpdateIdleAnimation);
        return sequence;
    }
}