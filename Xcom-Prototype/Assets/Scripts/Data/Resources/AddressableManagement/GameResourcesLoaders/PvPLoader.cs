using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Data.Resources.AddressableManagement.GameResourcesLoaders
{
    public class PvPLoader : BattleSceneLoader
    {
        [Inject]
        public PvPLoader(AddressableGroupsDataContainer addressableDataContainer, TemploaryInfo temploaryInfo, DataLoadingProgressTracker progressTracker)
            : base(addressableDataContainer, temploaryInfo, progressTracker)
        {
        }

        public async UniTask LoadPvPResourcesAsync()
        {
            await ProgressTracker.LoadWithProgress(LoadPlayerCharactersPrefabsAsync);
            await ProgressTracker.LoadWithProgress(LoadFakeLeaderCharactersAsync);
            await ProgressTracker.LoadWithProgress(LoadAllDecalesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedProjectilesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedParticlesAsync);
        }

        public async UniTask LoadFakeLeaderCharactersAsync()
        {
            var pvpEnemy = _temploaryInfo.FakeLeader.Characters;

            foreach (var enemy in pvpEnemy)
            {
                await LoadAsyncOperationHandleAddressable<GameObject>(enemy.CharacterPreset.name, AddressableDataContainer.AddCharactersPrefabs,"Loading opponents team...");
            }
        }
    }
}