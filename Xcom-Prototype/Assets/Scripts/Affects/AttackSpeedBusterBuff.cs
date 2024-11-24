using System.Collections;
using UnityEngine;

public class AttackSpeedBusterBuff : BaseBuff, IBuff
{
    public float SpeedMultiply = 2f;

    private Animator _animator;
    private float _originSpeed;
    private Coroutine _attackCoroutine;
    private static readonly int Atack = Animator.StringToHash("Atack");

    public AttackSpeedBusterBuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        buffType = BuffsEnum.AttackSpeedBuster;
        _damage = skill.BuffPreiodDamage;
        _skill = skill;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _animator = target.Animator;
        _originSpeed = target.characterData.AdditionalAttackSpeed;
        target.characterData.AdditionalAttackSpeed = -_skill.Value;
        //_attackCoroutine = Owner.StartCoroutine(AttackCoroutine());
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        target.characterData.AdditionalAttackSpeed = _originSpeed;

        /*if (_attackCoroutine != null)
        {
            Owner.StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }*/
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / (_originSpeed * SpeedMultiply));
           
            _animator.SetTrigger(Atack);
        }
    }
}