using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileThrow : BaseProjectile
{
    private Vector3 _initialPosition;
    private Vector3 _startPosition;
    private Vector3 _target;
    private bool _returning = false;
    private Transform _weaponItem;
    private ItemSlot _weaponSlot;
    private const float MaxTravelDistance = 5f;
    private float _rotationSpeed = 800f;

    public override void Setup(BaseCharacerView target, BaseCharacerView creator, BaseParticleView particle,
        BaseSkillModel skill, Action onHit, BaseDecale decale = null, BuffsEnum buff = 0)
    {
        base.Setup(target, creator, particle, skill, onHit, decale, buff);

        if (creator.AutoBattle || creator.IsBot)
        {
            _target = target.transform.position;
        }
        else
        {
            _target = Decale.Target.position;
        }

        _weaponSlot = Creator.SlotsHolder.GetSlot(skill.GetWeaponSlotBySkillWeapon(skill.WeaponSkill));
        _startPosition = transform.position;

        _weaponItem = _weaponSlot.GetItemSlotTransform();
        _weaponItem.localRotation = Quaternion.Euler(0f, 0f, 90f);
        _initialPosition = _weaponSlot.ItemPlaceholder.position;
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
            HandleMovement();
        }
    }

    private void UpdateInitialPosition() => _initialPosition = _weaponSlot.ItemPlaceholder.position;

    private void HandleMovement()
    {      
        Move();
    }

    private void Move()
    {
        StartRotation();

        if (!(Vector3.Distance(transform.position, _target) > 0.1f)) return;
        transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);
        transform.LookAt(_target);

        if (Vector3.Distance(_startPosition, transform.position) >= MaxTravelDistance)
        {
            _returning = true;
        }
    }

    private void StartRotation()
    {
        _weaponItem.Rotate(-Vector3.up * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void ReturnToHand()
    {
        transform.DOMove(_initialPosition, 0.5f).OnComplete(OnReturnComplete);
    }

    private void OnReturnComplete()
    {
        _weaponItem.SetParent(_weaponSlot.GetItemSlotTransform());
        _weaponItem.localPosition = Vector3.zero;
        _weaponItem.localRotation = Quaternion.identity;

        _returning = false;
        IsActive = false;
    }

    public override void OnDespawned()
    {
        if(!_returning)
        {
            ReturnToHand();
            _weaponItem.gameObject.SetActive(true);
        }

        base.OnDespawned();
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();

        if (damageable != null && damageable is BaseCharacerView targetCharacter && !Creator.IsMyTeammate(targetCharacter))
        {
            Enemy = damageable as BaseCharacerView;
            TargetView = Enemy;
            OnHit.Invoke();
        }
    }
}