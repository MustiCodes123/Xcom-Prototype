using System.Collections.Generic;
using PlayFab.EconomyModels;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ShopModelData : IShopModelDataProvider
{
    private readonly ShopModel _model;

    public ShopModelData(ShopModel model) => _model = model;

    public CatalogItem GetItemById(string itemId)
    {
        return _model.ItemsFromBundles.SelectMany(x => x.Value).FirstOrDefault(x => x.Id == itemId);
    }

    public List<CatalogItem> GetBundles(WindowType windowType)
    {
        return _model.Bundles.TryGetValue(windowType, out var items) ? items : new List<CatalogItem>();
    }

    public List<CatalogItem> GetItemsFromBundle(string bundleId)
    {
        return _model.ItemsFromBundles[bundleId];
    }

    public Dictionary<WindowType, List<CatalogItem>> GetBundlesDictionary()
    {
        return _model.Bundles;
    }

    public Dictionary<string, List<CatalogItem>> GetItemsFromBundlesDictionary()
    {
        return _model.ItemsFromBundles;
    }

    public List<string> GetBundleIdsForWindowType(WindowType windowType)
    {
        List<CatalogItem> bundles = _model.Bundles.TryGetValue(windowType, out var items) ? items : new List<CatalogItem>();

        return bundles.Select(b => b.Id).ToList();
    }
}