using Cysharp.Threading.Tasks;
using Data.Resources.AddressableManagement.Interfaces;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Data.Resources.AddressableManagement
{
    public class SceneLoader : ISceneLoader
    {
        public string GetActiveSceneName() => _currentSceneName;
        private string _currentSceneName;
        private readonly string _mainSceneName = "MainMenuScene_Dev_2";
    
        private AddressableGroupsDataContainer _addressableGroupsData;
        private IAddressableGroupsLoader _addressableGroupsLoader;
    
        [Inject]
        public SceneLoader(AddressableGroupsDataContainer addressableGroupsData, IAddressableGroupsLoader addressableGroupsLoader)
        {
            _addressableGroupsData = addressableGroupsData;
            _addressableGroupsLoader = addressableGroupsLoader;
        }


        private async UniTask LoadSceneAsync(string sceneName)
        {
            _currentSceneName = sceneName;
            var asyncLoad = Addressables.LoadSceneAsync(sceneName);
            _addressableGroupsData.CurrentSceneHandle = asyncLoad;
            await asyncLoad.ToUniTask();
        }

        public async UniTask LoadLevelAsync(string levelName)
        {
            _addressableGroupsData.ReleaseAllResources();
            await _addressableGroupsLoader.LoadAllResourcesForModeAsync();

            _addressableGroupsData.ReleaseScene(_addressableGroupsData.CurrentSceneHandle);
            await LoadSceneAsync(levelName);
          
        }

        public void ShowLoadingScreen()
        {
            LoadingPopup.Instance.ShowLoadingScreen();
        }

        public async UniTask LoadMainMenuSceneAsync()
        {
            _addressableGroupsData.ReleaseAllResources();
            await _addressableGroupsLoader.LoadMainMenuResourcesAsync();
            _addressableGroupsData.ReleaseScene(_addressableGroupsData.CurrentSceneHandle);
            await LoadSceneAsync(_mainSceneName);
        }
    }
}