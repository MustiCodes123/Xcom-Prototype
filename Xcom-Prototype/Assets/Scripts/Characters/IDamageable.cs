using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDamageable
{
    public Action OnDie { get; set; }
    public Team Team { get; }
    public bool TakeDamage(int damage, AttackType damageType, float accuracy = 100, Color color = new Color());
    public bool IsDead { get; }
    public BaseCharacerView CharacterView { get; }
    public Vector3 Position { get; }
}
