using System;
using Cysharp.Threading.Tasks;
using Data.Resources.AddressableManagement.Interfaces;
using UnityEngine;
using Zenject;

namespace Data.Resources.AddressableManagement
{
    public class ResourceManager : MonoBehaviour
    {
        private AddressableGroupsDataContainer _addressableContainer;
        private ISceneLoader _sceneLoader;
        private IDataLoadingProgressTracker _progressTracker;
        private IAddressableGroupsLoader _addressableLoader;
        
        public CharacterPresetsRegister CharacterPresetsRegister { get; private set; }


        [Inject]
        public void Construct(AddressableGroupsDataContainer addressableContainer, ISceneLoader sceneLoader,
            IDataLoadingProgressTracker progressTracker, IAddressableGroupsLoader addressableLoader, CharacterPresetsRegister characterPresetsRegister)
        {
            _addressableContainer = addressableContainer;
            _sceneLoader = sceneLoader;
            _progressTracker = progressTracker;
            _addressableLoader = addressableLoader;
            CharacterPresetsRegister = characterPresetsRegister;
        }

        public void SetLoadingCallbacks(Action<float> progressCallback)
        {
            _progressTracker.SetLoadingCallbacks(progressCallback);
        }

        #region Scene Management

        public string GetActiveSceneName() => _sceneLoader.GetActiveSceneName();

        public async void LoadLevelAsync(string levelName) => await _sceneLoader.LoadLevelAsync(levelName);

        public void ShowLoadingScreen() => _sceneLoader.ShowLoadingScreen();

        public async UniTask LoadMainMenuSceneAsync() => await _sceneLoader.LoadMainMenuSceneAsync();

        #endregion

        #region Resource Loading
        
        
        private async UniTask LoadAllResourcesForModeAsync() => await _addressableLoader.LoadAllResourcesForModeAsync();

        #endregion

        #region Resource Find Opperation
        
        public GameObject LoadForUIBackground(string backgroundName) => _addressableContainer.LoadForUIBackground(backgroundName);
        public GameObject LoadParticlePrefab(string particleName) => _addressableContainer.LoadParticlePrefab(particleName);
        public GameObject LoadProjectilePrefab(string projectileName) => _addressableContainer.LoadProjectilePrefab(projectileName);
        public GameObject LoadDecalePrefab(string decaleName) => _addressableContainer.LoadDecalePrefab(decaleName);
        public BaseCharacerView LoadEnemyBaseCharacterViewPrefab(string enemyName) => _addressableContainer.LoadEnemyBaseCharacterViewPrefab(enemyName);
        public GameObject LoadBaseCharacterPrefab(string characterName) => _addressableContainer.LoadBaseCharacterPrefab(characterName);
        public GameObject LoadForUICharacter(string characterName) => _addressableContainer.LoadForUICharacter(characterName);
        public CharacterSlotsHolder LoadForUISlotHolder(string characterName) => _addressableContainer.LoadForUISlotHolder(characterName);
        public ForUICharacterController LoadForUIController(string characterName) => _addressableContainer.LoadForUIController(characterName);
        public Sprite LoadSprite(string spriteName) => _addressableContainer.LoadSprite(spriteName);

        #endregion
        
        public async UniTask<GameObject> LoadItemPrefabAsync(string itemName) => await _addressableContainer.LoadItemPrefabAsync(itemName);
        public async UniTask<Sprite> LoadItemSpriteAsync(string itemID) => await _addressableContainer.LoadItemSpriteAsync(itemID);

        public Sprite GetEmptySprite() => _addressableContainer.NullIconSprite;
    }
}