using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgroTalentBehaviour : BaseSkillBehaviour
{
    public AgroTalentBehaviour(BaseSkillModel skill, BaseParticleView.Factory particleFactory, CharactersRegistry charactersRegistry) : base(skill, particleFactory, charactersRegistry)
    {
    }

    public override bool Use(BaseCharacerView target, BaseCharacerView selfCharacter)
    {
        var targets = _charactersRegistry.GetAllTargetsFromAnotherTeam(selfCharacter.Team);

        if (targets.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            var view = targets[i].CharacterView;
            if (view != null)
            {
                var state = view.CurrentState;
                if (state != null)
                {
                    if (view != target )
                    {
                        state.SetNewTarget(selfCharacter);
                        Debug.LogError("Agro to "  + view.gameObject.name);
                        return base.Use(target, selfCharacter);
                    }
                }
            }
        }

        return false;
    }
}
