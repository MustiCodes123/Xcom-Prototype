using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;
using System;
using PlayFab;
using PlayFab.ClientModels;

public class SaveManager
{
    private const string PvPDataKey = "PlayerPvPDataKey";
    private const string FakeLeaderDataKey = "FakeLeaderDataKey";
    private const string PlayerDataKey = "userData";
    private const string QuestDataKey = "questsData";

    private PlayerData _playerData;
    private PvPPlayerService _playerService;
    private List<LeaderSaveData> _leaderData;

    private ItemsDataInfo itemsDataInfo;
    private QuestManager questManager;
    private PvPBattleData _battleData;
    private ThreeToOneContainer _threeToOneContainer;

    private int _firstItemIndex = 10;

    [Inject]
    public SaveManager(ItemsDataInfo itemsDataInfo, PvPBattleData battleData, ThreeToOneContainer threeToOneContainer)
    {
        this.itemsDataInfo = itemsDataInfo;
        _battleData = battleData;
        _threeToOneContainer = threeToOneContainer;       
    }

    public void SetQuestManager(QuestManager questManager)
    {
        this.questManager = questManager;
    }

    public QuestManager GetQuestManager()
    {
        return questManager;
    }
    
    public void SaveGame()
    {
        _playerData.SaveCloudBossProgress();

        string json = JsonConvert.SerializeObject(_playerData);
        string pvpData = JsonConvert.SerializeObject(_playerService.Data);
        string saveLeadersData = SaveCloudFakeLeaderData();
        string questData = JsonConvert.SerializeObject(questManager.GetSaveData());

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary <string, string>()
            {
                { PlayerDataKey, json },
                { PvPDataKey, pvpData },
                { FakeLeaderDataKey, saveLeadersData },
                { QuestDataKey, questData }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendError);
    }

