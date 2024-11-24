using Data.Resources.AddressableManagement;
using Data.Resources.AddressableManagement.Interfaces;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UI.Popups;
using Zenject;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] private AuthenticationWindow _authenticationWindow;

    private ShopRepository _shopRepository;
    private ResourceManager _resourceManager;
    private IDataLoadingProgressTracker _dataLoadingProgress;
    private SaveManager _saveManager;
    private PlayerAnalyser _playerAnalyser;
    
    private string _playFabCustomID;

    [Inject]
    private void Construct(ShopRepository shopRepository, ResourceManager resourceManager,
        IDataLoadingProgressTracker dataLoadingProgress, SaveManager saveManager, PlayerAnalyser playerAnalyser)
    {
        _shopRepository = shopRepository;
        _resourceManager = resourceManager;
        _dataLoadingProgress = dataLoadingProgress;
        _saveManager = saveManager;
        _playerAnalyser = playerAnalyser;
    }

    public void Start()
    {
        SignInPlayFabService();
    }

    public void SignInPlayFabService()
    {
        var customIdRequest = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };

        _playFabCustomID = customIdRequest.CustomId;

        PlayFabClientAPI.LoginWithCustomID(customIdRequest, OnPlayFabSuccess, OnPlayFabError);
    }

    public void LoadGame()
    {
         LoadSave();
         _playerAnalyser.LoadEventData();

        //*** Initialize new added statistics ***
         //_playerAnalyser.InitAllStatistics();
    }    

    public void LoadSave()
    {
        _saveManager.LoadCloudPlayerData();
        _saveManager.LoadCloudQuests();
        _saveManager.LoadProgress();
        _saveManager.CheckEnergy();
        _saveManager.LoadCloudPvP();
        _saveManager.LoadFakeLeaderData();
    }

    public void CheckPlayerSvaves()
    {
        _saveManager.LoadCloudPlayerData();
    }

    private void CreateAccount()
    {
        _playerAnalyser.InitAllStatistics();

        Debug.Log("New Account Created");
    }

    private void OnPlayFabSuccess(LoginResult result)
    {
        LoadGame();

        if (result.NewlyCreated == true)
        {
            CreateAccount();
            GameLog.Instance.AddLog("PlayFab new account created. ID: " + _playFabCustomID);
        }
        else
        {
            GameLog.Instance.AddLog("PlayFab sign in Success. ID: " + _playFabCustomID);
            Debug.Log("PlayFab Sign in Success!");
            Debug.Log("Player PlayFab Id:" + _playFabCustomID);
        }

        GameEconomy.Initialize(_playFabCustomID, result.EntityToken.EntityToken, result.EntityToken.Entity.Id, result.EntityToken.Entity.Type, result.EntityToken.Entity.Id);

        StartLoadingScene();
    }

    private void OnPlayFabError(PlayFabError error)
    {
        GameLog.Instance.AddLog("PlayFab sign in ERROR. ID");
        Debug.Log("Error Sign in PlayFab!!!");
        Debug.Log(error.GenerateErrorReport());
    }
    private async void StartLoadingScene()
    {
        GameLog.Instance.AddLog("Start Loading Game");
        
        _resourceManager.SetLoadingCallbacks(UpdateLoadingProgress);

        ProgressDataLoaderUI.Instance.UpdateLoadingStatus("Initializing currency data...");
        await _dataLoadingProgress.LoadWithProgress(WalletStorage.InitializeCurrencyData);
        GameLog.Instance.AddLog("Scene Resources Loaded");
        
        ProgressDataLoaderUI.Instance.UpdateLoadingStatus("Updating inventory...");
        await _dataLoadingProgress.LoadWithProgress(PlayerInventory.Instance.AuthnUpdateInventory);
        GameLog.Instance.AddLog("Player Inventory Loaded");
            
        ProgressDataLoaderUI.Instance.UpdateLoadingStatus("Loading shop data...");
        await _dataLoadingProgress.LoadWithProgress(_shopRepository.LoadData);
        GameLog.Instance.AddLog("Shop Repository Loaded");
            
        ProgressDataLoaderUI.Instance.UpdateLoadingStatus("Loading main scene resources...");
        await _dataLoadingProgress.LoadWithProgress(_resourceManager.LoadMainMenuSceneAsync);
    }
    

    private void UpdateLoadingProgress(float progress)
    {
        LoadingPopup.Instance.UpdateLoadingProgress(progress);
    }
}
