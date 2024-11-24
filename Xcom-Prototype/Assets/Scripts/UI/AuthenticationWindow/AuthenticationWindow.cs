using DG.Tweening;
using System;
using Data.Resources.AddressableManagement;
using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class AuthenticationWindow : MonoBehaviour
{
    [Inject] private ShopRepository _shopRepository;

    [SerializeField] private Button _signInButton;
    [SerializeField] private Transform _authentication;
    [SerializeField] private TextMeshProUGUI _playerIdText;

    [Inject] private ResourceManager _resourceManager;

    public async void StartLoadingScene()
    {

        GameLog.Instance.AddLog("Start Loading Game");
        

        await WalletStorage.InitializeCurrencyData();

        GameLog.Instance.AddLog("Scene Resources Loaded");

        await PlayerInventory.Instance.UpdateInventory(true);

        GameLog.Instance.AddLog("Player Inventory Loaded");

        await _shopRepository.LoadData();

        GameLog.Instance.AddLog("Shop Repository Loaded");

        _resourceManager.LoadMainMenuSceneAsync();
    }
    private  void OnDestroy()
    {
        _signInButton.onClick.RemoveAllListeners();
        DOTween.Clear();
    }
}
