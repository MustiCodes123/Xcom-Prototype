using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

[Serializable]
public class PlayerData 
{
    [JsonIgnore] public Action<BaseItem> ItemAddedToStorage;
    [JsonIgnore] public Action<BaseItem> ItemAddedToInventory;
    [JsonIgnore] public int MaxEnergy = 60;
    [JsonIgnore] public int MaxKeysCount = 50;
    [JsonIgnore] public int StartKeysCount = 5;
    [JsonIgnore] public int EnergyTimeToRecover = 300;
    [JsonIgnore] private string bestTimeKey = "BestTIme";
    [JsonIgnore] private string _bossProgressKey = "BossProgress";
    [JsonIgnore] private string energyTimeKey = "EnergyLastSpended";
    [JsonIgnore] private string _companyProgressKey = "CompanyProgress";
    [JsonIgnore] private Dictionary<string, int> BestTIme = new Dictionary<string, int>();
    [JsonIgnore] private Dictionary<string, Difficulty> _bossProgress = new Dictionary<string, Difficulty>();
    [JsonIgnore] private Dictionary<string, List<int>> _companyProgress = new Dictionary<string, List<int>>();
    [JsonIgnore] public List<BaseItem> PlayerInventory;
    [JsonIgnore] public List<BaseItem> PlayerInventoryStorage;

    [JsonProperty] public bool[] IsWeaponUpgradeSlotsAvalable = new bool[8];
    [JsonProperty] private List<int> IdsInventory;
    [JsonProperty] private List<int> StorageItemsIDs;
    [JsonProperty] private List<int> SkillsIds;
    [JsonProperty] private List<int> LevelsInventory;
    [JsonProperty] private List<int> ItemSkillsCount;

    public int PlayerID;
    public int PlayerXP;
    public int PlayerLevel;
    public int Money;
    public int Gems;
    public int Energy;
    public int ItemsToRemoveSkill;
    public int MaxInventorySize;
    public int CurrentInventorySize;
    public int InventoryExtention;
    public int CharacterExtension;
    public int UpgradeSlotsCount;
    public int CommonSummonCristal;
    public int RareSummonCristal;
    public int EpicSummonCristal;
    public int LegendarySummonCristal;
    public int MythicalSummonCristal;
    public int KeysCount;
    public string PlayerIconPath;
    public string PlayerName;
    public BaseGroupInfo PlayerGroup;
    public List<BaseSkillModel> PlayerAvailablesSkills = new List<BaseSkillModel>();


    private string _bossProgressString;
    private string _bestTimeString;

    private DateTime _energylastSpendedTime;

    public Dictionary<string, List<int>> GetCompanyProgres()
    {
        if (_companyProgress.Count > 0)
        {
            return _companyProgress;
        }
        else
        {
            _companyProgress = new Dictionary<string, List<int>>
            {
                {"1 - Bandits' Camp", new List<int> {0} }
            };
        }

        return _companyProgress;
    }

    private void SaveCloudCompanyProgress()
    {
        string savedString = JsonConvert.SerializeObject(_companyProgress);
        string saveTimeString = JsonConvert.SerializeObject(BestTIme);

        if (savedString != null)
        {
            var saveProgressRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                {
                    { _companyProgressKey, savedString},
                }
            };
            var saveBestTimeData = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                {
                    { bestTimeKey, saveTimeString},
                }
            };

