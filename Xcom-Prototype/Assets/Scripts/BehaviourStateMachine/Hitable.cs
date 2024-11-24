public abstract class Hitable
{
    public virtual bool TakeDamage(int damage, float accuracy, AttackType damageType)
    {
        return false;
    }
}
