using System.Collections.Generic;
using System;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public static class GameEconomy
{
    private static Dictionary<string, UserDataRecord> s_userData;
    private static bool s_isGettingUserData = false;

    public static string s_TitleId { get { return s_titleId; } }
    public static string s_PlayFabId { get; private set; }
    public static string s_EntityToken { get; private set; }
    public static string s_EntityID { get; private set; }
    public static string s_EntityType { get; private set; }
    public static string s_PlayerTitleId { get; private set; }

    private static string s_titleId = "30E7B";

    public static void Initialize(string playFabID, string entityToken, string entityID, string entityType, string playerTitleId)
    {
        s_PlayFabId = playFabID;
        s_EntityToken = entityToken;
        s_EntityID = entityID;
        s_EntityType = entityType;
        s_PlayerTitleId = playerTitleId;

        Debug.Log("<color=green>Game Economy initialized succesfuly. </color>");
    }

    public static async Task<bool> RemoveItem(string itemId, int amount, string stackID = "default")
    {
        string postURL = $"https://{s_TitleId}.playfabapi.com/Inventory/SubtractInventoryItems";

        UnityWebRequest www = new UnityWebRequest(postURL, "POST");

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("X-EntityToken", s_EntityToken);

        var requestBody = new
        {
            Entity = new { Type = "title_player_account", Id = s_PlayerTitleId },
            Item = new { Id = itemId, StackId = stackID },
            Amount = amount
        };

        string jsonBody = JsonConvert.SerializeObject(requestBody);
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        www.downloadHandler = new DownloadHandlerBuffer();

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{www.error}");
            Debug.LogError($"Response: {www.downloadHandler.text}");
            return false;
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            return true;
        }
    }

    public static void AddCatalogItemToInventory(PlayFab.EconomyModels.CatalogItem item, int amount = 1, string stackID = "default")
    {
        var entityKey = new PlayFab.EconomyModels.EntityKey
        {
            Id = s_EntityID,
            Type = s_EntityType
        };

        PlayFabEconomyAPI.AddInventoryItems(
            new PlayFab.EconomyModels.AddInventoryItemsRequest
            {
                Amount = amount,
                Entity = entityKey,
                Item = new PlayFab.EconomyModels.InventoryItemReference
                {
                    Id = item.Id,
                    StackId = stackID
                }
            },
            result =>
            {
                Debug.Log($"Added {amount} of {item.Title["NEUTRAL"]} to inventory");
                Debug.Log(result.IdempotencyId);
            },
            error =>
            {
                Debug.LogError($"Error adding item to inventory: {error.GenerateErrorReport()}");
            }
        );
    }

    public static void SaveData(
            Dictionary<string, string> Data,
            Action<UpdateUserDataResult> onSuccess,
            Action<PlayFabError> onError
        )

    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = Data
        },
            successResult =>
            {
                if(s_userData != null)
                    foreach(string key in Data.Keys)
                    {
                        UserDataRecord Value = new() { Value = Data[key] };

                        if (s_userData.ContainsKey(key))
                            s_userData[key] = Value;

                        else
                            s_userData.Add(key, Value);
                    }

                onSuccess(successResult);
            },
            onError
        );
    }

    public static async Task GetUserData(
            Action<GetUserDataResult> onSuccess,
            Action<PlayFabError> onError
        )

    {
        while (s_isGettingUserData)
            await Task.Delay(100);

        if(s_userData != null)
        {
            onSuccess(new GetUserDataResult() { Data = s_userData });
            return;
        }

        s_isGettingUserData = true;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), onSuccessResult =>
        {
            s_userData = onSuccessResult.Data;
            s_isGettingUserData = false;
            onSuccess(onSuccessResult);
        }, 
        
        onErrorResult =>
        {
            s_isGettingUserData = false;
            onError(onErrorResult);
        });
    }
}