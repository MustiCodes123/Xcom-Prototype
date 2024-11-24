using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgradePricesData", menuName = "Data/WeaponUpgradePricesData")]
public class WeaponUpgradePrices : ScriptableObject
{
    [SerializeField] private float[] _levelsKoefs = new float[GameConstants.MaxItemLevel];
    [SerializeField] private float[] _rarityKoefs = new float[Enum.GetValues(typeof(RareEnum)).Length];

    public uint CalculatePrice(BaseItem item)
    {
        int level = item.CurrentLevel;
        int rarity = (int)item.Rare;
        int stars = (int)item.Rare;

        return (uint)(_levelsKoefs[level] * level + _rarityKoefs[rarity] * stars);
    }
}
