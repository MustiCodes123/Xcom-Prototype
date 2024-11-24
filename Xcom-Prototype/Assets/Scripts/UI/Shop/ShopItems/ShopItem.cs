using UnityEngine;
using PlayFab.EconomyModels;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine.Purchasing;

public class ShopItem : MonoBehaviour
{
    public event Action<ShopItem> TryPurchase;

    [SerializeField] protected TMP_Text ItemNameText;
    [SerializeField] public TMP_Text ItemPriceText;
    [SerializeField] protected Button PurchaseButton;
    [SerializeField] protected UnityEngine.UI.Image Icon;

    protected ShopPresenter Presenter;
    protected Wallet Wallet;
    protected IShopModelDataProvider DataProvider;

    public CatalogItem CatalogItem { get; set; }

    #region Initialization
    public virtual void Initialize(CatalogItem item, Wallet wallet, IShopModelDataProvider dataProvider, ShopPresenter presenter)
    {     
        InitializeBase(item, wallet, dataProvider, presenter);
    }

    protected virtual async void InitializeBase(CatalogItem item, Wallet wallet, IShopModelDataProvider dataProvider, ShopPresenter presenter)
    {
        Wallet = wallet;
        Presenter = presenter;
        CatalogItem = item;
        DataProvider = dataProvider;

        await SetBaseView(item);
    }
    #endregion

    #region View
    protected async UniTask DownloadImageFromUrl(string url)
    {
        ShopLocalDatabase localDatabase = new ShopLocalDatabase(Application.persistentDataPath);

        await Presenter.ShowLoadingScreen(async () =>
        {
            Icon.sprite = await localDatabase.GetOrDownloadImage(url);

            Icon.color = new Color(1f, 1f, 1f, 1f);
        });
    }

    private async Task SetBaseView(CatalogItem item)
    {
        string priceText = "";

        CatalogPriceAmount price = item.PriceOptions?.Prices?.LastOrDefault()?.Amounts?.LastOrDefault();
        
        if (price != null) 
        {
            string currency = ShopHelper.ConvertItemIDToCurrencyCode(price.ItemId).ToString();
            priceText = $"{price.Amount} {currency}";
        }

        string googlePlayId = item.AlternateIds.FirstOrDefault(id => id.Type == "GooglePlay")?.Value;

        if (!string.IsNullOrEmpty(googlePlayId))
        {
            Product product = IAPManager.Instance.GetProduct(googlePlayId);
            priceText = product.metadata.localizedPriceString;
        }

        if (string.IsNullOrWhiteSpace(priceText))
        {
            Debug.LogError($"Item: {item.Id} priceText is empty.");
            gameObject.SetActive(false); 
            return;
        }

        ItemNameText.text = item.Title["NEUTRAL"].ToString();
        ItemPriceText.text = priceText;

        if(item.Images != null)
        {
            foreach (var image in item.Images)
            {
                if(image.Url != null)
                {
                    string imageURL = image.Url;
                    await DownloadImageFromUrl(imageURL);
                }
            }
        }

    }
    #endregion

    #region Events
    protected virtual void OnTryPurchase()
    {
        TryPurchase?.Invoke(this);
    }
    #endregion
}

[Serializable]
public class ShopItemRewardData
{
    public string ItemReferenceID { get; set; }
    public int Amount { get; set; }
}