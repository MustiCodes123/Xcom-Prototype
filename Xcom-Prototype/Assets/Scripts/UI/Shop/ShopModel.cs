using PlayFab.EconomyModels;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ShopModel : MonoBehaviour
{
    [Inject] private ShopRepository _shopRepository;

    public Dictionary<WindowType, List<CatalogItem>> Bundles { get; private set; }
    public Dictionary<string, List<CatalogItem>> ItemsFromBundles { get; private set; }
    public Dictionary<string, Sprite> DownloadedImages { get; private set; }

    public void Initialize()
    {
        Bundles = new Dictionary<WindowType, List<CatalogItem>>(_shopRepository.Bundles);
        ItemsFromBundles = new Dictionary<string, List<CatalogItem>>(_shopRepository.ItemsFromBundles);
        DownloadedImages = new Dictionary<string, Sprite>(_shopRepository.DownloadedImages);
    }
}