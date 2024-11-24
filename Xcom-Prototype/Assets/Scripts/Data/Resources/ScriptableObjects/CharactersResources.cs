using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Data.Resources.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CharactersResources", menuName = "Data/ResourceManagement/CharactersResources")]
    public class CharactersResources : ScriptableObject
    {
        [SerializeField] private List<AssetReferenceGameObject> _forUICharacters;
        [SerializeField] private List<AssetReferenceGameObject> _forUIBackgrounds;
        [SerializeField] private List<AssetReferenceGameObject> _charactersPrefabs;
        [SerializeField] private EnemyResources _enemiesPrefabs;

        public List<AssetReferenceGameObject> GetForUICharacters()
        {
            return _forUICharacters;
        }
        public List<AssetReferenceGameObject> GetForUIBackgrounds()
        {
            return _forUIBackgrounds;
        }
        public List<AssetReferenceGameObject> GetCharactersPrefabs()
        {
            return _charactersPrefabs;
        }

        public List<AssetReferenceGameObject>GetAllEnemiesPrefabs()
        {
            List<AssetReferenceGameObject> result = new List<AssetReferenceGameObject>();
            result.AddRange(_enemiesPrefabs.GoblinsPrefabs);
            result.AddRange(_enemiesPrefabs.DemonsPrefabs);
            result.AddRange(_enemiesPrefabs.DemonsPrefabs);
            result.AddRange(_enemiesPrefabs.HumanPrefabs);
            return result;
        }
        public List<AssetReferenceGameObject> GetEnemiesPrefabsByRace(CharacterRace enemyRace)
        {
            switch(enemyRace)
            {
                case CharacterRace.Goblin: 
                    return _enemiesPrefabs.GoblinsPrefabs;
                case CharacterRace.Skeleton:
                    return _enemiesPrefabs.SkeletonsPrefabs;
                case CharacterRace.Demon:
                    return _enemiesPrefabs.DemonsPrefabs;
                case CharacterRace.Human:
                    return _enemiesPrefabs.HumanPrefabs;
                default:
                    return GetAllEnemiesPrefabs();
            }
        }


        [Serializable]
        public struct EnemyResources
        {
            public List<AssetReferenceGameObject> GoblinsPrefabs;
            public List<AssetReferenceGameObject> SkeletonsPrefabs;
            public List<AssetReferenceGameObject> DemonsPrefabs;
            public List<AssetReferenceGameObject> HumanPrefabs;
        }
    }
}