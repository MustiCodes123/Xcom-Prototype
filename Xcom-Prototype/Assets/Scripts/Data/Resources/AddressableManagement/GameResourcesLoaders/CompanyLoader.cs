using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Data.Resources.AddressableManagement.GameResourcesLoaders
{
    public class CompanyLoader: BattleSceneLoader
    {
        [Inject]
        public CompanyLoader(AddressableGroupsDataContainer addressableDataContainer, TemploaryInfo temploaryInfo, DataLoadingProgressTracker progressTracker) : base(addressableDataContainer, temploaryInfo, progressTracker)
        {
        }

        public async UniTask LoadCompanyModeResourcesAsync()
        {
            await ProgressTracker.LoadWithProgress(LoadPlayerCharactersPrefabsAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedEnemiesPrefabsAsync);
            await ProgressTracker.LoadWithProgress(LoadAllDecalesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedProjectilesAsync);
            await ProgressTracker.LoadWithProgress(LoadAssignedParticlesAsync);
        }

        private async UniTask LoadAssignedEnemiesPrefabsAsync()
        {
            if (_temploaryInfo.CurrentMode.GameMode == GameMode.Default)
            {
                var firstEnemyRace = _temploaryInfo.LevelInfo.Waves[0].Enemie[0].EnemyRace;
                var addressableEnemies = AddressableDataContainer.CharactersResources.GetEnemiesPrefabsByRace(firstEnemyRace);
                
                await LoadAsyncOperationHandleAddressable<GameObject>(addressableEnemies, AddressableDataContainer.AddEnemiesPrefabs,"Loading enemies...");
            }
        }

        protected override List<BaseSkillModel> GetAllSkillsInUse(List<BaseCharacterModel> characters)
        {
            var result = new List<BaseSkillModel>();

            result.AddRange(GetPlayerCharacterSkills(characters));
            result.AddRange(GetEnemySkills());

            return result;
        }

        private IEnumerable<BaseSkillModel> GetPlayerCharacterSkills(List<BaseCharacterModel> characters)
        {
            var skills = new List<BaseSkillModel>();
    
            foreach (var character in characters)
            {
                skills.AddRange(character.SkillsInUse);
            }

            return skills;
        }

        private IEnumerable<BaseSkillModel> GetEnemySkills()
        {
            var skills = new List<BaseSkillModel>();

            foreach (var level in _temploaryInfo.CurrentStage.Levels)
            {
                foreach (var wave in level.Waves)
                {
                    foreach (var enemy in wave.Enemie)
                    {
                        var enemySkills = enemy.Stats.EnemySkills;
                        if (enemySkills != null && enemySkills.Length > 0)
                        {
                            foreach (var skill in enemySkills)
                            {
                                if (skill != null)
                                {
                                    skills.Add(skill.GetSkill());
                                }
                            }
                        }
                    }
                }
            }

            return skills;
        }
    }
}