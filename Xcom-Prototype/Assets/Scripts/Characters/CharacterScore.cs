
using UnityEngine.Purchasing;

public class CharacterScore
{
    private int _dmageDealed;
    private int _damageReceived;

    private BaseCharacterModel _owner;
    public CharacterScore(BaseCharacterModel owner)
    {
        _owner = owner;
    }
    public void ResetScore()
    {
        _damageReceived = 0;
        _dmageDealed = 0;
    }

    public void SetReceivedDamage(int damage)
    {
        _damageReceived += damage;
    }

    public int GetReceivedDamage()
    {
        _damageReceived = (_owner.GetMaxHP() - _owner.Health);

        return _damageReceived;
    }

    public void SetDealedDamage(int damage)
    {
        _dmageDealed += damage;
    }

    public int GetDealedDamage()
    {
        return _dmageDealed;
    }
}
