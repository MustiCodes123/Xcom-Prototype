using UnityEngine;
using UniRx;

public class ManikinTalentBehaviour : BaseSkillBehaviour
{
    private Manikin _manikin;

    public ManikinTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseDecale.Factory decaleFactory)
        : base(skill, particleFactory, charactersRegistry)
    {
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        CreateManikin(selfCharacter);
        return base.Use(target, selfCharacter);
    }

    private void CreateManikin(BaseCharacerView selfCharacter)
    {
        BaseDecale decale = CreateDecale(selfCharacter.transform);
        GameObject cloneMesh = selfCharacter.transform.Find("Mesh").gameObject;
        GameObject manikin = GameObject.Instantiate(cloneMesh, _decale.Target.position, selfCharacter.transform.rotation);
        _manikin = manikin.AddComponent<Manikin>();

        BaseParticleView onCreateParticle = _particleFactory.Create(Skill.ParticleType);
        BaseParticleView onDeathParticle = _particleFactory.Create(Skill.ParticleType);

        _manikin.Setup(selfCharacter, onCreateParticle, onDeathParticle);

        _charactersRegistry.AddCharacter(_manikin);
        decale.OnDispawned();

        foreach (IDamageable enemy in _charactersRegistry.GetAllTargetsFromAnotherTeam(Team.Allies))
        {
            BaseCharacerView characterView = enemy as BaseCharacerView;

            characterView.SetTarget(_manikin);
        }

        Observable.Timer(System.TimeSpan.FromSeconds(Skill.Duration))
            .Subscribe(_ =>
            {
                _manikin.Die();
                _charactersRegistry.RemoveCharacter(_manikin);
                foreach (IDamageable enemy in _charactersRegistry.GetAllTargetsFromAnotherTeam(Team.Allies))
                {
                    BaseCharacerView characterView = enemy as BaseCharacerView;
                    IDamageable newTarget = _charactersRegistry.GetClosestEnemy(Team.Allies, characterView.transform.position);
                    if (newTarget != null)
                    {
                        characterView.SetTarget(newTarget);
                    }
                }
            })
            .AddTo(_manikin);
    }
}