            PlayFabClientAPI.UpdateUserData(saveProgressRequest, OnDataSaveSuccess, OnDataSendError);
            PlayFabClientAPI.UpdateUserData(saveBestTimeData, OnDataSaveSuccess, OnDataSendError);
        }
    }

    public void LoadCloudCompanyProgress()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCompanyProgressLoadSuccess, OnDataSendError);
    }

    public void SaveCloudBossProgress()
    {
        var bossProgressData = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
                {
                    { _bossProgressKey, _bossProgressString},
                }
        };

        PlayFabClientAPI.UpdateUserData(bossProgressData, OnDataSaveSuccess, OnDataSendError);
    }

    public void LoadCloudBestTime()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnBestTimeLoadSuccess, OnDataSendError);
    }

    public void SaveCloudEnergyCount()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                { energyTimeKey, _energylastSpendedTime.ToString()},
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSaveSuccess, OnDataSendError);
    }

    public void LoadCloudEnergyTime()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnEnergyLoadSuccess, OnDataSendError);
    }

    public void AddPvPReward(LevelUpReward reward)
    {
        foreach (var resource in reward.ResourcesReward.Resources)
        {
            if (resource.Type == ResourceType.Gold) Money += resource.Count;
            else if (resource.Type == ResourceType.Gems) Gems += resource.Count;
            else if (resource.Type == ResourceType.CommonSummonCrystal) CommonSummonCristal += resource.Count;
            else if (resource.Type == ResourceType.RareSummonCrystal) RareSummonCristal += resource.Count;
            else if (resource.Type == ResourceType.EpicSummonCrystal) EpicSummonCristal += resource.Count;
            else if (resource.Type == ResourceType.LegendarySummonCrystal) LegendarySummonCristal += resource.Count;
        }

        foreach (var item in reward.RewardItem)
        {
            AddItemToInventory(ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(item));
        }
    }

    public int GetBestTime(Stage stage, CampLevel campLevel)
    {
        string levelName = stage.Name + campLevel.Name.ToString();

        return BestTIme[levelName];
    }

    public void RestoreEnergy()
    {
        if (_energylastSpendedTime != DateTime.MinValue)
        {
            DateTime lastSpendTime = _energylastSpendedTime;

            TimeSpan timeSpan = DateTime.Now - lastSpendTime;

            int energyToRestore = (int)(timeSpan.TotalSeconds / EnergyTimeToRecover);

            if (energyToRestore > 0)
            {
                Energy += energyToRestore;

                _energylastSpendedTime = DateTime.Now;
                SaveCloudEnergyCount();

                if (Energy > MaxEnergy)
                {
                    Energy = MaxEnergy;
                }
            }
        } 
        else
        {
            Energy = MaxEnergy;
        }
    }

    public void SpendKey(int count)
    {
        KeysCount -= count;
    }

    public bool TryToSpendEnergy(int energy)
    {
        if (Energy >= energy)
        {
            Energy -= energy;

            _energylastSpendedTime = DateTime.Now;
            SaveCloudEnergyCount();

            return true;
        }

        return false;
    }

    public void AddFinishedLevel(Stage stage, CampLevel campLevel, int stars, int time = 0)
    {
        int campIndex = 0;
        bool allLevelsCompleted = true;

        if (_companyProgress.ContainsKey(stage.Name))
        {
            var list = _companyProgress[stage.Name];

            for (int i = 0; i < stage.Levels.Length; i++)
            {
                if (stage.Levels[i] == campLevel)
                    campIndex = i;
            }

            if (list.Count <= campIndex)
            {
                while (list.Count <= campIndex)
                {
                    list.Add(0);
                }
                list[campIndex] = stars;
            }
            else
            {
                list[campIndex] = Math.Max(list[campIndex], stars);
            }

            for (int i = 0; i < stage.Levels.Length; i++)
            {
                if (i >= list.Count || list[i] == 0)
                {
                    allLevelsCompleted = false;
                    break;
                }
            }
        }
        else
        {
            _companyProgress.Add(stage.Name, new List<int> { stars });
            allLevelsCompleted = false;
        }

        if (allLevelsCompleted && stage.NextStage != null)
        {
            if (!_companyProgress.ContainsKey(stage.NextStage.Name))
            {
                List<int> starsList = new List<int> { 0 };
                _companyProgress.Add(stage.NextStage.Name, starsList);
            }
        }

        string levelName = stage.Name + campLevel.Name.ToString();

        if (BestTIme.ContainsKey(levelName))
        {
            if (BestTIme[levelName] > time || BestTIme[levelName] == 0) 
            {
                BestTIme[levelName] = time;
                PlayerPrefs.SetFloat(levelName, BestTIme[levelName]);
            }
        }
        else
        {
            BestTIme.Add(levelName, time);
            PlayerPrefs.SetFloat(levelName, BestTIme[levelName]);
        }

        SaveCloudCompanyProgress();
    }


    public void MoveCharacterToGroup(BaseCharacterModel characterData, GroupType groupType)
    {
        switch (groupType)
        {
            case GroupType.Battle:
                PlayerGroup.AddCharacterToBattleGroup(characterData);
                PlayerGroup.RemoveCharacterFromNotAsignedGroup(characterData);
                break;
            case GroupType.None:
                PlayerGroup.RemoveCharacterFromBattleGroup(characterData);
                PlayerGroup.AddCharacterToNotAsignedGroup(characterData);
                break;
        }
    }

    public void AddItemToInventory(BaseItem item)
    {
        if(item is WeaponItem || item is ArmorItem)
        {
            if (HasFreeInventorySlots())
            {
                PlayerInventory.Add(item);
                ItemAddedToInventory?.Invoke(item);
            }
            else
            {
                PlayerInventoryStorage.Add(item);
                ItemAddedToStorage?.Invoke(item);
                Debug.Log($"Not enough space in player's inventory. {item.itemName} was moved to storage");
            }
        }
        else
        {
            PlayerInventoryStorage.Add(item);
            ItemAddedToStorage?.Invoke(item);
        }
    }

    public void RemoveItemFromInventory(BaseItem item)
    {
        if (PlayerInventory.Contains(item))
        {
            PlayerInventory.Remove(item);
        }
    }

    public void RemoveItemFromStorage(BaseItem itemToRemove)
    {
        if (PlayerInventoryStorage.Contains(itemToRemove))
        {
            PlayerInventoryStorage.Remove(itemToRemove);
        }
    }

    public bool HasFreeInventorySlots()
    {
        List<BaseItem> inventoryCopy = new List<BaseItem>(PlayerInventory);
        inventoryCopy.RemoveAll(baseItem => baseItem is ArtifactRecipeItem or CraftItem);

        return CurrentInventorySize > inventoryCopy.Count;
    }

    public List<BaseItem> GetInventoryItems()
    {
        List<BaseItem> inventoryCopy = new List<BaseItem>(PlayerInventory);
        inventoryCopy.RemoveAll(baseItem => baseItem is ArtifactRecipeItem or CraftItem);

        return inventoryCopy;
    }


    public void MoveItemToCharacterInventory(BaseCharacterModel characterData, BaseItem item)
    {        
        PlayerInventory.Remove(item);
        characterData.AddItemToCharacterInventory(item);
    }

    public bool IsItemEquiped(BaseCharacterModel characterData, BaseItem item)
    {
        if (characterData.EquipedItems.Contains(item)) return true;
        return false;
    }

    public void MoveItemFromCharacterToInventory(BaseCharacterModel characterData, BaseItem item)
    {
        if (HasFreeInventorySlots())
            PlayerInventory.Add(item);

        else
        {
            PlayerInventoryStorage.Add(item);
            ItemAddedToStorage?.Invoke(item);
        }

        characterData.RemoveItemFromCharacterInventory(item);
    }

    [OnDeserialized]
    protected void OnDeserialized(StreamingContext context)
    {
        PlayerInventory = new List<BaseItem>();
        PlayerInventoryStorage = new List<BaseItem>();

        if (ItemsDataInfo.Instance != null)
        {
            for (int i = 0; i < IdsInventory.Count; i++)
            {
                var template = ItemsDataInfo.Instance.GetItemTemplate(IdsInventory[i]);
                var item = ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(template, LevelsInventory[i], false);

                PlayerInventory.Add(item);
            }

            for (int i = 0; i < StorageItemsIDs.Count; i++)
            {
                var template = ItemsDataInfo.Instance.GetItemTemplate(StorageItemsIDs[i]);
                var item = ItemsDataInfo.Instance.ConvertTemplateToItem<BaseItem>(template, 1, false);

                PlayerInventoryStorage.Add(item);
            }

            int globalSkillID = 0;
            for (int i = 0; i < PlayerInventory.Count; i++)
            {
                for (int j = 0; j < ItemSkillsCount[i]; j++)
                {
                    int concretteItemSkillID = SkillsIds[globalSkillID];
                    globalSkillID++;

                    BaseSkillModel skill = SkillsDataInfo.Instance.GetSkillTemplate(concretteItemSkillID).GetSkill();
                    PlayerInventory[i].skillModels.Add(skill);
                }
            }
        }
    }

    [OnSerializing]
    protected void OnSerializing(StreamingContext context)
    {
        IdsInventory = new List<int>();
        StorageItemsIDs = new List<int>();
        LevelsInventory = new List<int>();
        SkillsIds = new List<int>();
        ItemSkillsCount = new List<int>();

        for (int i = 0; i < PlayerInventory.Count; i++)
        {
            IdsInventory.Add(PlayerInventory[i].itemID);
            LevelsInventory.Add(PlayerInventory[i].CurrentLevel);
            ItemSkillsCount.Add(PlayerInventory[i].skillModels.Count);

            for (int j = 0; j < PlayerInventory[i].skillModels.Count; j++)
            {
                SkillsIds.Add((int)PlayerInventory[i].skillModels[j].Id);
            }
        }

        for (int i = 0; i < PlayerInventoryStorage.Count; i++)
        {
            StorageItemsIDs.Add(PlayerInventoryStorage[i].itemID);
        }
    }

    public void CreateBossProgress(ThreeToOneContainer threeToOneContainer)
    {
        _bossProgress = new Dictionary<string, Difficulty>();
        for (int i = 0; i < threeToOneContainer.BossDatas.Length; i++)
        {
            if (i == 0)
                _bossProgress.Add(threeToOneContainer.BossDatas[i].CharacterPreset.PresetName, Difficulty.Easy);
            else
                _bossProgress.Add(threeToOneContainer.BossDatas[i].CharacterPreset.PresetName, Difficulty.None);
        }
        KeysCount = StartKeysCount;
        string savedString = JsonConvert.SerializeObject(_bossProgress);
        _bossProgressString = savedString;
    }

    public BossData GetLatestBoss(ThreeToOneContainer threeToOneContainer)
    {
        BossData data = null;
        for (int i = 0; i < threeToOneContainer.BossDatas.Length; i++)
        {
            if (_bossProgress.TryGetValue(threeToOneContainer.BossDatas[i].CharacterPreset.PresetName, out var difficulty) && (int)difficulty < (int)Difficulty.None)
            {
                data = threeToOneContainer.BossDatas[i];
            }
        }
        return data;
    }

    public void UpdateBossProgress(BossData data)
    {
        if (_bossProgress.TryGetValue(data.CharacterPreset.PresetName, out var difficult) && (int)difficult < (int)Difficulty.None)
        {
            if (difficult == Difficulty.Mythical)
            {
                for (int i = 0; i < _bossProgress.Keys.Count; i++)
                {
                    if (_bossProgress.ElementAt(i).Key == data.CharacterPreset.PresetName && i < _bossProgress.Keys.Count - 1)
                    {
                        _bossProgress[_bossProgress.ElementAt(i + 1).Key] = Difficulty.Easy;
                        break;
                    }
                }
            }
            else
            {
                _bossProgress[data.CharacterPreset.PresetName]++;
            }
        }
    }

    public int GetCristalCount(SummonCristalsEnum currentCristal)
    {
        switch(currentCristal)
        {
            case SummonCristalsEnum.Common:
                return CommonSummonCristal;
            case SummonCristalsEnum.Rare:
                return RareSummonCristal;
            case SummonCristalsEnum.Epic:
                return EpicSummonCristal;
            case SummonCristalsEnum.Legendary:
                return LegendarySummonCristal;
            case SummonCristalsEnum.Mythical:
                return MythicalSummonCristal;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RemoveCristal(SummonCristalsEnum currentCristal)
    {   
        switch (currentCristal)
        {
            case SummonCristalsEnum.Common:
                CommonSummonCristal--;
                break;
            case SummonCristalsEnum.Rare:
                RareSummonCristal--;
                break;
            case SummonCristalsEnum.Epic:
                EpicSummonCristal--;
                break;
            case SummonCristalsEnum.Legendary:
                LegendarySummonCristal--;
                break;
        }
    }

    private void OnEnergyLoadSuccess(GetUserDataResult result)
    {
        if (result.Data.ContainsKey(energyTimeKey))
        {
            _energylastSpendedTime = DateTime.Parse(result.Data[energyTimeKey].Value.ToString());
        }
        else
        {
            Debug.Log("Data no received");
        }
    }

    private void OnCompanyProgressLoadSuccess(GetUserDataResult result)
    {
        if (result.Data.ContainsKey(_companyProgressKey))
        {
            string progressLoadString = result.Data[_companyProgressKey].Value.ToString();
            _companyProgress = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(progressLoadString);
        }
    }

    private void OnBestTimeLoadSuccess(GetUserDataResult result)
    {
        if (result.Data.ContainsKey (bestTimeKey))
        {
            _bestTimeString = result.Data[bestTimeKey].Value.ToString();
            BestTIme = JsonConvert.DeserializeObject<Dictionary<string, int>>(_bestTimeString);
        }
        else
        {
            Debug.Log("Data no received");
        }
    }

    private void OnDataSaveSuccess(UpdateUserDataResult result)
    {

    }

    private void OnDataSendError(PlayFabError error)
    {
        Debug.Log("Data not sended!" + error.GenerateErrorReport());
    }
}
