using DG.Tweening;
using System;
using Data.Resources.AddressableManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public abstract class UIWindowView : MonoBehaviour
{
    [Inject] protected UIWindowManager windowManager;
    [Inject] protected SignalBus _signalBus;
    [Inject] protected PlayerData playerData;
    [Inject] protected PlayerAnalyser _analyser;
    [Inject] protected ResourceManager _resourceManager;
    [Inject] protected TemploaryInfo _tempInfo;
    [Inject] protected SaveManager _saveManager;
    [Inject] protected TemploaryInfo temploaryInfo;

    [SerializeField] protected Button closeButton;
    [SerializeField] protected Button changeLevelButton;
    [SerializeField] protected Transform showPosition;
    [SerializeField] protected Transform hidePosition;
    [SerializeField] TMP_Text energyText;
    [SerializeField] protected float animationDuration = 0.5f;
    [SerializeField] protected float fadeDuration = 0.3f;
    public WindowsEnum windowType;
    [SerializeField] protected GameObject infoPanel;
    [SerializeField] protected Button infoButton; 


    protected CanvasGroup CanvasGroup;
    

    protected virtual void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        if (CanvasGroup != null)
        {
            CanvasGroup.alpha = 0;
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        if (changeLevelButton != null)
        {
            changeLevelButton.onClick.AddListener(OnChangeLevelButtonClick);
        }
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(ShowInfoPanel);
        }
    }
    private void ShowInfoPanel()
    {
    if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
    }


    protected virtual void OnCloseClicked()
    {       
        windowManager.HideWindow(windowType);
    }

    protected async void OnChangeLevelButtonClick()
    {
        temploaryInfo.SelectedCharacters.Clear();
        temploaryInfo.FirstWinfow = WindowsEnum.LevelSelectionWindow;
        _resourceManager.ShowLoadingScreen();
        await _resourceManager.LoadMainMenuSceneAsync();
    }

    protected virtual void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
        if (changeLevelButton != null)
        {
            changeLevelButton.onClick.RemoveAllListeners();
        }
    }

    public virtual void Show()
    {
        transform.DOKill(true);
        gameObject.SetActive(true);
        
        CanvasGroup.DOFade(1, fadeDuration).SetEase(Ease.InOutQuad);

        _signalBus.Fire(new OpenWindowSignal { WindowType = windowType });
    }
    public virtual void ShowWOAnimation()
    {
        transform.DOKill(true);
        gameObject.SetActive(true);

        CanvasGroup.alpha = 1;

        _signalBus.Fire(new OpenWindowSignal { WindowType = windowType });
    }

    public virtual void Hide()
    {
        InfoPopup.Instance.Hide();
        transform.DOKill(true);
        
        CanvasGroup.DOFade(0, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() => gameObject.SetActive(false));
    }

    public virtual void HideImmediate()
    {
        InfoPopup.Instance.Hide();
        transform.DOKill(true);
        gameObject.SetActive(false);
    }

    protected bool CanUseEnergy(int count)
    {
        if (Wallet.Instance.SpendCachedCurrency(GameCurrencies.Energy, (uint)count))
        {
            _signalBus.Fire(new UseResourceSignal() { Count = count, ResourceType = ResourceType.Energy, IsSpendSignal = true });
            return true;
        }

        return false;
    }
    
    protected bool CanUseKeys(int count)
    {
        if (Wallet.Instance.SpendCachedCurrency(GameCurrencies.Key, (uint)count))
        {
            _signalBus.Fire(new UseResourceSignal() { Count = count, ResourceType = ResourceType.Keys, IsSpendSignal = true });
            return true;
        }

        return false;
    }

    protected void UseMoney(int count)
    {
        _signalBus.Fire(new UseResourceSignal() { Count = count, ResourceType = ResourceType.Gold, IsSpendSignal = true });
        Wallet.Instance.SpendCachedCurrency(GameCurrencies.Gold, (uint)count);
    }

    protected void CollectMoney(int count)
    {
        _signalBus.Fire(new UseResourceSignal() { Count = count, ResourceType = ResourceType.Gold, IsSpendSignal = false });
        Wallet.Instance.AddCachedCurrency(GameCurrencies.Gold, count);
    }

    public void DisplayEnergy()
    {
        if (_tempInfo.CurrentMode.GameMode is GameMode.PvP or GameMode.TestOfStrenght or GameMode.ThreeToOne)
        {
            int keys = (int)Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Key);
            energyText.text = $"{keys}/{GameConstants.KeyLimit}";
        }
        else
        {
            int energy = (int)Wallet.Instance.GetCachedCurrencyAmount(GameCurrencies.Energy);
            energyText.text = $"{energy}/{GameConstants.EnergyLimit}";
        }
    }
}
