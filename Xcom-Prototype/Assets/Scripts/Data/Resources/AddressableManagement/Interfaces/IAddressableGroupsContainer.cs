using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data.Resources.AddressableManagement.Interfaces
{
    public interface IAddressableGroupsContainer
    {
        void ReleaseAllResources();
        void ReleaseScene(AsyncOperationHandle sceneOperation);
    }
}