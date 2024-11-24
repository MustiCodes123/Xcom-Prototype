using Cysharp.Threading.Tasks;

namespace Data.Resources.AddressableManagement.Interfaces
{
    public interface IAddressableGroupsLoader
    {
        UniTask LoadAllResourcesForModeAsync();
        UniTask LoadMainMenuResourcesAsync();
    }
}