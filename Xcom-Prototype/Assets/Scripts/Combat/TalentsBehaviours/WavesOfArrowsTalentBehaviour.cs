using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class WavesOfArrowsTalentBehaviour : BaseSkillBehaviour
{
    private readonly int _arrowCountToSpawn = 5;
    private readonly float _spreadAngle = 45f;
    
    private GameObject _currentItem;
    private Transform _bowItem;
    private BaseProjectile[] _projectiles;

    public WavesOfArrowsTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _projectiles = new BaseProjectile[_arrowCountToSpawn];
        
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
        AnimationDelay = 0.2f;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        InitializeBow(selfCharacter);
        

        var halfSpread = _spreadAngle / 2;
        var angleStep = _spreadAngle / (_arrowCountToSpawn - 1);
        var startPoint = _bowItem.position;
        var initialRotation = _bowItem.localRotation;

        _bowItem.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.25f).OnComplete(() =>
        {
            SpawnArrows(target, selfCharacter, halfSpread, angleStep, startPoint);
            _bowItem.DOLocalRotate(initialRotation.eulerAngles, 0.1f);
        });

        return base.Use(target, selfCharacter);
    }

    private void InitializeBow(BaseCharacerView selfCharacter)
    {
        var bowSlot = selfCharacter.SlotsHolder.GetSlot(SlotEnum.Bow);
        _currentItem = bowSlot.GetCurrentItem();

        if (_currentItem == null)
        {
            _currentItem = selfCharacter.SlotsHolder.GetSlot(SlotEnum.Weapon).ItemPlaceholder.gameObject;
        }

        _bowItem = _currentItem.transform;
    }

    private void SpawnArrows(BaseCharacerView target, BaseCharacerView selfCharacter, float halfSpread, float angleStep, Vector3 startPoint)
    {
        var distanceBetweenArrows = 1f;

        for (int i = 0; i < _arrowCountToSpawn; i++)
        {
            InitializeProjectile(target, selfCharacter, i);

            var currentAngle = -halfSpread + angleStep * i;
            var forwardDirection = selfCharacter.transform.forward;
            var rotatedDirection = Quaternion.Euler(0, currentAngle, 0) * forwardDirection;
            var arrowStartPosition = CalculateArrowStartPosition(startPoint, rotatedDirection, distanceBetweenArrows, i);

            SetupProjectile(i, target, selfCharacter, rotatedDirection, arrowStartPosition, startPoint.y);
        }
    }

    private void InitializeProjectile(BaseCharacerView target, BaseCharacerView selfCharacter, int index)
    {
        _target = target;
        _selfCharacter = selfCharacter;
        _particle = _particleFactory.Create(Skill.ParticleType);
        _projectiles[index] = _projectileFactory.Create(Skill.ProjectileType);
    }

    private Vector3 CalculateArrowStartPosition(Vector3 startPoint, Vector3 rotatedDirection, float distanceBetweenArrows, int index)
    {
        return startPoint + rotatedDirection * distanceBetweenArrows * index + startPoint;
    }

    private void SetupProjectile(int index, BaseCharacerView target, BaseCharacerView selfCharacter, Vector3 rotatedDirection, Vector3 arrowStartPosition, float startY)
    {
        var projectile = _projectiles[index];

        if (projectile != null)
        {
            projectile.transform.position = arrowStartPosition;
            projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);
            _particle.SetParent(projectile.transform);
            projectile.transform.forward = rotatedDirection;
            projectile.transform.position = new Vector3(projectile.transform.position.x, startY, projectile.transform.position.z +0.5f);
        }
        else
        {
            Debug.LogError($"Projectile at index {index} is null.");
        }
    }

    public void OnHit()
    {
        foreach (var projectile in _projectiles)
        {
            if (projectile != null)
            {
                var target = projectile.GetTarget();
                if (target != null)
                {
                    target.TakeDamage(Skill.Value, AttackType.Physical);
                }
            }
        }
    }
}