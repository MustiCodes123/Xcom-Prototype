using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RareToLevelUpModel", menuName = "Data/Characters/BossLevelUpModel")]
public class BossLevelUpModel : ScriptableObject
{
    public float Easy;
    public float Rare;
    public float Epic;
    public float Legendary;
    public float Mythical;

    public int AdditionalHealth;
    public int AdditionalMana;
    public int AdditionalAttack;
    public float AdditionalPhysicalResistance;
    public float AdditionalMagicalResistance;
    public int LimitPhysicalResistance;
    public int LimitMagicalResistance;
}