using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class BaseShootProjectile : BaseProjectile
{
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private int _destroyTime = 3000;

    private bool _isNearTheTarget;

    public void Setup(BaseCharacerView creator, int damage, AttackType attackType, Transform parent)
    {
        _isNearTheTarget = false;
        IsActive = true;
        TargetView = creator.Target;
        Creator = creator;
        Damage = damage;
        DamageType = attackType;
        SetParent(parent);
        transform.rotation = Quaternion.identity;
        transform.SetParent(null);
    }

    public override void SkillUpdate()
    {
        if (_isNearTheTarget) return;

        UpdatePosition();
    }

    public override void UpdatePosition()
    {
        if (TargetView != null)
        {
            TargetPosition = TargetView.transform.position + (Vector3.up * TargetView.NavMeshAgent.height / 2);
            if (TargetView.IsDead && IsEnoughtDistance())
            {
                OnDespawned();
            }
        }
        else if (IsEnoughtDistance())
        {
            IsActive = false;
            OnDespawned();
        }

        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
    }

    public override async void OnDamage()
    {
        _isNearTheTarget = true;
        if (TargetView != null)
        {
            TargetView.TakeDamage(Damage, DamageType, Creator.characterData.GetAccuracy());
        }
        if (_hitParticles)
        {
            _hitParticles.Play();
        }
        await UniTask.Delay(_destroyTime);
        OnDespawned();
        IsActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HitBox>(out var hitBox))
        {
            if (hitBox.CharacterView != null && hitBox.CharacterView == TargetView && !TargetView.IsDead && _isNearTheTarget == false)
            {
                transform.parent = hitBox.transform;
                OnDamage();
            }
        }
    }

    public class Factory : PlaceholderFactory<RangeWeaponView, BaseShootProjectile> { }
}