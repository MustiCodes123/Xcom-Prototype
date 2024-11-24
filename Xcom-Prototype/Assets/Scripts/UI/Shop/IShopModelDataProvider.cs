using System.Collections.Generic;
using PlayFab.EconomyModels;

public interface IShopModelDataProvider
{
    public CatalogItem GetItemById(string itemId);
    public List<CatalogItem> GetBundles(WindowType windowType);
    public List<CatalogItem> GetItemsFromBundle(string bundleId);
    public Dictionary<WindowType, List<CatalogItem>> GetBundlesDictionary();
    public Dictionary<string, List<CatalogItem>> GetItemsFromBundlesDictionary();
    public List<string> GetBundleIdsForWindowType(WindowType windowType);
}
