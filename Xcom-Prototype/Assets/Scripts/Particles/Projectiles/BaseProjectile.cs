using System.Collections;
using System;
using UnityEngine;
using Zenject;
using Signals;

public class BaseProjectile : MonoBehaviour
{
    public bool IsActive { get; set; }

    public BaseCharacerView TargetView { get; set; }
    public ProjectileType projectileType;

    [SerializeField] protected float speed = 0.5f;
    [SerializeField] protected float damageRange = 1f;

    [Inject] private SignalBus _signalBus;

    protected Vector3 TargetPosition;
    protected Vector3 CharacterPosition;
    protected Collider[] Colliders;
    protected BaseCharacerView Enemy;
    protected BaseCharacerView Creator;
    protected BaseParticleView ParticleView;
    protected BaseDecale Decale;
    protected BaseSkillModel Skill;
    protected Action OnHit;

    protected float Distance;
    protected float Duration;
    protected int Damage;
    protected AttackType DamageType;

    private bool _isOnPlay = true;
    private IEnumerator _projectileDespawnCorutine;

    private void Update()
    {
        if (_isOnPlay)
        {
            SkillUpdate();
        }
    }

    public virtual void Setup(BaseCharacerView target, BaseCharacerView creator, BaseParticleView particle, BaseSkillModel skill, Action onHit, BaseDecale decale = null, BuffsEnum buff = 0)
    {
        IsActive = true;
        transform.position = creator.transform.position;
        TargetView = target;
        ParticleView = particle;
        Skill = skill;
        OnHit = onHit;
        Decale = decale;
        Duration = Skill.Duration;
        Creator = creator;
        _projectileDespawnCorutine = DespawnAfterDelay(Duration);
        StartCoroutine(_projectileDespawnCorutine);
    }

    public virtual void UpdatePosition()
    {
        if (TargetView != null)
        {
            TargetPosition = TargetView.transform.position;
        }
    }

    public virtual void OnSpawned()
    {
        gameObject.SetActive(true);

        //*** Make Bug Error***
        //_signalBus.Subscribe<ChangeGameStateSignal>(OnChangeGameState);

        OnSpawnAction();
    }
    public virtual void OnDespawned()
    {
        if (ParticleView != null)
        {
            ParticleView.OnDespawned();
        }

        _signalBus.TryUnsubscribe<ChangeGameStateSignal>(OnChangeGameState);

        OnDespawnedAction();
    }

    public bool IsEnoughtDistance()
    {
        Distance = Vector3.Distance(TargetPosition, transform.position);
        if (Vector3.Distance(TargetPosition, transform.position) < damageRange)
            return true;
        else
            return false;
    }

    public virtual void OnDamage()
    {
        IsActive = false;
        OnHit.Invoke();
        ParticleView.OnDespawned();
        this.OnDespawned();
    }

    public virtual void OnSpawnAction()
    {

    }

    public virtual void OnDespawnedAction()
    {
        gameObject.transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public void OverLapDamage()
    {
        Colliders = Physics.OverlapSphere(transform.position, damageRange);
        int damage = Skill.Value;

        if (Colliders[0] != null)
        {
            foreach (Collider collider in Colliders)
            {
                var enemy = collider.GetComponent<IEnemy>() as BaseCharacerView;
                if (enemy != null)
                {
                    IsActive = false;
                    ParticleView.OnDespawned();
                    enemy.TakeDamage(damage, AttackType.Physical, 100);
                }
            }
        }
    }

    public virtual void SkillUpdate()
    {
        UpdatePosition();

        if (IsActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
        }

        if (IsEnoughtDistance())
        {
            OnDamage();
        }
    }

    public BaseCharacerView GetTarget() => TargetView;

    private void OnChangeGameState()
    {
        if (_isOnPlay)
        {
            _isOnPlay = false;
            if (_projectileDespawnCorutine != null)
            {
                StopCoroutine(_projectileDespawnCorutine);
            }
        }
        else
        {
            _isOnPlay = true;
            if (_projectileDespawnCorutine != null)
            {
                StartCoroutine(_projectileDespawnCorutine);
            }
        }
    }

    protected IEnumerator DespawnAfterDelay(float delay)
    {
        int timeer = 0;
        while (timeer < delay)
        {
            yield return new WaitForSeconds(1f);
            timeer++;
        }
        OnDespawned();
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<ChangeGameStateSignal>(OnChangeGameState);
    }

    
    public class Factory : PlaceholderFactory<ProjectileType, BaseProjectile>
    {


    }
}