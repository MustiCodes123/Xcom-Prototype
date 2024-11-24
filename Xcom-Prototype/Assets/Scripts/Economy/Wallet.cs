using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class Wallet
{
    public Action<long, GameCurrencies> CurrencyChanged;

    [Inject] private SignalBus _signalBus;

    private bool _isPurchaseInProgress = false;
    private float _debounceTime = 2f;
    private CompositeDisposable _disposables = new CompositeDisposable();

    private static Wallet _instance;

    public static Wallet Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Wallet();
            }
            return _instance;
        }
    }

    public void Initialize(SignalBus signalBus)
    {
        _signalBus = signalBus;
        StartPeriodicSync();
    }

    #region Cached Currency Operations
    public long GetCachedCurrencyAmount(GameCurrencies currencyType)
    {
        return WalletStorage.GetCachedCurrency(currencyType);
    }

    public void AddCachedCurrency(GameCurrencies currencyType, int amount)
    {
        long currentAmount = WalletStorage.GetCachedCurrency(currencyType);
        long newAmount = currentAmount + amount;
        WalletStorage.SetCachedCurrency(currencyType, newAmount);
        CurrencyChanged?.Invoke(newAmount, currencyType);

        FireSignal(currencyType, amount, false);
    }

    public bool IsEnoughCachedCurrency(GameCurrencies currencyType, uint amount)
    {
        long currentAmount = WalletStorage.GetCachedCurrency(currencyType);
        return currentAmount >= amount;
    }

    public bool SpendCachedCurrency(GameCurrencies currencyType, uint amount)
    {
        long currentAmount = WalletStorage.GetCachedCurrency(currencyType);

        if (currentAmount < amount)
        {
            Debug.Log($"Not enough {currencyType}. SpendCachedCurrency method completed with result FALSE");
            return false;
        }

        long newAmount = currentAmount - amount;
        WalletStorage.SetCachedCurrency(currencyType, newAmount);
        CurrencyChanged?.Invoke(newAmount, currencyType);

        FireSignal(currencyType, (int)amount, true);

        return true;
    }
    #endregion

    #region Async Currency Operations
    public async Task<long> GetCurrencyAmountAsync(GameCurrencies currencyType)
    {
        long amount = await WalletStorage.LoadCurrency(currencyType);

        return amount;
    }

    public async Task AddCurrencyAsync(GameCurrencies currencyType, int amount)
    {
        await WalletStorage.AddCurrency(currencyType, amount);
        long newAmount = await WalletStorage.LoadCurrency(currencyType);
        CurrencyChanged?.Invoke(newAmount, currencyType);
    }

    public async Task<bool> SpendCurrencyAsync(GameCurrencies currencyType, uint amount)
    {
        if (_isPurchaseInProgress)
        {
            Debug.LogWarning("Purchase is already in progress. Inoring the request.");
            return false;
        }

        if (amount == 0)
            return true;

        try
        {
            _isPurchaseInProgress = true;

            long currentAmount = await WalletStorage.LoadCurrency(currencyType);

            if (currentAmount < amount)
            {
                Debug.LogError($"Not enough {currencyType}. SpendCurrencyAsync method completed with result FALSE");
                return false;
            }

            await WalletStorage.SubtractCurrency(currencyType, amount);
            long newAmount = await WalletStorage.LoadCurrency(currencyType);
            CurrencyChanged?.Invoke(newAmount, currencyType);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in SpendCurrencyAsync: {ex.Message}");
            return false;
        }
        finally
        {
            await Task.Delay(TimeSpan.FromSeconds(_debounceTime));
            _isPurchaseInProgress = false;
        }
    }
    #endregion

    #region Sync Operations
    public async Task SyncCurrencyWithServer(GameCurrencies currencyType, int retryCount = 0)
    {
        try
        {
            await WalletStorage.SyncCurrencyWithServer(currencyType);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("TooManyRequests") && retryCount < 3)
            {
                int delay = (int)Math.Pow(2, retryCount) * 1000;
                Debug.LogWarning($"Too many requests. Retrying in {delay} ms.");
                await Task.Delay(delay);
                await SyncCurrencyWithServer(currencyType, retryCount + 1);
            }
            else
            {
                Debug.LogError($"Error syncing currency: {ex.Message}");
            }
        }
    }

    private void StartPeriodicSync()
    {
        Observable.Interval(TimeSpan.FromMinutes(2))
            .Subscribe(_ => PeriodicSyncWithServer())
            .AddTo(_disposables);
    }

    private void PeriodicSyncWithServer()
    {
        foreach (GameCurrencies currency in Enum.GetValues(typeof(GameCurrencies)))
        {
            SyncCurrencyWithServer(currency)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogError($"Error syncing {currency}: {task.Exception}");
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public void StopPeriodicSync()
    {
        _disposables.Clear();
    }

    public async Task SyncCurrencies()
    {
        await WalletStorage.SyncCurrencies();
    }
    #endregion

    #region SignalBus
    private void FireSignal(GameCurrencies currencyType, int amount, bool isSpendSignal)
    {
        ResourceType resource;
        switch (currencyType)
        {
            case GameCurrencies.Gold:
                resource = ResourceType.Gold;
                break;

            case GameCurrencies.Gem:
                resource = ResourceType.Gems;
                break;

            case GameCurrencies.Key:
                resource = ResourceType.Keys;
                break;

            case GameCurrencies.Energy:
                resource = ResourceType.Energy;
                break;

            default:
                Debug.LogError($"Unknown currency {currencyType}");
                return;
        }

        _signalBus.Fire(new UseResourceSignal
        {
            IsSpendSignal = isSpendSignal,
            Count = amount,
            ResourceType = resource
        });
    }
    #endregion
}