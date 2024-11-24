using System;
using System.Collections.Generic;
using UniRx;

public class AttackersHandler 
{
    private UniRxDisposable _uniRxdisposable;

    public List<BaseCharacerView> Attackers = new List<BaseCharacerView>();

    private const int _attackTimeOffset = 2;

    public AttackersHandler (UniRxDisposable uniRxdisposable)
    {
        _uniRxdisposable = uniRxdisposable;
    }

    public void AddAttacker(BaseCharacerView attacker)
    {
        Attackers.Add(attacker);

        Observable.Timer(TimeSpan.FromSeconds(_attackTimeOffset)).Subscribe(_ =>
        {
            if (attacker != null)
            {
                Attackers.Remove(attacker);
            }
        }).AddTo(_uniRxdisposable.CombatCharacterTimerDisposable);
    }
}