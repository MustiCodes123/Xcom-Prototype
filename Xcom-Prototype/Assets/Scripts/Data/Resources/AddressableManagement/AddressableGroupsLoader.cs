using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Resources.AddressableManagement.GameResourcesLoaders;
using Data.Resources.AddressableManagement.Interfaces;
using UI.Popups;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Data.Resources.AddressableManagement
{
    public class AddressableGroupsLoader : IAddressableGroupsLoader
    {
        private TemploaryInfo _temploaryInfo;
        private MainMenuLoader _mainMenuLoader;
        private CompanyLoader _companyLoader;
        private PvPLoader _pvPLoader;
        private TestOfStrengthLoader _testOfStrengthLoader;
        private ThreeToOneLoader _threeToOneLoader;
        private DataLoadingProgressTracker _progressTracker;
        private AddressableGroupsDataContainer _addressableDataContainer;
    
        [Inject]
        public AddressableGroupsLoader(TemploaryInfo temploaryInfo, MainMenuLoader mainMenuLoader, CompanyLoader companyLoader, PvPLoader pvPLoader, TestOfStrengthLoader testOfStrengthLoader, ThreeToOneLoader threeToOneLoader, DataLoadingProgressTracker progressTracker, AddressableGroupsDataContainer addressableDataContainer)
        {
            _temploaryInfo = temploaryInfo;
            _mainMenuLoader = mainMenuLoader;
            _companyLoader = companyLoader;
            _pvPLoader = pvPLoader;
            _testOfStrengthLoader = testOfStrengthLoader;
            _threeToOneLoader = threeToOneLoader;
            _progressTracker = progressTracker;
            _addressableDataContainer = addressableDataContainer;
        }

        public async UniTask LoadMainMenuResourcesAsync()
        {
            await _mainMenuLoader.LoadMainMenuResourcesAsync();
        }

        public async UniTask LoadAllResourcesForModeAsync()
        {
            await _progressTracker.LoadWithProgress(LoadAllAvatarsAsync);
            
            switch (_temploaryInfo.CurrentMode.GameMode)
            {
                case GameMode.Default:
                     await _companyLoader.LoadCompanyModeResourcesAsync();
                     break;
                case GameMode.PvP:
                    await _pvPLoader.LoadPvPResourcesAsync();
                    break;
                case GameMode.TestOfStrenght:
                    await _testOfStrengthLoader.LoadTOSResourcesAsync();
                    break;
                case GameMode.ThreeToOne:
                    await _threeToOneLoader.LoadThreeToOneResourcesAsync();
                    break;
            }
        }

        

        private async UniTask LoadResourceAsync<T>(ICollection<T> targetList, IReadOnlyList<AssetReference> addressables, string loadingMessageFormat) where T : Object
        {
            long totalDownloadSize = 0;
            
            for (int i = 0; i < addressables.Count; i++)
            {
                var addressable = addressables[i];
                totalDownloadSize += await Addressables.GetDownloadSizeAsync(addressable).Task;
            }
            
            if (totalDownloadSize == 0)
            {
                totalDownloadSize = addressables.Count * 256;
            }
            
          
            
            for (int i = 0; i < addressables.Count; i++)
            {
                ProgressDataLoaderUI.Instance.StartLoading(string.Format(loadingMessageFormat, i + 1, addressables.Count));
                ProgressDataLoaderUI.Instance.UpdateProgress(0, totalDownloadSize);
                var addressable = addressables[i];
                var handle = Addressables.LoadAssetAsync<T>(addressable);
                _addressableDataContainer.ResourcesInWorkHandles.Add(handle);

                while (!handle.IsDone)
                {
                    var downloadedBytes = handle.PercentComplete * totalDownloadSize / addressables.Count;
                    ProgressDataLoaderUI.Instance.UpdateProgress(downloadedBytes, totalDownloadSize);

                    await UniTask.Yield();
                }

                targetList.Add(handle.Result);
            }
        }
        
        private async UniTask LoadAllAvatarsAsync() => await LoadResourceAsync(_addressableDataContainer.Avatars, _addressableDataContainer.AvatarResources.GetAvatars(), "Avatar {0}/{1}");

        private async UniTask LoadAllForUICharactersAsync() => await LoadResourceAsync(_addressableDataContainer.ForUICharacters, _addressableDataContainer.CharactersResources.GetForUICharacters(), "UI Character {0}/{1}");

        private async UniTask LoadAllForUIBackgroundsAsync() => await LoadResourceAsync(_addressableDataContainer.ForUIBackGrounds, _addressableDataContainer.CharactersResources.GetForUIBackgrounds(), "UI Background {0}/{1}");

        private async UniTask LoadAllCharactersPrefabsAsync() => await LoadResourceAsync(_addressableDataContainer.CharactersPrefabs, _addressableDataContainer.CharactersResources.GetCharactersPrefabs(), "Character Prefab {0}/{1}");

        private async UniTask LoadAllEnemiesPrefabsAsync() => await LoadResourceAsync(_addressableDataContainer.EnemiesPrefabs, _addressableDataContainer.CharactersResources.GetAllEnemiesPrefabs(), "Enemy Prefab {0}/{1}");

        private async UniTask LoadAllDecalesAsync() => await LoadResourceAsync(_addressableDataContainer.DecalesPrefabs, _addressableDataContainer.SkillResources.GetDecales(), "Decale {0}/{1}");

        private async UniTask LoadAllParticlesAsync() => await LoadResourceAsync(_addressableDataContainer.Particles, _addressableDataContainer.SkillResources.GetParticles(), "Particle {0}/{1}");

        private async UniTask LoadAllProjectilesAsync() => await LoadResourceAsync(_addressableDataContainer.Projectiles, _addressableDataContainer.SkillResources.GetProjectiles(), "Projectile {0}/{1}");
    }
}