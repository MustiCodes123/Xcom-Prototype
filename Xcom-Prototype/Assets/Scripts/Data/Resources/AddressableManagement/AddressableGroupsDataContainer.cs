using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Resources.AddressableManagement.Interfaces;
using Data.Resources.ScriptableObjects;
using UI.Popups;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data.Resources.AddressableManagement
{
    public class AddressableGroupsDataContainer : MonoBehaviour, IAddressableGroupsContainer
    {   
        //private const string UIKey = "ForUI";
    
        [SerializeField] private AvatarResources _avatarResources;
        [SerializeField] private CharactersResources _charactersResources;
        [SerializeField] private SkillsResources _skillResources;
        [SerializeField] private Sprite _nullIconSprite;

        public AsyncOperationHandle CurrentSceneHandle;
    
        public AvatarResources AvatarResources=> _avatarResources;
        public CharactersResources CharactersResources=> _charactersResources;
        public SkillsResources SkillResources=> _skillResources;

        public Sprite NullIconSprite => _nullIconSprite;
        public List<Sprite> Avatars => _avatars;
        public List<GameObject> ForUICharacters => _forUICharacters;
        public List<GameObject> ForUIBackGrounds => _forUIBackGrounds;
        public List<GameObject> CharactersPrefabs => _charactersPrefabs;
        public List<GameObject> EnemiesPrefabs => _enemiesPrefabs;
        public List<GameObject> DecalesPrefabs => _decalesPrefabs;
        public List<GameObject> Particles => _particles;
        public List<GameObject> Projectiles => _projectiles;
        public List<GameObject> WeaponPrefabs => _weaponPrefabs;
        public List<Sprite> ItemSprites => _itemSprites;
        public List<AsyncOperationHandle> ResourcesInWorkHandles => _resourcesInWorkHandles;
    
        private List<Sprite> _avatars = new();
        private List<Sprite> _itemSprites = new();
        private List<GameObject> _forUICharacters = new();
        private List<GameObject> _forUIBackGrounds = new();
        private List<GameObject> _charactersPrefabs = new();
        private List<GameObject> _enemiesPrefabs = new();
        private List<GameObject> _decalesPrefabs = new();
        private List<GameObject> _particles = new();
        private List<GameObject> _projectiles = new();
        private List<GameObject> _weaponPrefabs = new();
        private List<AsyncOperationHandle> _resourcesInWorkHandles = new();
    
        public void ReleaseScene(AsyncOperationHandle sceneOperation)
        {
            if (sceneOperation.IsValid())
            {
                Addressables.UnloadSceneAsync(sceneOperation);
            }
        }
    
        public void ReleaseAllResources()
        {
            foreach (var handle in _resourcesInWorkHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

            _resourcesInWorkHandles.Clear();
            ClearAllLists();
        }
    
        private void ClearAllLists()
        {
            _avatars.Clear();
            _forUICharacters.Clear();
            _forUIBackGrounds.Clear();
            _charactersPrefabs.Clear();
            _enemiesPrefabs.Clear();
            _decalesPrefabs.Clear();
            _particles.Clear();
            _projectiles.Clear();
        }
    
        public void AddAvatars(Sprite avatar) => _avatars.Add(avatar);
        public void AddForUICharacters(GameObject prefab) => _forUICharacters.Add(prefab);
        public void AddForUIBackground(GameObject prefab) => _forUIBackGrounds.Add(prefab);
        public void AddCharactersPrefabs(GameObject character) => _charactersPrefabs.Add(character);
        public void AddEnemiesPrefabs(GameObject enemyPrefab) => _enemiesPrefabs.Add(enemyPrefab);
        public void AddDecales(GameObject prefab) => _decalesPrefabs.Add(prefab);

        public void AddProjectiles(GameObject projectile) => _projectiles.Add(projectile);
        public void AddWeaponPrefab(GameObject weapon) => _weaponPrefabs.Add(weapon);
        public void AddParticles(GameObject particle) => _particles.Add(particle);

        public void AddResourceOperation(AsyncOperationHandle operation) => _resourcesInWorkHandles.Add(operation);
        public bool IsAssetLoaded(GameObject assetToLoad, List<GameObject> loadedAssets) => loadedAssets.Exists(asset => assetToLoad.name == asset.name);
    
        public GameObject LoadForUIBackground(string backgroundName) => _forUIBackGrounds.Find(bg => bg.name == backgroundName) ?? _forUIBackGrounds[0];

        public GameObject LoadParticlePrefab(string particleName) => _particles.Find(p => p.name == particleName) ?? _particles[0];

        public GameObject LoadProjectilePrefab(string projectileName) => _projectiles.Find(p => p.name == projectileName) ?? _projectiles[0];

        public GameObject LoadDecalePrefab(string decaleName) => _decalesPrefabs.Find(d => d.name == decaleName) ?? _decalesPrefabs[0];

        public BaseCharacerView LoadEnemyBaseCharacterViewPrefab(string enemyName) => _enemiesPrefabs.Find(e => e.name == enemyName)?.GetComponent<BaseCharacerView>() ?? _enemiesPrefabs[0].GetComponent<BaseCharacerView>();

        public GameObject LoadBaseCharacterPrefab(string characterName) => _charactersPrefabs.Find(c => c.name == characterName) ?? _charactersPrefabs[0];

        public GameObject LoadForUICharacter(string characterName) => _forUICharacters.Find(c => c.name == characterName) ?? _forUICharacters[0];

        public CharacterSlotsHolder LoadForUISlotHolder(string characterName) => _forUICharacters.Find(c => c.name == characterName)?.GetComponent<CharacterSlotsHolder>() ?? _forUICharacters[0].GetComponent<CharacterSlotsHolder>();

        public ForUICharacterController LoadForUIController(string characterName) => _forUICharacters.Find(c => c.name == characterName)?.GetComponent<ForUICharacterController>() ?? _forUICharacters[0].GetComponent<ForUICharacterController>();

        public Sprite LoadSprite(string spriteName) => _avatars.Find(s => s.name == spriteName) ?? _avatars[0];
        
        public async UniTask<GameObject> LoadItemPrefabAsync(string itemName)
        {
            Debug.Log("Prefab to load = " + itemName);

            var handle = Addressables.LoadAssetAsync<GameObject>(itemName);
            AddResourceOperation(handle);

            await handle.Task;
            AddWeaponPrefab(handle.Result);

            return handle.Result;
        }
        
        public async UniTask<Sprite> LoadItemSpriteAsync(string itemID)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>("ItemIcon" + itemID);
            ProgressDataLoaderUI.Instance.UpdateLoadingStatus("Loading Items: " + itemID);
            await handle.Task;
            _resourcesInWorkHandles.Add(handle);
            _itemSprites.Add(handle.Result);

            return handle.Result;
        }
    }
}

