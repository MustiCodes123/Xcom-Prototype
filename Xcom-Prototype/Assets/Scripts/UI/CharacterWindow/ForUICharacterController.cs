using DG.Tweening;
using UnityEngine;
using System;
using UniRx;

public class ForUICharacterController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private UniRxDisposable _uniRxDisposable;

    private const float _speed = 1.3f;
    private const float _moveDistance = 7;
    private const float _moveDelay = 2;
    private const string _animatorIsMove = "IsMove";

    private Tween _move;

    public void Init( UniRxDisposable disposable )
    {
        _uniRxDisposable = disposable;
    }

    public void MoveCharacterWithDelay()
    {
        _uniRxDisposable.UICharacterTimerDisposable.Clear();
        Observable.Timer(TimeSpan.FromSeconds(_moveDelay)).Subscribe(_ =>
        {
            MoveCharacter();
            _uniRxDisposable.UICharacterTimerDisposable.Clear();
        }).AddTo(_uniRxDisposable.UICharacterTimerDisposable);
    }

    private void MoveCharacter()
    {
        _animator.SetBool(_animatorIsMove, true);
        _move = transform.DOMove(transform.position + transform.forward * _moveDistance, _speed).OnComplete(() =>
        {
            StopCharacter();
        }); ;
    }

    private void StopCharacter()
    {
        DOTween.Kill(transform);
        _animator.SetBool(_animatorIsMove, false);
    }

    private void OnDestroy()
    {
        DOTween.Kill(_move);
    }
}
