using UnityEngine;

public class ShieldHammerTalentBehaviour : BaseSkillBehaviour
{
    public ShieldHammerTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory, decaleFactory)
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

        base.Use(target, selfCharacter);

        selfCharacter.LookAtTarget(target.transform);
        selfCharacter.ChangeAutoAtack(false);
        selfCharacter.NavMeshAgent.isStopped = true;

        selfCharacter.SetAnimation(AnimStates.TalentID, (int)Skill.Id);

        return true;
    }

    private bool IsTargetInRange(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        float distance = Vector3.Distance(target.transform.position, selfCharacter.transform.position);
        return distance <= Skill.DamageRange;
    }
}