public class WalletViewTestOfStrength : WalletView
{
    protected override void InitializeCurrencyDisplay()
    {
        long goldAmount = Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Gold);
        long keysAmount = Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Key);

        UpdateCurrencyDisplay(goldAmount, GameCurrencies.Gold);
        UpdateCurrencyDisplay(keysAmount, GameCurrencies.Key);
    
        EnableAllTMPComponents(KeysTMP);

        DisableTMPComponents(EnergyTMP, _gemsTMP, GoldTMP);
    }
}