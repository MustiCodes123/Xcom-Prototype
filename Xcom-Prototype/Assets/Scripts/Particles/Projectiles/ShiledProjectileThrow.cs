using System;
using DG.Tweening;
using UnityEngine;

public class ShieldProjectileThrow : BaseProjectile
{
    private Vector3 _initialPosition;
    private Vector3 _startPosition;
    private Vector3 _target;
    private bool _returning = false;
    private Transform _shieldItem;
    private ItemSlot _shieldSlot;
    private const float MaxTravelDistance = 5f;
    private Tween _rotationTween;
    
    public override void Setup(BaseCharacerView target, BaseCharacerView creator, BaseParticleView particle,
        BaseSkillModel skill, Action onHit, BaseDecale decale = null, BuffsEnum buff = 0)
    {
        base.Setup(target, creator, particle, skill, onHit, decale, buff);
        _shieldSlot = Creator.SlotsHolder.GetSlot(SlotEnum.Shield);
        _startPosition = transform.position;
        _target = creator.AutoBattle ? target.transform.position : Decale.Target.position;

        _initialPosition = _shieldSlot.ItemPlaceholder.position;
    }

    public override void SkillUpdate()
    {
        if (_returning)
        {
            UpdateInitialPosition();
            ReturnToHand();
        }
        else if (IsActive)
        {
            HandleShieldMovement();
        }
    }

    private void UpdateInitialPosition() => _initialPosition = _shieldSlot.ItemPlaceholder.position;

    private void HandleShieldMovement()
    {
        var currentItem = _shieldSlot.GetCurrentItem();
        _shieldItem = currentItem.transform;
        
        MoveShield();
    }

    private void MoveShield()
    {
        StartShieldRotation();

        if (!(Vector3.Distance(transform.position, _target) > 0.1f)) return;
        transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);
        transform.LookAt(_target);

        if (Vector3.Distance(_startPosition, transform.position) >= MaxTravelDistance) 
            _returning = true;
    }

    private void StartShieldRotation()
    {
        _shieldItem.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        _rotationTween ??= _shieldItem.DORotate(new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void ReturnToHand() => transform.DOMove(_initialPosition, 0.5f).OnComplete(OnReturnComplete);

    private void OnReturnComplete()
    {
        _shieldItem.SetParent(_shieldSlot.ItemPlaceholder);
        _shieldItem.localPosition = Vector3.zero;
        _shieldItem.localRotation = Quaternion.identity;
        StopShieldRotation();
        
        _returning = false;
        IsActive = false;
    }

    private void StopShieldRotation()
    {
        _rotationTween?.Kill();
        _rotationTween = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<IEnemy>();
        var damageable = other.GetComponent<IDamageable>();

        if (enemy != null && damageable != null)
        {
            Enemy = damageable as BaseCharacerView;
            TargetView = Enemy;
            OnHit.Invoke();
        }
    }
}
