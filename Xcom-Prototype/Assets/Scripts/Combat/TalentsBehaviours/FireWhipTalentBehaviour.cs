using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class FireWhipTalentBehaviour : BaseSkillBehaviour
{
    private int _attractionDelay = 700;
    private int _attractionSpeed = 1;
    private Vector3 _attractionPoint = new Vector3(0.5f,0,0);
    public FireWhipTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry, BaseProjectile.Factory projectileFactory, BaseDecale.Factory decaleFactory) : base(skill, particleFactory, charactersRegistry)
    {
        _projectileFactory = projectileFactory;
        _particleFactory = particleFactory;
        _decaleFactory = decaleFactory;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        _selfCharacter = selfCharacter;

        CreateParticleAndProjectile(target, selfCharacter);
        
        _projectile.Setup(target, selfCharacter, _particle, Skill, OnHit, _decale);

        var slot = selfCharacter.GetComponentInChildren<CharacterSlotsHolder>();
        _particle.SetParent(slot.GetSlot(SlotEnum.Weapon).GetItemSlotTransform());
        _particle.transform.localPosition = Vector3.zero;

        return base.Use(target, selfCharacter);
    }

    public void OnHit()
    {
        _target = _projectile.GetTarget();

        var stun = GetBuff(Skill.OnCollisionBuff);
        _target.SkillServiceProvider.AddBuff(stun);
        var fetters = CreateFetters(_target);
        
        fetters.duration = Skill.BuffDuration;
        int damage = Skill.Value;
        _target.TakeDamage(damage, Skill.DamageType);


        _target.transform.DOMove(_selfCharacter.transform.position + _attractionPoint, _attractionSpeed);

        /*if (_decale is PointDecale point && !_selfCharacter.AutoBattle)
        {
            //await UniTask.Delay(_attractionDelay);
            _target.transform.DOMove(point.AttractionPoint.transform.position, _attractionSpeed);
        }
        else
        {
            //await UniTask.Delay(_attractionDelay);
            
        }*/
    }

    private BaseParticleView CreateFetters(BaseCharacerView targeet)
    {
        var fetters = _particleFactory.Create(Skill.OnCollisionParticle);
        fetters.SetParent(targeet.transform);

        return fetters;
    }
}