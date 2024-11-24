using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GamePassCardViewConfig
{
    [field: SerializeField] public Sprite FrameSprite { get; private set; }
    [field: SerializeField] public TMP_FontAsset LvlTMPFont { get; private set; }
    [field: SerializeField] public bool IsLocked { get; private set; }
    [field: SerializeField] public bool IsPurchased { get; private set; }
}

public enum GamePassCardStates
{
    Awailable,
    NotEnoughLevel,
    NotPurchased
};

public class GamePassCard : ShopItem
{
    [SerializeField] private GamePassCardViewConfig _awailableConfig;
    [SerializeField] private GamePassCardViewConfig _notEnoughLevelConfig;
    [SerializeField] private GamePassCardViewConfig _notPurchasedConfig;

    [SerializeField] private UnityEngine.UI.Image _frameImage;
    [SerializeField] private UnityEngine.UI.Image _lockImage;
    [SerializeField] private GameObject _claimed;
    [SerializeField] private GameObject _notEnoughLevel;

    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _rewardText;

    private PlayerData _playerData;
    private GamePassCardStates _currentState;

    public int Level { get; private set; }

    public async void InitializeWithReward(PlayFab.EconomyModels.CatalogItem item, GamePassWindowData.GamePassReward customData, int rewardQuantity, Wallet wallet, ShopPresenter presenter, bool isPurchased)
    {
        Wallet = wallet;
        Presenter = presenter;
        CatalogItem = item;
        Level = customData.Level;

        string imageURL = item.Images.FirstOrDefault().Url;
        await DownloadImageFromUrl(imageURL);

        SetupView(customData, rewardQuantity, isPurchased);
        CheckRewardClaimedStatus();
        PurchaseButton.onClick.AddListener(ClaimReward);

        _lockImage.gameObject.SetActive(!isPurchased);
        UpdateButtonState(isPurchased);
    }

    public void InjectPlayerData(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void ClaimReward()
    {
        if (CatalogItem == null)
        {
            Debug.LogError("CatalogItem is not set for this game pass card.");
            return;
        }

        GameEconomy.AddCatalogItemToInventory(CatalogItem);

        ShopHelper.AddRewardToInventory(CatalogItem);

        GameEconomy.SaveData(
            new Dictionary<string, string>
            {
                { $"GamePassRewardClaimed_Level:{CatalogItem.Id}", "true" }
            },
            result =>
            {
                SetupRewardClaimedView(isLocked: true);
            },
            error =>
            {
                Debug.LogError($"Failed to save reward claimed status: {error.ErrorMessage}");
            });
    }

    public void SetPurchased(bool isPurchased)
    {
        if (!isPurchased)
        {
            SetState(GamePassCardStates.NotPurchased);
            PurchaseButton.interactable = false;

            return;
        }
        else if (Level > _playerData.PlayerLevel)
        {
            SetState(GamePassCardStates.NotEnoughLevel);
        }
        else
        {
            SetState(GamePassCardStates.Awailable);
        }
    }

    public void SetState(GamePassCardStates newState)
    {
        _currentState = newState;
        switch (_currentState)
        {
            case GamePassCardStates.Awailable:
                ApplyViewConfig(_awailableConfig);
                break;
            case GamePassCardStates.NotEnoughLevel:
                ApplyViewConfig(_notEnoughLevelConfig);
                break;
            case GamePassCardStates.NotPurchased:
                ApplyViewConfig(_notPurchasedConfig);
                break;
        }
    }
    private async void CheckRewardClaimedStatus()
    {
        await Presenter.ShowLoadingScreen(async () =>
        {
            await GameEconomy.GetUserData(
                result =>
                {
                    Dictionary<string, UserDataRecord> data = result.Data;
                    string record = data.TryGetValue($"GamePassRewardClaimed_Level:{CatalogItem.Id}", out var recordValue) ? recordValue.Value : null;

                    SetupRewardClaimedView(isLocked: record == "true");
                },
                error =>
                {
                    Debug.LogError($"Failed to get user data: {error.ErrorMessage}");
                });
        });
    }

    private void SetupRewardClaimedView(bool isLocked)
    {
        _claimed.SetActive(isLocked);
    }

    public void SetupView(GamePassWindowData.GamePassReward customData, int quantity, bool isPurchased)
    {
        _levelText.text = $"LVL {customData.Level}";
        _rewardText.text = quantity.ToString();

        if (!isPurchased)
        {
            SetState(GamePassCardStates.NotPurchased);
        }
        else if (customData.Level > _playerData.PlayerLevel)
        {
            SetState(GamePassCardStates.NotEnoughLevel);
        }
        else
        {
            SetState(GamePassCardStates.Awailable);
        }
    }

    private void ApplyViewConfig(GamePassCardViewConfig config)
    {
        _notEnoughLevel.SetActive(config.IsLocked);
        _levelText.font = config.LvlTMPFont;
        _frameImage.sprite = config.FrameSprite;
        _lockImage.gameObject.SetActive(!config.IsPurchased);

        UpdateButtonState(config.IsPurchased);
    }

    private void UpdateButtonState(bool isPurchased)
    {
        PurchaseButton.interactable = !isPurchased;
    }
}

[Serializable]
public class GamePassCardRewardData
{
    public string ItemReferenceID { get; set; }
    public string RequireLevel { get; set; }
    public int Amount { get; set; }
}