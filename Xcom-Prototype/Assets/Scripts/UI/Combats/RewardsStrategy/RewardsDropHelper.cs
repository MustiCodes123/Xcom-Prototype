using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RewardsDropHelper
{
    private BaseDragableItem _prefab;
    private Transform _rewardsContainer;

    private ItemsDataInfo _itemsDataInfo;

    private PlayerData _playerData;

    public RewardsDropHelper(BaseDragableItem prefab, Transform rewardsContainer)
    {
        _prefab = prefab;
        _rewardsContainer = rewardsContainer;

        _itemsDataInfo = ItemsDataInfo.Instance;
    }

    #region Drop Methods
    public void TryToDropItems(TemploaryInfo temploaryInfo, PlayerData playerData)
    {
        _playerData = playerData;   

        foreach (var dropSettings in temploaryInfo.LevelInfo.DropSettings)
        {
            if (dropSettings.DropItemChances.Length > 0)
            {
                int randomItemIndex = Random.Range(0, dropSettings.DropItemChances.Length);
                var selectedDropItemChance = dropSettings.DropItemChances[randomItemIndex];

                float random = Random.Range(0, 100);
                RareEnum rarity = GetChanceForRarity(random, selectedDropItemChance);
                int itemLevel = Random.Range(selectedDropItemChance.MinLevel, selectedDropItemChance.MaxLevel);

                ItemTemplate itemTemplate = _itemsDataInfo.GetRandomItemTemplateOfType(selectedDropItemChance.ItemType, rarity);

                if (itemTemplate != null)
                {
                    var item = _itemsDataInfo.ConvertTemplateToItem<BaseItem>(itemTemplate, itemLevel);
                    var itemCard = GameObject.Instantiate(_prefab, _rewardsContainer);
                    itemCard.SetItem(item, null);

                    playerData.AddItemToInventory(item);
                }
            }
        }
    }

    public List<BaseItem> CalculateRewardItems(TemploaryInfo temploaryInfo)
    {
        var rewards = new List<BaseItem>();

        foreach (var dropSettings in temploaryInfo.LevelInfo.DropSettings)
        {
            if (dropSettings.DropItemChances.Length > 0)
            {
                int randomItemIndex = Random.Range(0, dropSettings.DropItemChances.Length);
                var selectedDropItemChance = dropSettings.DropItemChances[randomItemIndex];

                float random = Random.Range(0, 100);
                RareEnum rarity = GetChanceForRarity(random, selectedDropItemChance);
                int itemLevel = Random.Range(selectedDropItemChance.MinLevel, selectedDropItemChance.MaxLevel);

                ItemTemplate itemTemplate = _itemsDataInfo.GetRandomItemTemplateOfType(selectedDropItemChance.ItemType, rarity);

                if (itemTemplate != null)
                {
                    var item = _itemsDataInfo.ConvertTemplateToItem<BaseItem>(itemTemplate, itemLevel);
                    rewards.Add(item);
                }
            }
        }

        return rewards;
    }

    public void ShowItemReward(List<BaseItem> itemRewards)
    {
        foreach (var item in itemRewards)
        {
            var itemCard = GameObject.Instantiate(_prefab, _rewardsContainer);
            itemCard.SetItem(item, null);
        }
    }

    public async void TryAddCristal(TemploaryInfo temploaryInfo)
    {
        foreach (var dropSettings in temploaryInfo.LevelInfo.DropSettings)
        {
            if (dropSettings.DropCristalChances.Length > 0)
            {

                int randomCristalIndex = Random.Range(0, dropSettings.DropCristalChances.Length);
                var selectedDropCristalChance = dropSettings.DropCristalChances[randomCristalIndex];

                float random = Random.Range(0, 100);
                SummonCristalsEnum rarity = GetCristalRarityForChance(random, selectedDropCristalChance);

                CristalData cristalData = new CristalData();
                switch (rarity)
                {
                    case SummonCristalsEnum.Common:
                        cristalData = dropSettings.CommonCristal;
                        break;
                    case SummonCristalsEnum.Rare:
                        cristalData = dropSettings.RareCristal;
                        break;
                    case SummonCristalsEnum.Epic:
                        cristalData = dropSettings.EpicCristal;
                        break;
                    case SummonCristalsEnum.Legendary:
                        cristalData = dropSettings.LegendaryCristal;
                        break;
                    case SummonCristalsEnum.Mythical:
                        cristalData = dropSettings.MythicalCristal;
                        break;
                }

                if(cristalData != null)
                {
                    CreateItemCard(cristalData);
                    await PlayerInventory.Instance.AddCristalToInventory(rarity);
                }
            }
        }
    }

    private void CreateItemCard(CristalData cristalData)
    {
        var itemCard = GameObject.Instantiate(_prefab, _rewardsContainer);
        itemCard.SetCristal(cristalData);
        itemCard.transform.SetParent(_rewardsContainer);
    }

    public async void TryAddChest(TemploaryInfo temploaryInfo)
    {
        foreach (var dropSettings in temploaryInfo.LevelInfo.DropSettings)
        {
            if (dropSettings.DropChestChances.Length > 0)
            {
                int randomChestIndex = Random.Range(0, dropSettings.DropBrokenChestChances.Length);
                DropChestChance selectedDropChestChance = dropSettings.DropChestChances[randomChestIndex];

                float random = Random.Range(0, 100);

                ChestType rarity = GetChestRarityForChance(random, selectedDropChestChance);

                ChestViewData viewData = dropSettings.GetChestViewDataByRarity(rarity);

                if (dropSettings.GetChesViewData().Length > 0 && viewData != null)
                {
                    BaseDragableItem itemCard = GameObject.Instantiate(_prefab, _rewardsContainer);
                    itemCard.SetChest(viewData);

                    await PlayerInventory.Instance.AddChestToInventory(rarity);
                }
            }
        }
    }

    public async void TryAddBrokenChest(TemploaryInfo temploaryInfo)
    {
        foreach (var dropSettings in temploaryInfo.LevelInfo.DropSettings)
        {
            if (dropSettings.DropBrokenChestChances.Length > 0)
            {
                int randomChestIndex = Random.Range(0, dropSettings.DropBrokenChestChances.Length);
                var selectedDropChestChance = dropSettings.DropBrokenChestChances[randomChestIndex];

                float random = Random.Range(0, 100);

                ChestType rarity = GetBrokenChestRarityForChance(random, selectedDropChestChance);

                ChestViewData viewData = dropSettings.GetChestViewDataByRarity(rarity);

                if (viewData != null)
                {
                    var itemCard = GameObject.Instantiate(_prefab, _rewardsContainer);
                    itemCard.SetChest(viewData);

                    await PlayerInventory.Instance.AddChestToInventory(rarity);
                }
            }
        }
    }
    #endregion

    #region Chances Calculation
    private SummonCristalsEnum GetCristalRarityForChance(float random, DropCristalChance dropCristalChance)
    {
        if (random < dropCristalChance.CommonChance)
            return SummonCristalsEnum.Common;
        else if (random < dropCristalChance.CommonChance + dropCristalChance.RareChance)
            return SummonCristalsEnum.Rare;
        else if (random < dropCristalChance.CommonChance + dropCristalChance.RareChance + dropCristalChance.EpicChance)
            return SummonCristalsEnum.Epic;
        else if (random < dropCristalChance.CommonChance + dropCristalChance.RareChance + dropCristalChance.EpicChance + dropCristalChance.LegendaryChance)
            return SummonCristalsEnum.Legendary;

        return 0;
    }

    private ChestType GetBrokenChestRarityForChance(float random, DropBrokenChestChance dropBrokenChestChance)
    {
        if (random < dropBrokenChestChance.CommonChance)
            return ChestType.BrokenWooden;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance)
            return ChestType.BrokenAncient;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance + dropBrokenChestChance.EpicChance)
            return ChestType.BrokenKing;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance + dropBrokenChestChance.EpicChance + dropBrokenChestChance.LegendaryChance)
            return ChestType.BrokenHero;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance + dropBrokenChestChance.EpicChance + dropBrokenChestChance.LegendaryChance + dropBrokenChestChance.MythicalChance)
            return ChestType.BrokenGod;

        return 0;
    }

    private ChestType GetChestRarityForChance(float random, DropChestChance dropBrokenChestChance)
    {
        if (random < dropBrokenChestChance.CommonChance)
            return ChestType.Wooden;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance)
            return ChestType.Ancient;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance + dropBrokenChestChance.EpicChance)
            return ChestType.King;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance + dropBrokenChestChance.EpicChance + dropBrokenChestChance.LegendaryChance)
            return ChestType.Hero;
        else if (random < dropBrokenChestChance.CommonChance + dropBrokenChestChance.RareChance + dropBrokenChestChance.EpicChance + dropBrokenChestChance.LegendaryChance + dropBrokenChestChance.MythicalChance)
            return ChestType.God;

        return 0;
    }

    private RareEnum GetChanceForRarity(float random, DropItemChance dropItemChance)
    {
        if (random < dropItemChance.CommonChance)
            return RareEnum.Common;
        else if (random < dropItemChance.CommonChance + dropItemChance.RareChance)
            return RareEnum.Rare;
        else if (random < dropItemChance.CommonChance + dropItemChance.RareChance + dropItemChance.EpicChance)
            return RareEnum.Epic;
        else if (random < dropItemChance.CommonChance + dropItemChance.RareChance + dropItemChance.EpicChance + dropItemChance.LegendaryChance)
            return RareEnum.Legendary;

        return 0;
    }
    #endregion
}
