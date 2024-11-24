using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Data.Resources.AddressableManagement.GameResourcesLoaders
{
    public class MainMenuLoader : BaseLoader
    {
        [Inject]
        public MainMenuLoader(AddressableGroupsDataContainer addressableDataContainer, TemploaryInfo temploaryInfo, DataLoadingProgressTracker progressTracker) 
            : base(addressableDataContainer, temploaryInfo, progressTracker)
        {
        }

        public async UniTask LoadMainMenuResourcesAsync()
        {
            await ProgressTracker.LoadWithProgress(LoadAllAvatarsAsync);
            await ProgressTracker.LoadWithProgress(LoadAllForUICharactersAsync);
            await ProgressTracker.LoadWithProgress(LoadAllForUIBackgroundsAsync);
        }

        public async UniTask LoadAllForUICharactersAsync()
        {
            var addressableForUICharacters = AddressableDataContainer.CharactersResources.GetForUICharacters();
            await LoadAsyncOperationHandleAddressable<GameObject>(addressableForUICharacters, AddressableDataContainer.AddForUICharacters, "Loading UI Characters...");
        }

        public async UniTask LoadAllForUIBackgroundsAsync()
        {
            var addressableForUIBackgrounds = AddressableDataContainer.CharactersResources.GetForUIBackgrounds();
            await LoadAsyncOperationHandleAddressable<GameObject>(addressableForUIBackgrounds, AddressableDataContainer.AddForUIBackground, "Loading UI BackGrounds...");
        }
    }
}