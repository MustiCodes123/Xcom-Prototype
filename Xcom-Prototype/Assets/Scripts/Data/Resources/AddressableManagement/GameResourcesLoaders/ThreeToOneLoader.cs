using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Data.Resources.AddressableManagement.GameResourcesLoaders
{
    public class ThreeToOneLoader : BattleSceneLoader
    {
        public ThreeToOneLoader(AddressableGroupsDataContainer addressableDataContainer, TemploaryInfo temploaryInfo, DataLoadingProgressTracker progressTracker) : base(addressableDataContainer, temploaryInfo, progressTracker)
        {
            
        }

        public async UniTask LoadThreeToOneResourcesAsync()
        {
            await ProgressTracker.LoadWithProgress(LoadPlayerCharactersPrefabsAsync);
            await ProgressTracker.LoadWithProgress(LoadBossCharacterAsync);
            await ProgressTracker.LoadWithProgress(LoadFakeTeamMemberCharacters);
            await ProgressTracker.LoadWithProgress(LoadAllDecalesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedProjectilesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedParticlesAsync);
        }

        private async UniTask LoadBossCharacterAsync()
        {
            var charactersToLoad = _temploaryInfo.CurrentBoss;

            await LoadAsyncOperationHandleAddressable<GameObject>(charactersToLoad.BossPreset.PresetName, AddressableDataContainer.AddCharactersPrefabs,"Loading boss...");

        }

        private async UniTask LoadFakeTeamMemberCharacters()
        {
            var teamCharacters = new List<EnemyCharacter>();

            foreach (var teamMember in _temploaryInfo.FakeTeamMembers)
            {
                teamCharacters.AddRange(teamMember.Characters);
            }

            foreach (var enemy in teamCharacters)
            {
                await LoadAsyncOperationHandleAddressable<GameObject>(enemy.CharacterPreset.name, AddressableDataContainer.AddCharactersPrefabs,"Loading opponents team...");
            }
        }
    }
}