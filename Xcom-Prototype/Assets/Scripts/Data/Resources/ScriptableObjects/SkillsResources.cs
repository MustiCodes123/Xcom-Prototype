using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Data.Resources.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SkillsResources", menuName = "Data/ResourceManagement/SkillsResources")]
    public class SkillsResources : ScriptableObject
    {
        [SerializeField] private List<AssetReferenceGameObject> _decalesPrefabs;
        [SerializeField] private List<AssetReferenceGameObject> _particlesPrefabs;
        [SerializeField] private List<AssetReferenceGameObject> _projectilesPrefabs;
        [SerializeField] private List<AssetReferenceGameObject> _alwaysInUseBuffParticles;

        public List<AssetReferenceGameObject> GetDecales()
        {
            return _decalesPrefabs;
        }
        public List<AssetReferenceGameObject> GetParticles()
        {
            return _particlesPrefabs;
        }
        public List<AssetReferenceGameObject> GetProjectiles()
        {
            return _projectilesPrefabs;
        }

        public List<AssetReferenceGameObject> GetBuffParticles()
        {
            return _alwaysInUseBuffParticles;   
        }
    }
}