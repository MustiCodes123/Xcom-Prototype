using System.Collections.Generic;

[System.Serializable]
public class BaseCustomData
{
    public int ID;
    public int Gold;
    public int Gems;
    public int Energy;
    public Dictionary<string, int> Skills;
    public Dictionary<string, int> Items;
    public Dictionary<string, int> Characters;
}
