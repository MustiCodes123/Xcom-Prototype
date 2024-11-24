using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data.Resources.AddressableManagement;
using UI.Popups;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

public class BaseLoader 
{
    protected AddressableGroupsDataContainer AddressableDataContainer;
    protected TemploaryInfo _temploaryInfo;
    protected DataLoadingProgressTracker ProgressTracker;
   
    [Inject]
    public BaseLoader(AddressableGroupsDataContainer addressableDataContainer, TemploaryInfo temploaryInfo, DataLoadingProgressTracker progressTracker)
    {
        AddressableDataContainer = addressableDataContainer;
        _temploaryInfo = temploaryInfo;
        ProgressTracker = progressTracker;
    }

    
    protected async UniTask LoadAsyncOperationHandleAddressable<T>(IEnumerable<object> keys, Action<T> addAction, string message) where T : Object
    {
        var enumerable = keys.ToList();
        var totalAssets = enumerable.Count;
        var loadedAssets = 0;
        
        foreach (AsyncOperationHandle<T> handle in enumerable.Select(Addressables.LoadAssetAsync<T>))
        {
            AddressableDataContainer.AddResourceOperation(handle);
            await handle.Task;
            addAction(handle.Result);
            
            ProgressDataLoaderUI.Instance.UpdateLoadingStatus(string.Format(message, loadedAssets + 1, totalAssets));
            loadedAssets++;
        }
    }
    
    protected async UniTask LoadAsyncOperationHandleAddressable<T>(string keys, Action<T> addAction, string message) where T : Object
    {
        var enumerable = keys.ToList();
        var totalAssets = enumerable.Count;
        var loadedAssets = 0;
        
        foreach (AsyncOperationHandle<T> handle in enumerable.Select(key => Addressables.LoadAssetAsync<T>(key)))
        {
            AddressableDataContainer.AddResourceOperation(handle);
            await handle.Task;
            addAction(handle.Result);
            
            ProgressDataLoaderUI.Instance.UpdateLoadingStatus(string.Format(message, loadedAssets + 1, totalAssets));
            loadedAssets++;
        }
    }
    
    protected async UniTask LoadAllAvatarsAsync()
    {
        var addressableAvatars = AddressableDataContainer.AvatarResources.GetAvatars();
        await LoadAsyncOperationHandleAddressable<Sprite>(addressableAvatars, AddressableDataContainer.AddAvatars, "Loading avatars");
    }
}