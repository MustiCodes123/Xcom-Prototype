public interface ICharacterDisplayView
{
    public void ShowCharacters();
    public void CharacterRemoved();
}

public interface IStatsCharacterDisplayView : ICharacterDisplayView
{
    public void SetCharacterStats(BaseCharacterModel character);

}
