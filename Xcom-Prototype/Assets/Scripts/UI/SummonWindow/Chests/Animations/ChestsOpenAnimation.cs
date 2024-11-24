using UnityEngine;
using DG.Tweening;

public class ChestsOpenAnimation : MonoBehaviour
{
    [SerializeField] private Transform _lidTransform;

    [SerializeField] private Transform _insideParticles;
    [SerializeField] private Transform _volumetricLightsParticles;
    [SerializeField] private ChestSummonUIAnimation _chestSummonUIAnimation;

    [Tooltip("Delay before starting to open the lid after the shaking ends")]
    [SerializeField] private float _openDelay = 1f;

    [Tooltip("Duration of the lid opening animation")]
    [SerializeField] private float _openDuration = 1f;

    [Tooltip("Rotation angle of the lid when opened")]
    [SerializeField] private float _openAngle = 60f;

    [Tooltip("Delay before starting to increase the shaking strength")]
    [SerializeField] private float _shakeIncreaseDelay = 0.1f;

    [Tooltip("Duration of the shaking strength increase")]
    [SerializeField] private float _shakeIncreaseTime = 1f;

    [Tooltip("Duration of the chest squash")]
    [SerializeField] private float _squashDuration = 0.2f;

    [Tooltip("Squash scale of the chest")]
    [SerializeField] private Vector3 _squashScale = new Vector3(1f, 0.8f, 1f);

    [Tooltip("Vibration duration")]
    [SerializeField] private float _vibrationDuration = 1f;

    [Tooltip("Vibration strength multiplier")]
    [SerializeField] private int _vibrationStrengthMultiplier = 2;

    [Tooltip("Jump height of the chest")]
    [SerializeField] private float _jumpHeight = 0.2f;

    [Tooltip("Jump up duration")]
    [SerializeField] private float _jumpUpDuration = 0.25f;

    [Tooltip("Jump down duration")]
    [SerializeField] private float _jumpDownDuration = 0.15f;

    private Sequence _openSequence;
    private ChestIdleAnimation _chestIdleAnimation;

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

        if (!TryGetComponent(out _chestIdleAnimation))
            _chestIdleAnimation = GetComponent<ChestIdleAnimation>();
    }

    private void OnDisable()
    {
        ResetAnimations();
    }

    public void ResetAnimations()
    {
        _insideParticles.gameObject.SetActive(false);
        _volumetricLightsParticles.gameObject.SetActive(false);

        if (_openSequence != null && _openSequence.IsPlaying())
        {
            _openSequence.Kill();
        }

        transform.localPosition = _startLocalPosition;
        transform.localRotation = _startLocalRotation;
        transform.localScale = _startLocalScale;

        if (_lidTransform != null)
        {
            _lidTransform.localRotation = Quaternion.identity;
        }
    }

    public void OpenChest()
    {
        ResetAnimations();

        if (_chestIdleAnimation != null)
        {
            _volumetricLightsParticles.gameObject.SetActive(true);

            _chestIdleAnimation.StopIdleAnimation();

            _openSequence = DOTween.Sequence();

            _openSequence.Append(_chestIdleAnimation.VibrateChest(_vibrationDuration, _vibrationStrengthMultiplier));
            _openSequence.Join(transform.DOScale(_squashScale, _squashDuration));

            _openSequence.AppendCallback(() =>
            {
                Vector3 currentRotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(0f, currentRotation.y, _startLocalRotation.eulerAngles.z);

                _chestIdleAnimation.StopIdleAnimation();
                OpenLid();
            });

            _openSequence.Play();
        }
    }

    private void OpenLid()
    {
        Sequence openLidSequence = DOTween.Sequence();

        Vector3 jumpUpPosition = transform.localPosition + new Vector3(0f, _jumpHeight, 0f);
        openLidSequence.Append(transform.DOLocalMove(jumpUpPosition, _jumpUpDuration).SetEase(Ease.OutQuad));
        openLidSequence.Append(transform.DOLocalMove(_startLocalPosition, _jumpDownDuration).SetEase(Ease.InCubic));

        openLidSequence.Join(transform.DOScale(_startLocalScale, _openDuration));

        if (_lidTransform != null)
        {
            _volumetricLightsParticles.gameObject.SetActive(false);
            _insideParticles.gameObject.SetActive(true);

            openLidSequence.Join(_lidTransform.DORotate(new Vector3(0f, 0f, -_openAngle), _openDuration, RotateMode.LocalAxisAdd));
        }

        openLidSequence.OnComplete(() =>
        {
            _chestSummonUIAnimation.PlayAnimation();
        });

        _openSequence.Append(openLidSequence);
    }
}