using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorPunishmentTalentBehaviour : BaseSkillBehaviour
{
    public WarriorPunishmentTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
    {
        _particleFactory = particleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        if (target == null || target.IsDead)
        {
            target = _charactersRegistry.GetClosestEnemy(Team.Allies, selfCharacter.transform.position) as BaseCharacerView;
        }

        if (!IsTargetInRange(target, selfCharacter) || target == null)
            return false;

        selfCharacter.ChangeAutoAtack(false);
        selfCharacter.NavMeshAgent.isStopped = true;

        selfCharacter.SetAnimation(AnimStates.TalentID, (int)Skill.Id);

        _particle = _particleFactory.Create(Skill.ParticleType);
        _particle.SetParent(selfCharacter.transform);
        var slot = selfCharacter.SlotsHolder;
        _particle.SetParent(slot.GetSlot(SlotEnum.Weapon).GetItemSlotTransform());
        _particle.transform.localPosition = new Vector3(_particle.transform.localPosition.x, _particle.transform.localPosition.y, _particle.transform.localPosition.z);
        selfCharacter.LookAtTarget(target.transform);
        int damage = selfCharacter.characterData.GetDamage();
        target.TakeDamage(damage, AttackType.Physical, selfCharacter.characterData.GetAccuracy());

        return base.Use(target, selfCharacter);
    }
}
