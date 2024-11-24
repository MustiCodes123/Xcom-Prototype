using Cysharp.Threading.Tasks;

namespace Data.Resources.AddressableManagement.Interfaces
{
    public interface ISceneLoader
    {
        string GetActiveSceneName();
        void ShowLoadingScreen();
        UniTask LoadMainMenuSceneAsync();
        UniTask LoadLevelAsync(string levelName);
    }
}