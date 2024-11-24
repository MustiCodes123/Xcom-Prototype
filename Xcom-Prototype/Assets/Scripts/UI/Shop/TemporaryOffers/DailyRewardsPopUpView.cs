using DG.Tweening;
using UnityEngine;
using PlayFab;
using PlayFab.EconomyModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class DailyRewardsPopUpView : MonoBehaviour
{
    [SerializeField] private Transform _rewardsContainer;
    [SerializeField] private ScheduleRewardItemView _rewardItemView;

    private static Dictionary<string, CatalogItem> s_cachedBundles = new Dictionary<string, CatalogItem>();

    public async void Initialize(DailyOfferRewardData rewardData)
    {
        ClearRewards();
        await FetchRewardsAsync(rewardData);

        return;
    }

    private async Task FetchRewardsAsync(DailyOfferRewardData rewardData)
    {
        for (int i = 0; i < rewardData.Days.Count; i++)
        {
            var day = rewardData.Days[i];
            var request = new GetItemRequest
            {
                Id = day.BundleID
            };
            var bundle = await GetItemAsync(request);
            if (bundle.Type == "bundle")
            {
                Dictionary<string, (Image, int)> items = new Dictionary<string, (Image, int)>();
                foreach (var itemReference in bundle.ItemReferences)
                {
                    var getItemRequest = new GetItemRequest
                    {
                        Id = itemReference.Id
                    };
                    CatalogItem item = await GetItemAsync(getItemRequest);
                    Image image = item.Images.FirstOrDefault();
                    int amount = (int)itemReference.Amount;

                    if (items.ContainsKey(item.Id))
                    {
                        var (existingImage, existingAmount) = items[item.Id];
                        items[item.Id] = (existingImage, existingAmount + amount);
                    }
                    else
                    {
                        items[item.Id] = (image, amount);
                    }
                }

                if(_rewardItemView != null)
                {
                    var rewardObject = Instantiate(_rewardItemView, _rewardsContainer);
                    _rewardItemView = rewardObject.GetComponent<ScheduleRewardItemView>();
                    await _rewardItemView.DisplayRewards(day.Day, items);
                }

                Debug.Log($"Day {day.Day}: Added {items.Count} unique items");
            }
            else
            {
                Debug.LogError($"Item {day.BundleID} is not a bundle.");
            }
        }
    }

    private async Task<CatalogItem> GetItemAsync(GetItemRequest request)
    {
        if (s_cachedBundles.ContainsKey(request.Id))
        {
            return s_cachedBundles[request.Id];
        }

        TaskCompletionSource<CatalogItem> tcs = new TaskCompletionSource<CatalogItem>();

        PlayFabEconomyAPI.GetItem(request, result =>
        {
            CatalogItem item = result.Item;
            s_cachedBundles[request.Id] = item;
            tcs.SetResult(item);
        },
        error =>
        {
            tcs.SetException(new PlayFabException(PlayFabExceptionCode.BuildError, error.GenerateErrorReport()));
        });

        return await tcs.Task;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _rewardsContainer.transform.DOScale(1, 0.3f).From(0.8f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ClearRewards()
    {
        foreach (Transform child in _rewardsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public static void ClearBundleCache()
    {
        s_cachedBundles.Clear();
    }
}