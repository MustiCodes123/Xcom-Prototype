using System.Collections.Generic;
using UnityEngine;

public class SpriteToTextConvertor : MonoBehaviour
{
    private Dictionary<GameCurrencies, string> _currencyToSpriteID = new Dictionary<GameCurrencies, string>()
    {
        { GameCurrencies.Gem, "<sprite=0>" },
        { GameCurrencies.Gold, "<sprite=1>" },
        { GameCurrencies.Energy, "<sprite=2>" },
        { GameCurrencies.Key, "<sprite=3>" },
    };
    
    public string GetSpriteID(GameCurrencies currencie)
    {
        if (_currencyToSpriteID.ContainsKey(currencie))
        {
            return _currencyToSpriteID[currencie];
        }
        else
        {
            return "";
        }
    }
}
