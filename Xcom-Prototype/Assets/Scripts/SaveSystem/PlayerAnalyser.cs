using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerAnalyser 
{
    private SignalBus _signalBus;

    private const string _windowStatisticName = "OpenWindow";
    private const string _windowEventName = "Window_Opened";
    private const string _windowTypeName = "WindowType";

    private const string _battlesStatisticName = "TatalBattles";
    private const string _battlesWinEventName = "Battle_Win";
    private const string _battlesLooseEventName = "Battle_Loose";
    private const string _battlesTypeName = "BattleMode";
    private const string _battlesStageName = "Stage";
    private const string _battlesLvlNumberName = "LVL";

    private const string _summonHeroStatisticName = "TotalHeroesSummoned";
    private const string _summonHeroEventName = "Hero_Summon";
    private const string _summonHeroName = "Name";
    private const string _summonHeroRare = "Rare";

    private const string _upgradeHeroStatisticName = "TotalUpgradeHero";
    private const string _upgradeHeroEventName = "Hero_Upgrade";
    private const string _upgradeHeroName = "Name";
    private const string _upgradeHeroRare = "Rare";
    private const string _upgradeHeroLVL = "HeroLVL";

    private const string _upgradeItemStatisticName = "TotalItemUpgrade";
    private const string _upgradeItemEventName = "Item_Upgrade";
    private const string _upgradeItemName = "Name";
    private const string _upgradeItemType = "Type";
    private const string _upgradeItemRare = "Rare";
    private const string _upgradeItemLvl = "ItemLVL";

    private const string _characterEquipEventName = "Item_Equiped";
    private const string _characterEquipItemName = "Name";
    private const string _characterEquipItemType = "Type";
    private const string _characterEquipItemRare = "Rare";
    private const string _characterEquipItemLVL = "ItemLVL";

    private const string _shopTabClickStatisticName = "TotalShopTabClick";
    private const string _shopTabClickEventName = "ShopTab_Open";
    private const string _shopTabType = "ShopTabType";

    public PlayerAnalyser(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void InitAllStatistics()
    {
        CreatePlayerStatistic(_windowStatisticName);
        CreatePlayerStatistic(_battlesStatisticName);
        CreatePlayerStatistic(_summonHeroStatisticName);
        CreatePlayerStatistic(_upgradeHeroStatisticName);
        CreatePlayerStatistic(_upgradeItemStatisticName);
        CreatePlayerStatistic(_shopTabClickStatisticName);
    }

    public void LoadEventData()
    {
        _signalBus.Subscribe<LevelFinishSignal>(OnBattleFinished);
        _signalBus.Subscribe<SummonHeroSignal>(OnSummonHero);
        _signalBus.Subscribe<UpgradeSignal>(OnUpgrade);
        _signalBus.Subscribe<CharacterEquipSignal>(OnEquipCharacter);
        _signalBus.Subscribe<ShopWindowOpenSignal>(OnShopTabClick);
        _signalBus.Subscribe<OpenWindowSignal>(OnOpenWindowClick);
        
    }
    private void CreatePlayerStatistic(string statisticName)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName
                },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    private void UpdatePlayerStatistics(string  statisticName, int value)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName, Value = value
                },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    public void OpenWindowAPICall(OpenWindowSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _windowEventName,
            Body = new Dictionary<string, object>
                {
                    { _windowTypeName, signal.WindowType }
                }
        },
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void WinBattleAPICall(LevelFinishSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _battlesWinEventName,
            Body = new Dictionary<string, object>
                {
                    { _battlesTypeName, signal.GameMode.ToString() },
                    {_battlesStageName, signal.Stage.Name.ToString()},
                    {_battlesLvlNumberName, signal.campLevel.Name.ToString()}
                }
        },
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void LooseBattleAPICall(LevelFinishSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _battlesLooseEventName,
            Body = new Dictionary<string, object>
            {
                { _battlesTypeName, signal.GameMode.ToString() },
                {_battlesStageName, signal.Stage.Name.ToString()},
                {_battlesLvlNumberName, signal.campLevel.Name.ToString()}
            }
        }, 
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void SummonHeroAPICall(SummonHeroSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _summonHeroEventName,
            Body = new Dictionary<string, object>
            {
                {_summonHeroName, signal.Hero.PresetName },
                {_summonHeroRare, signal.Hero.Rare},
            }
        },
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void UpgradeHeroAPICall(UpgradeSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _upgradeHeroEventName,
            Body = new Dictionary<string, object>
            {
                {_upgradeHeroName, signal.Hero.Name },
                {_upgradeHeroRare, signal.Hero.Rare},
                {_upgradeHeroLVL, signal.Hero.Level}
            }
        },
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void UpgradeItemAPICall(UpgradeSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _upgradeItemEventName,
            Body = new Dictionary<string, object>
            {
                {_upgradeItemName, signal.Item.itemName },
                {_upgradeItemType, signal.Item.Slot},
                {_upgradeItemRare, signal.Item.Rare},
                {_upgradeItemLvl, signal.Item.CurrentLevel}
            }
        },
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void CharacterdEquipAPICall(CharacterEquipSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _characterEquipEventName,
            Body = new Dictionary<string, object>
            {
                {_characterEquipItemName, signal.baseItem.itemName},
                {_characterEquipItemType, signal.baseItem.Slot},
                {_characterEquipItemRare, signal.baseItem.Rare},
                {_characterEquipItemLVL, signal.baseItem.CurrentLevel}
            }
        },
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void ShopTabClickAPICall(ShopWindowOpenSignal signal)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = _shopTabClickEventName,
            Body = new Dictionary<string, object>
            {
                {_shopTabType, signal.ShopTab},
            }
        },
        OnEventSendSuccess,
        OnDataSendError);
    }

    public void OnBattleFinished(LevelFinishSignal signal)
    {
        if (signal.IsWin)
        {
            WinBattleAPICall(signal);
        }
        else
        {
            LooseBattleAPICall(signal);
        }
    }

    public void OnSummonHero(SummonHeroSignal signal)
    {
        SummonHeroAPICall(signal);
    }

    public void OnUpgrade(UpgradeSignal signal)
    {
        if (signal.IsCharacterUpgrade)
        {
            UpgradeHeroAPICall(signal);
        }
        else
        {
            UpgradeItemAPICall(signal);
        }
    }

    public void OnEquipCharacter(CharacterEquipSignal signal)
    {
        CharacterdEquipAPICall(signal);
    }
    
    public void OnShopTabClick(ShopWindowOpenSignal signal)
    {
        ShopTabClickAPICall(signal);
    }

    private void OnOpenWindowClick(OpenWindowSignal signal)
    {
        OpenWindowAPICall(signal);
    }
    private void OnEventSendSuccess(WriteEventResponse response)
    {
        Debug.Log("Event Data Sended");
    }

    private void OnDataSendError(PlayFabError error)
    {
        Debug.Log("Data Send Error" + error.GenerateErrorReport());
    }
}
