using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgradeSlotPriceModel", menuName = "Data/Resources/WeaponUpgrade")]
public class WeaponUpgradeSlotPriceModel : ScriptableObject
{
    public OnePurchasePrice[] Prices = new OnePurchasePrice[8];
    public Sprite[] CurrencySprites;

    public Sprite GetCurrencyByCurrencyType(GameCurrencies type)
    {
        var currencyTypeToSprite = new Dictionary<GameCurrencies, Sprite>()
        {
            {GameCurrencies.Gold, CurrencySprites[0]},
            {GameCurrencies.Gem, CurrencySprites[1]},
            {GameCurrencies.Energy, CurrencySprites[2]},
            {GameCurrencies.Key, CurrencySprites[3]},
        };

        if (currencyTypeToSprite.ContainsKey(type))
        {
            return currencyTypeToSprite[type];
        }
        return null;
    }
}


[Serializable]
public struct OnePurchasePrice
{
    public GameCurrencies Currency;
    public uint Price;
}