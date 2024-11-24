
public class CombatScore
{
    private TemploaryInfo _myTemploaryInfo;

    public CombatScore(TemploaryInfo temploaryInfo)
    {
        _myTemploaryInfo = temploaryInfo;
    }

    public int GetEnemiesOnLevelCount()
    {
        int enemies = 0;

        foreach (var wave in _myTemploaryInfo.LevelInfo.Waves)
        {
            foreach(var enemy in wave.Enemie)
            {
                enemies++;
            }
        }

        return enemies;
    }

    public int GetTotalReceivedDamage()
    {
        int totalReceivedDamage = 0;

        foreach (var character in _myTemploaryInfo.CreatedCharacters)
        {
            BaseCharacterModel characterData = character.characterData;
            totalReceivedDamage += characterData.Score.GetReceivedDamage();
        }

        return totalReceivedDamage;
    }

    public int GetTotalDealedDamage()
    {
        int totalReceivedDamage = 0;

        foreach (var character in _myTemploaryInfo.CreatedCharacters)
        {
            BaseCharacterModel characterData = character.characterData;
            totalReceivedDamage += characterData.Score.GetDealedDamage();
        }

        return totalReceivedDamage;
    }
}
