using TMPro;
using UnityEngine;
using Zenject;

public class WalletView : MonoBehaviour
{
    [SerializeField] protected TMP_Text EnergyTMP;
    [SerializeField] protected TMP_Text GoldTMP;
    [SerializeField] protected TMP_Text KeysTMP;
    [SerializeField] protected TMP_Text _gemsTMP;

    private Wallet _wallet;

    private void OnDisable()
    {
        Wallet.Instance.CurrencyChanged -= UpdateCurrencyDisplay;
        EnableAllTMPComponents(EnergyTMP, GoldTMP, KeysTMP, _gemsTMP);
    }

    private void OnEnable()
    {
        _wallet = Wallet.Instance;
        _wallet.CurrencyChanged += UpdateCurrencyDisplay;

        InitializeCurrencyDisplay();
    }

    public void UpdateCurrency(GameCurrencies currencyType, int amount)
    {
        _wallet.AddCachedCurrency(currencyType, amount);
    }

    public void DisplayCurrency(GameCurrencies currency)
    {
        switch (currency)
        {
            case GameCurrencies.Gold:
                GoldTMP.gameObject.SetActive(true);
                break;
            case GameCurrencies.Gem:
                _gemsTMP.gameObject.SetActive(true);
                break;
            case GameCurrencies.Energy:
                EnergyTMP.gameObject.SetActive(true);
                break;
            case GameCurrencies.Key:
                KeysTMP.gameObject.SetActive(true);
                break;

            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    protected virtual void InitializeCurrencyDisplay()
    {
        long gemsAmount = Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Gem);
        long energyAmount = Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Energy);
        long goldAmount = Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Gold);
        long keysAmount = Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Key);

        UpdateCurrencyDisplay(goldAmount, GameCurrencies.Gold);
        UpdateCurrencyDisplay(keysAmount, GameCurrencies.Key);
        UpdateCurrencyDisplay(gemsAmount, GameCurrencies.Gem);
        UpdateCurrencyDisplay(energyAmount, GameCurrencies.Energy);        
    }

    protected void UpdateCurrencyDisplay(long amount, GameCurrencies currencyType)
    {
        if (amount < 0) return;

        string formattedAmount = Extension.FormatNumber(amount);

        switch (currencyType)
        {
            case GameCurrencies.Gold:
                if (GoldTMP != null) GoldTMP.text = $"{formattedAmount}";
                break;
            case GameCurrencies.Gem:
                if (_gemsTMP != null) _gemsTMP.text = $"{formattedAmount}";
                break;
            case GameCurrencies.Energy:
                if (EnergyTMP != null) EnergyTMP.text = $"{formattedAmount} / 60";
                break;   
            case GameCurrencies.Key:
                if (KeysTMP != null) KeysTMP.text = $"{formattedAmount} / 60";
                break;
        }
    }

    public void DisableTMPComponents(params TMP_Text[] components)
    {
        foreach (var component in components)
        {
            if (component != null) component.gameObject.SetActive(false);
        }
    }

    public void DisableTMPComponents()
    {
        DisableTMPComponents(KeysTMP, GoldTMP, _gemsTMP, EnergyTMP);
    }


    public void EnableAllTMPComponents(params TMP_Text[] components)
    {
        foreach (var component in components)
        {
            if (component != null) component.gameObject.SetActive(true);
        }
    }
}
