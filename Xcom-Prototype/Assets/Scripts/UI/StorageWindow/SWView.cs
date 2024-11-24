using System;
using System.Collections.Generic;
using UnityEngine;

public class SWView : UIWindowView
{
    [SerializeField] private SWItem _prefab;

    [SerializeField] private SWCategoryView _allCategoryView;
    [SerializeField] private SWCategoryView _charactersCategoryView;
    [SerializeField] private SWCategoryView _itemsCategoryView;
    [SerializeField] private SWCategoryView _giftsCategoryView;

    [SerializeField] private SWCategoryButton _allButton;
    [SerializeField] private SWCategoryButton _charactersButton;
    [SerializeField] private SWCategoryButton _itemsButton;
    [SerializeField] private SWCategoryButton _giftsButton;

    [SerializeField] private SlotsExtensionStoragePopUp _noFreeInventorySlotsPopup;

    private List<SWCategoryView> _categoryViews = new();
    private List<SWItem> _allItems = new();
    private List<SWItem> _characterItems = new();
    private List<SWItem> _inventoryItems = new();
    private List<SWItem> _giftItems = new();

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _categoryViews = new List<SWCategoryView> { _allCategoryView, _charactersCategoryView, _itemsCategoryView, _giftsCategoryView };

        foreach (var categoryView in _categoryViews)
        {
            categoryView.CategoryButton.Button.onClick.AddListener(() => ShowCategory(categoryView));
        }
    }

    private void OnDisable()
    {
        foreach (var categoryView in _categoryViews)
        {
            categoryView.CategoryButton.Button.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region Overrided Methods
    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }
    #endregion

    #region View
    private void ShowCategory(SWCategoryView categoryView)
    {
        foreach (var view in _categoryViews)
        {
            if (view == categoryView)
                view.Show();
            else
                view.Hide();
        }
    }

    public void ShowNoFreeSlotsPopup(int extensionIndex, SlotsExtensionSettings slotsExtensionSettings, ExtensionType extensionType)
    {
        _noFreeInventorySlotsPopup.Show(extensionIndex, slotsExtensionSettings, extensionType);
    }

    public List<SWItem> DisplayItems(List<IStorageItemData> dataList)
    {
        _allItems = new List<SWItem>();
        _characterItems = new List<SWItem>();
        _inventoryItems = new List<SWItem>();
        _giftItems = new List<SWItem>();

        foreach (IStorageItemData data in dataList)
        {
            string ID = Guid.NewGuid().ToString();

            SpawnItem(_allCategoryView.ItemsContainer, data, _allItems, ID);

            if (data is BaseCharacterModel)
                SpawnItem(_charactersCategoryView.ItemsContainer, data, _characterItems, ID);
            else if (data is BaseItem)
                SpawnItem(_itemsCategoryView.ItemsContainer, data, _inventoryItems, ID);
            // TODO: Add logic for gifts category
        }

        List<SWItem> allSpawnedItems = new List<SWItem>();

        allSpawnedItems.AddRange(_allItems);
        allSpawnedItems.AddRange(_characterItems);
        allSpawnedItems.AddRange(_inventoryItems);
        allSpawnedItems.AddRange(_giftItems);

        return allSpawnedItems;
    }

    public void RemoveAllItems()
    {
        ClearContainer(_allCategoryView.ItemsContainer);
        ClearContainer(_charactersCategoryView.ItemsContainer);
        ClearContainer(_itemsCategoryView.ItemsContainer);
        ClearContainer(_giftsCategoryView.ItemsContainer);
    }

    public void RemoveSingleItem(SWItem itemToRemove)
    {
        RemoveItemFromListAndContainer(itemToRemove.ID, _allItems);
        RemoveItemFromListAndContainer(itemToRemove.ID, _characterItems);
        RemoveItemFromListAndContainer(itemToRemove.ID, _inventoryItems);
        RemoveItemFromListAndContainer(itemToRemove.ID, _giftItems);
    }
    #endregion

    #region Utility Methods
    private void ClearContainer(Transform container)
    {
        foreach (var child in container.GetComponentsInChildren<SWItem>())
        {
            Destroy(child.gameObject);
        }
    }

    private void RemoveItemFromListAndContainer(string itemId, List<SWItem> itemList)
    {
        SWItem itemToRemove = itemList.Find(item => item.ID == itemId);

        if (itemToRemove != null)
        {
            Destroy(itemToRemove.gameObject);
            itemList.Remove(itemToRemove);
        }
    }
    private void SpawnItem(Transform container, IStorageItemData data, List<SWItem> itemList, string ID)
    {
        SWItem item = Instantiate(_prefab, container);
        item.Initialize(data, ID, _resourceManager);
        itemList.Add(item);
    }
    #endregion
}