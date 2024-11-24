using UnityEngine;

public class AssassinComboTalentBehaviour : BaseSkillBehaviour
{
    public AssassinComboTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory = null, BaseDecale.Factory decaleFactory = null) : base(skill, particleFactory, charactersRegistry, projectileFactory)
    {
        _projectileFactory = projectileFactory;
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

        selfCharacter.Target = target;
        selfCharacter.LookAtTarget(target.transform);

        return base.Use(target, selfCharacter);
    }

    private bool IsTargetInRange(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        if(target != null)
        {
            float distance = Vector3.Distance(target.transform.position, selfCharacter.transform.position);
            return distance <= Skill.DamageRange;
        }

        return false;
    }
}