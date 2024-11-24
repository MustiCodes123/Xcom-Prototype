using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class ScheduleRewardItemView : MonoBehaviour
{
    [SerializeField] private ScheduleItem _prefab;
    [SerializeField] private Transform _root;
    [SerializeField] private TMP_Text _dayCount;

    public async Task DisplayRewards(int day, Dictionary<string, (PlayFab.EconomyModels.Image, int)> items)
    {
        _dayCount.text = $"Day {day}";

        foreach (Transform child in _root)
        {
            Destroy(child.gameObject);
        }

        foreach (var kvp in items)
        {
            var (image, amount) = kvp.Value;

            if (image != null)
            {
                var item = Instantiate(_prefab, _root);
                string url = image.Url;
                await DownloadImageFromUrl(url, item);
                item.Amount.text = amount.ToString();
            }
            else
            {
                Debug.LogError($"Image is null for item with ID: {kvp.Key}");
            }
        }
    }

    protected async UniTask DownloadImageFromUrl(string url, ScheduleItem item)
    {
        ShopLocalDatabase localDatabase = new ShopLocalDatabase(Application.persistentDataPath);

        if(item.Image != null)
        {
            item.Image.sprite = await localDatabase.GetOrDownloadImage(url);
            item.Image.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}