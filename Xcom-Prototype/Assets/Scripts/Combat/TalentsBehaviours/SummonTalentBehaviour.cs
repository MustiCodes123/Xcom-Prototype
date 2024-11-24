using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonTalentBehaviour : BaseSkillBehaviour
{
    private List<BaseCharacerView> mCharacerView = new List<BaseCharacerView>();
    private int summonedLimit = 0;

    public SummonTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {
        summonedLimit = skill.Level;
        if(summonedLimit <= 0)
        {
            summonedLimit = 1;
        }
        //TODO create basick cooldown for battle start
        cooldown = 0.5f;
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        if(mCharacerView.Count >= summonedLimit)
        {
            return false;
        }

        var particle = _particleFactory.Create(Skill.ParticleType);
        particle.transform.position = selfCharacter.transform.position;

        BaseCharacterModel model = new BaseCharacterModel(1);
        switch (Skill.Element)
        {
            case ElementsEnum.Fire:
                model.Name = "FireElemental";
                break;
            case ElementsEnum.Water:
                model.Name = "WaterElemental";
                break;
            case ElementsEnum.Earth:
                model.Name = "EarthElemental";
                break;
            case ElementsEnum.Air:
                model.Name = "AirElemental";
                break;
            case ElementsEnum.Light:

                break;
            case ElementsEnum.Dark:
                break;
            case ElementsEnum.Nectomant:
                model.Name = "Sceleton";
                break;
            case ElementsEnum.Sumon:
                break;
            default:
                model.Name = "Skeleton";
                break;
        }

      


        var summoned = _charactersRegistry.GetCharacterFactory.Create(model);
        summoned.transform.position = selfCharacter.transform.position + new Vector3(Random.Range(0,2), 0, Random.Range(0,2));
        mCharacerView.Add(summoned);


        return base.Use(target, selfCharacter);
    }
}
