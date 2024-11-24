using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterUpgradePricesData", menuName = "Data/CharacterUpgradePricesData")]
public class CharacterUpgradePrices : ScriptableObject
{
    [SerializeField] private float[] _levelsCoefs = new float[Enum.GetValues(typeof(RareEnum)).Length * 10];
    [SerializeField] private float[] _rarityCoefs = new float[Enum.GetValues(typeof(RareEnum)).Length];

    private uint Calculate(int level, int rarity, int stars)
    {
        return (uint)(_levelsCoefs[level] * level + _rarityCoefs[rarity] * stars);
    }

    public uint CalculatePrice(BaseCharacterModel character)
    {
        int level = character.Level;
        int rarity = (int)character.Rare;
        int stars = character.Stars;

        return Calculate(level, rarity, stars);
    }

    public uint CalculatePrice(BaseCharacterModel character, int upLevels)
    {
        uint price = 0;
        int level = character.Level;
        int rarity = (int)character.Rare;
        int stars = character.Stars;

        for (int i = 0; i < upLevels; i++)
        {
            price += Calculate(level, rarity, stars);
            level++;
        }
        return price;
    }
}
