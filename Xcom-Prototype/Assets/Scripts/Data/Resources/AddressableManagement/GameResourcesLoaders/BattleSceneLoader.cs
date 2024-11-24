using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Data.Resources.AddressableManagement.GameResourcesLoaders
{
    public class BattleSceneLoader : BaseLoader
    {
        [Inject]
        public BattleSceneLoader(AddressableGroupsDataContainer addressableDataContainer, TemploaryInfo temploaryInfo, DataLoadingProgressTracker progressTracker) : base(addressableDataContainer, temploaryInfo, progressTracker)
        {
        }

        protected async UniTask LoadPlayerCharactersPrefabsAsync()
        {
            var charactersToLoad = _temploaryInfo.SelectedCharacters;
            await LoadAsyncOperationHandleAddressable<GameObject>(charactersToLoad.Select(c => c.Name), AddressableDataContainer.AddCharactersPrefabs, "Loading your Characters...");
        }

        protected async UniTask LoadAllDecalesAsync()
        {
            var addressableDecales = AddressableDataContainer.SkillResources.GetDecales();
            await LoadAsyncOperationHandleAddressable<GameObject>(addressableDecales, AddressableDataContainer.AddDecales,"Loading decals...");
        }

        protected async UniTask LoadAssignedProjectilesAsync()
        {
            var projectiles = GetAllSkillsInUse(GetAllStageCharacters())
                .Where(s => s.ProjectileType != ProjectileType.None)
                .Select(s => s.ProjectileType.ToString())
                .Distinct();

            await LoadAsyncOperationHandleAddressable<GameObject>(projectiles, AddressableDataContainer.AddProjectiles,"Loading projectiles...");
        }

        protected async UniTask LoadAssignedParticlesAsync()
        {
            var particles = GetAllSkillsInUse(GetAllStageCharacters())
                .SelectMany(s => new[] { s.ParticleType, s.OnCollisionParticle })
                .Where(p => p != ParticleType.None)
                .Select(p => p.ToString())
                .Distinct();

            await LoadAsyncOperationHandleAddressable<GameObject>(particles, AddressableDataContainer.AddParticles,"Loading particles...");
        }
    
        protected virtual List<BaseCharacterModel> GetAllStageCharacters()
        {
            var result = new List<BaseCharacterModel>();
            foreach (var character in _temploaryInfo.SelectedCharacters)
            {
                result.Add(character);
            }

            return result;
        }

        protected virtual List<BaseSkillModel> GetAllSkillsInUse(List<BaseCharacterModel> characters)
        {
            var result = new List<BaseSkillModel>();

            for (int i = 0; i < characters.Count; i++)
            {
                var character = characters[i];

                for (int j = 0; j < character.SkillsInUse.Count; j++)
                {
                    result.Add(character.SkillsInUse[j]);
                }
            }

            return result;
        }
    }
}