using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class ShopLocalDatabase : MonoBehaviour
{
    private readonly string _localDataPath;

    public ShopLocalDatabase(string localDataPath)
    {
        _localDataPath = localDataPath;
    }

    #region Public Methods
    public void ClearLocalData()
    {
        DirectoryInfo directory = new DirectoryInfo(_localDataPath);
        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();

            Debug.Log($"File {file.FullName} was deleted succesfuly");
        }
    }

    public async UniTask<Sprite> GetOrDownloadImage(string url)
    {
        string fileName = Path.GetFileName(url);
        string filePath = Path.Combine(_localDataPath, fileName);

        if (File.Exists(filePath))
        {
            byte[] bytes = await File.ReadAllBytesAsync(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            return sprite;
        }
        else
        {
            using (UnityWebRequest requestOperation = UnityWebRequestTexture.GetTexture(url))
            {
                await requestOperation.SendWebRequest();
                if (requestOperation.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(requestOperation);
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    await SaveImageLocally(url, texture);
                    return sprite;
                }
                else
                {
                    Debug.LogError($"Error downloading image from {url}: {requestOperation.error}");
                    return null;
                }
            }
        }
    }
    #endregion

    #region Utility Methods
    private async UniTask SaveImageLocally(string url, Texture2D texture)
    {
        string fileName = Path.GetFileName(url);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        byte[] bytes = texture.EncodeToPNG();
        await File.WriteAllBytesAsync(filePath, bytes);

    }
    #endregion
}