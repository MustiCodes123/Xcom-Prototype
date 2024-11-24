using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Data.Resources.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Avatars", menuName = "Data/ResourceManagement/Avatars")]
    public class AvatarResources : ScriptableObject
    {
        [SerializeField] private List<AssetReferenceSprite> _avatars;

        public List<AssetReferenceSprite> GetAvatars()
        {
            return _avatars;
        }
    }
}