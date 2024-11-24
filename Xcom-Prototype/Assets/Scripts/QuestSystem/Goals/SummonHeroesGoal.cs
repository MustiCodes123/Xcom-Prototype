using UnityEngine;

public class SummonHeroesGoal : QuestGoal
{
    public RareEnum CharacterRare;
    public CharacterRace CharacterRace;
    public bool IsAnyCharacter;

    public override void Process(ISignal signal)
    {
        if (IsAnyCharacter)
        {
            CurrentAmount++;
            Evaluate();

            return;
        }

        if (signal is SummonHeroSignal summonHeroSignal)
        {
            if ((summonHeroSignal.Hero.Rare == CharacterRare && CharacterRace == CharacterRace.Dummy) ||
                (CharacterRare == summonHeroSignal.Hero.Rare && CharacterRace == summonHeroSignal.Hero.Race))
            {
                CurrentAmount++;
                Evaluate();
            }
            else if (CharacterRare == RareEnum.Common && CharacterRace == CharacterRace.Dummy)
            {
                CurrentAmount++;
            }
        }
    }
}