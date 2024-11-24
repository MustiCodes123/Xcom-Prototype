using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Data.Resources.AddressableManagement.GameResourcesLoaders
{
    public class TestOfStrengthLoader : BattleSceneLoader
    {
        [Inject]
        public TestOfStrengthLoader(AddressableGroupsDataContainer addressableDataContainer, TemploaryInfo temploaryInfo, DataLoadingProgressTracker progressTracker) : base(addressableDataContainer, temploaryInfo, progressTracker)
        {
        }

        public async UniTask LoadTOSResourcesAsync()
        {
            await ProgressTracker.LoadWithProgress(LoadPlayerCharactersPrefabsAsync);
            await ProgressTracker.LoadWithProgress(LoadBossCharacterAsync);
            await ProgressTracker.LoadWithProgress(LoadAllDecalesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedProjectilesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedParticlesAsync);
        }

        private async UniTask LoadBossCharacterAsync()
        {
            var bossToLoad = _temploaryInfo.CurrentBoss;
            
            await LoadAsyncOperationHandleAddressable<GameObject>(bossToLoad.BossPreset.PresetName, AddressableDataContainer.AddCharactersPrefabs,"Loading boss...");
         

            var charactersToLoad = GetAllStageCharacters();

            foreach (var character in charactersToLoad)
            {
                await LoadAsyncOperationHandleAddressable<GameObject>(character.Name, AddressableDataContainer.AddCharactersPrefabs,"Loading friendly characters...");
            }
        }

        protected override List<BaseCharacterModel> GetAllStageCharacters()
        {
            var result = _temploaryInfo.SelectedCharacters.ToList();

            for (int i = 0; i < _temploaryInfo.SelectedCharacterGroups.Count; i++)
            {
                foreach (var character in _temploaryInfo.SelectedCharacterGroups[i])
                {
                    {
                        result.Add(character);
                    }
                }
            }

            return result;
        }
    }
}