    public void LoadCloudFakeLeader()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnFakeLeaderDataReceivedSuccess, OnDataSendError);
    }

    public void LoadCloudPvP()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnPvPDataReceivedSuccess, OnDataSendError);
    }

    public void LoadCloudPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnPlayerDataRecieveSuccess, OnDataSendError);
    }

    public void  LoadCloudQuests()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnQuestsDataReceivedSuccess, OnDataSendError);
    }

    public void CheckEnergy()
    {
        _playerData.LoadCloudEnergyTime();
        _playerData.RestoreEnergy();
    }

    public string SaveCloudFakeLeaderData()
    {
        List<LeaderSaveData> datas = new List<LeaderSaveData>();
        for (int i = 0; i < _battleData.FakeLeaders.Count; i++)
        {
            datas.Add(_battleData.FakeLeaders[i].CurrentSaveData);
        }
        return JsonConvert.SerializeObject(datas);
    }

    public void LoadFakeLeaderData()
    {
        LoadCloudFakeLeader();

        if (_leaderData != null)
        {
            for (int i = 0; i < _battleData.FakeLeaders.Count; i++)
            {
                _battleData.FakeLeaders[i].SetCurrentData(_leaderData[i]);
            }
        }
        else
        {
            _leaderData = new List<LeaderSaveData>();
            for (int i = 0; i < _battleData.FakeLeaders.Count; i++)
            {
                var data = new LeaderSaveData(_battleData.FakeLeaders[i].LeaderData.Level, _battleData.FakeLeaders[i].LeaderData.Score);
                _battleData.FakeLeaders[i].SetCurrentData(data);
                _leaderData.Add(data);
            }
        }
    }

    public PvPPlayerService LoadPvPData()
    {
        _playerService = new PvPPlayerService();

        return _playerService;
    }

    public void LoadProgress()
    {
        _playerData.LoadCloudCompanyProgress();
        _playerData.LoadCloudBestTime();
    }

    public PlayerData InitPlayerData(bool needReset = false)
    {
        CreatePlayerDataTemplate();

        return _playerData;
    }

    public PlayerData CreatePlayerDataTemplate()
    {
        PlayerData data = new PlayerData();

        data.CharacterExtension = 0;
        data.InventoryExtention = 0;
        data.PlayerGroup = new BaseGroupInfo();
        data.PlayerInventory = new List<BaseItem>();
        data.PlayerInventoryStorage = new List<BaseItem>();
        data.PlayerGroup.MaxGroupSize = 10;
        data.CurrentInventorySize = 10;
        data.MaxInventorySize = 100;
        data.Energy = 60;
        data.UpgradeSlotsCount = 2;
        for (int i = 0; i < data.UpgradeSlotsCount; i++)
        {
            data.IsWeaponUpgradeSlotsAvalable[i] = true;
        }
        data.PlayerInventory = new List<BaseItem>()
        {

        };

        data.CreateBossProgress(_threeToOneContainer);

        _playerData = data;

        return data;
    }

    public void ReplaceSaves(PlayerData loadData)
    {
        _playerData.CharacterExtension = loadData.CharacterExtension;
        _playerData.InventoryExtention = loadData.InventoryExtention;
        _playerData.Money = loadData.Money;
        _playerData.PlayerGroup = loadData.PlayerGroup;
        _playerData.PlayerGroup.MaxGroupSize = loadData.PlayerGroup.MaxGroupSize;
        _playerData.CurrentInventorySize = loadData.CurrentInventorySize;
        _playerData.MaxInventorySize = loadData.MaxInventorySize;
        _playerData.PlayerInventory = loadData.PlayerInventory;
        _playerData.PlayerInventoryStorage = loadData.PlayerInventoryStorage;
        _playerData.UpgradeSlotsCount = loadData.UpgradeSlotsCount;
        _playerData.PlayerXP = loadData.PlayerXP;
        _playerData.PlayerLevel = loadData.PlayerLevel;
        _playerData.Gems = loadData.Gems;
        _playerData.PlayerName = loadData.PlayerName;
        _playerData.PlayerIconPath = loadData.PlayerIconPath;
        _playerData.PlayerID = loadData.PlayerID;
        _playerData.Energy = loadData.Energy;
        _playerData.IsWeaponUpgradeSlotsAvalable = loadData.IsWeaponUpgradeSlotsAvalable;
    }

    private void OnDataSendSuccess(UpdateUserDataResult result)
    {
        //Debug.Log("Data Sended to Cloud");
    }

    private void OnPlayerDataRecieveSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(PlayerDataKey))
        {
            var data = JsonConvert.DeserializeObject<PlayerData>(result.Data[PlayerDataKey].Value.ToString());
            ReplaceSaves(data);
        }
        else
        {
            Debug.Log("Player Data no received");
        }
    }

    private void OnFakeLeaderDataReceivedSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(FakeLeaderDataKey))
        {
            string data = result.Data[FakeLeaderDataKey].Value.ToString();

            _leaderData = JsonConvert.DeserializeObject<List<LeaderSaveData>>(data);
        }
        else
        {
            Debug.Log("PVP Data no received");
        }
    }

    private void OnPvPDataReceivedSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(PvPDataKey))
        {
            string data = result.Data[PvPDataKey].Value.ToString();

            _playerService.Data = JsonConvert.DeserializeObject<PlayerPvPSavedData>(data);
        }
        else
        {
            _playerService.Data = new PlayerPvPSavedData();

            Debug.Log("PVP Data no received");
        }
    }

    private void OnQuestsDataReceivedSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(QuestDataKey))
        {
            string jsonData = result.Data[QuestDataKey].Value;
            AllQuestsJsonData allQuestsData = JsonConvert.DeserializeObject<AllQuestsJsonData>(jsonData);
            questManager.LoadQuests(allQuestsData);
            Debug.Log($"Quests data loaded successfully");
            Debug.Log($"JSON: {jsonData}");
        }
        else
        {
            questManager.ResetAllQuests();
            Debug.Log("Data no received");
        }
    }

    private void OnDataSendError(PlayFabError error)
    {
        Debug.Log("Data not sended!" + error.GenerateErrorReport());
    }
}