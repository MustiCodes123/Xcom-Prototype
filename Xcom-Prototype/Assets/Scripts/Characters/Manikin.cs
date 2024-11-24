using System;
using UnityEngine;

public class Manikin : MonoBehaviour, IDamageable
{
    public Action OnDie { get; set; }

    private BaseParticleView _onDeathParticle;

    public BaseCharacerView CharacterView { get; set; }
    public Team Team { get; private set; }
    public Vector3 Position => transform.position;
    public bool IsDead { get; private set; }


    public void Setup(BaseCharacerView creator, BaseParticleView onCreateParticle, BaseParticleView onDeathParticle)
    {
        Team = creator.Team;
        CharacterView = creator;

        _onDeathParticle = onDeathParticle;

        BaseParticleView particle = Instantiate(onCreateParticle, transform.position, onCreateParticle.transform.rotation, transform);
    }

    public void Die()
    {
        BaseParticleView particle = Instantiate(_onDeathParticle, transform.position, _onDeathParticle.transform.rotation);

        gameObject.SetActive(false);
        CharacterView = null;

        OnDie?.Invoke();
    }

    public bool TakeDamage(int damage, AttackType damageType, float accuracy = 100, Color color = default)
    {
        return true;
    }
}