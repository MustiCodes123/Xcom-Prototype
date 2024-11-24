using System.Collections.Generic;

public class UpgradeHeroesGoal : QuestGoal
{
    public int HeroCount = 1;
    public int RequiredLvlForEachHero;
    public RareEnum RequiredRare;
    public CharacterRace RequiredRace;

    private List<BaseCharacterModel> _usedCharacters;

    public override void Process(ISignal signal)
    {
        if (signal is UpgradeSignal upgradeHeroSignal && upgradeHeroSignal.IsCharacterUpgrade)
        {
            if (upgradeHeroSignal.Hero.Rare != RequiredRare)
                return;

            if (upgradeHeroSignal.IsCharacterUpgrade)
            {
                if (RequiredRace == CharacterRace.Dummy && HeroCount == 1)
                {
                    CurrentAmount++;
                }
                else if (RequiredRare == upgradeHeroSignal.Hero.Rare && CurrentAmount < upgradeHeroSignal.Hero.Level && HeroCount == 1)
                {
                    CurrentAmount = upgradeHeroSignal.Hero.Level;
                }
                else if (HeroCount > 1 && upgradeHeroSignal.PlayerData != null)
                {
                    CurrentAmount = GetCharactersMoreThanRequiredLvl(upgradeHeroSignal.PlayerData.PlayerGroup.GetCharactersFromNotAsignedGroup(), RequiredLvlForEachHero);
                }
                else if (HeroCount > 1 && RequiredRare == upgradeHeroSignal.Hero.Rare && RequiredLvlForEachHero < upgradeHeroSignal.Hero.Level && !IsCharacterUsed(upgradeHeroSignal.Hero))
                {
                    CurrentAmount++;
                }
            }
        }
    }

    private bool IsCharacterUsed(BaseCharacterModel modelTocheck)
    {
        foreach (BaseCharacterModel character in _usedCharacters)
        {
            if (character == modelTocheck)
            {
                return true;
            }
        }
        return false;
    }

    private int GetCharactersMoreThanRequiredLvl( List<BaseCharacterModel> characters, int requiredCharacterLvl )
    {
        int result = 0;
        foreach (var character in characters)
        {
            if (character.Level >= requiredCharacterLvl)
            {
                result++;
            }
        }
        return result;
    }
}