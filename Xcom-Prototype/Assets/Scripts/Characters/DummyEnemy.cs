using System;
using UnityEngine;
using Zenject;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    [Inject] private CharactersRegistry charactersRegistry;

    public BaseCharacerView CharacterView => null;
    public Team Team => _team;
    bool IDamageable.IsDead => false;
    public Vector3 Position => transform.position;

    public Action OnDie { get; set; }

    private Team _team = Team.Enemies;

    public bool TakeDamage(int damage, AttackType damageType, float accuracy, Color color)
    {
        Debug.Log("DummyEnemy: Damage " + damage + " / " + accuracy);
        return true;
    }

    private void Start()
    {
        charactersRegistry.AddCharacter(this);
    }
}
