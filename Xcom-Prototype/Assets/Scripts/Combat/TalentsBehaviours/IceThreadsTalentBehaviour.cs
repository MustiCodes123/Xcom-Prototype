using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceThreadsTalentBehaviour : BaseSkillBehaviour
{
    private List<GameObject> _iceThreads = new List<GameObject>();
    private float _explosionDelay = 5f;
    private float _explosionTimer;
    private float _explosionRadius = 5f;
    private List<Transform> _explosionTransforms = new List<Transform>();

    private float _radius = 3f;

    public IceThreadsTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        selfCharacter.SetAnimation(AnimStates.TalentID, (int)Skill.Id);

        List<Transform> icePoints = GetIcePointsOnDecal(_decale.Target.position, _radius, selfCharacter.Team);

        for (int i = 0; i < icePoints.Count; i++)
        {
            for (int j = i + 1; j < icePoints.Count; j++)
            {
                CreateIceThread(icePoints[i], icePoints[j]);
            }
        }

        _explosionTimer = _explosionDelay;

        return base.Use(target, selfCharacter);
    }

    private List<Transform> GetIcePointsOnDecal(Vector3 center, float radius, Team playerTeam)
    {
        List<Transform> icePoints = new List<Transform>();
        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider collider in colliders)
        {
            BaseCharacerView character = GetBaseCharacerViewFromParent(collider.transform);
            if (character != null && character.Team != playerTeam && !character.IsDead && HasColdDebuff(character))
            {
                if (!icePoints.Contains(character.transform))
                {
                    icePoints.Add(character.transform);
                }
            }
            else if (collider.CompareTag("IceSource"))
            {
                Transform iceSourceTransform = collider.transform;
                if (!icePoints.Contains(iceSourceTransform))
                {
                    icePoints.Add(iceSourceTransform);
                }
            }
        }

        return icePoints;
    }

    private bool HasColdDebuff(BaseCharacerView characterView)
    {
        return characterView.SkillServiceProvider.IsBuffOnMe(BuffsEnum.ColdDebuff);
    }

    private void CreateIceThread(Transform startPoint, Transform endPoint)
    {
        //GameObject threadPrefab = Resources.Load<GameObject>("Prefabs/Particles/IceThread");
        //GameObject threadObject = GameObject.Instantiate(threadPrefab);

        var threadObject = _particleFactory.Create(Skill.ParticleType);
        CableComponent cableComponent = threadObject.GetComponentInChildren<CableComponent>();
        cableComponent.transform.position = startPoint.position;
        cableComponent.EndPoint = endPoint;
        _iceThreads.Add(threadObject.gameObject);
        _explosionTransforms.Add(startPoint);
        _explosionTransforms.Add(endPoint);
    }

    private BaseCharacerView GetBaseCharacerViewFromParent(Transform transform)
    {
        if (transform == null)
            return null;

        BaseCharacerView characterView = transform.GetComponent<BaseCharacerView>();
        if (characterView != null)
        {
            return characterView;
        }

        return GetBaseCharacerViewFromParent(transform.parent);
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();
        UpdateExplosionTimer();
    }

    private void UpdateExplosionTimer()
    {
        _explosionTimer -= Time.deltaTime;

        if (_explosionTimer <= 0f)
        {
            ExplodeIceThreads();
            _explosionTimer = _explosionDelay;
        }
    }

    private void ExplodeIceThreads()
    {
        foreach (Transform explosionTransform in _explosionTransforms)
        {
            BaseParticleView explosion = _particleFactory.Create(ParticleType.Bang);
            explosion.transform.position = explosionTransform.position;

            Collider[] colliders = Physics.OverlapSphere(explosionTransform.position, _explosionRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out BaseCharacerView character))
                {
                    if (character.Team != Team.Allies && !character.IsDead)
                    {
                        character.TakeDamage(Skill.Value, AttackType.Magical);
                    }
                }
            }
        }

        foreach (var thread in _iceThreads)
        {
            GameObject.Destroy(thread);
        }

        _iceThreads.Clear();
        _explosionTransforms.Clear();
    }
}