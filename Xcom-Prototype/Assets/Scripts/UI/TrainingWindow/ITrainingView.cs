using System;

public interface ITrainingView : IStatsCharacterDisplayView, IAnimatable
{
    public void Initialize();
    public void SetupXPBar(BaseCharacterModel character, BaseCharacterModel? newCharacter=null);
    public void UpdatePrice(uint price); 
}

public interface IAnimatable
{
    public void SetActiveUIWithoutAnim(bool value);
    public void SetActiveUIwithAnim(bool value);
}