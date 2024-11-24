using Zenject;

public class MoneyChangeHandler
{
    readonly SignalBus _signalBus;

    public MoneyChangeHandler(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void ChangeMoney(int money)
    {
        _signalBus.Fire(new MoneyChangeSignal() { Money = money });
    }

    public void BoughtPurchaseItem(PurchaseItem purchaseItem)
    {
        switch (purchaseItem.purchaseType)
        {
            case PurchaseType.Gem:
                _signalBus.Fire(new MoneyChangeSignal() { Gems = purchaseItem.ContainsCount });
                break;
            case PurchaseType.Energy:
                _signalBus.Fire(new MoneyChangeSignal() { Cristal = purchaseItem.ContainsCount });
                break;
            case PurchaseType.Gold:
                _signalBus.Fire(new MoneyChangeSignal() { Money = purchaseItem.ContainsCount });
                break;
            default:
                _signalBus.Fire(new MoneyChangeSignal() { Cristal = purchaseItem.ContainsCount });
                break;
        }
    }
}
