using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RareToLevelUpModel", menuName = "Data/ItemsData/RareToLevelUpModel")]
public class RareToLevelUpModel : ScriptableObject
{
    public int Default;
    public int Common;
    public int Uncommon;
    public int Rare;
    public int Epic;
    public int Legendary;
    public int Mythical;
}