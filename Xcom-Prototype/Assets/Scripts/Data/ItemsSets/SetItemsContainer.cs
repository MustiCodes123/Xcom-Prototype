using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

#region Enumerations
public enum ItemsSetRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythical
};
#endregion

[CreateAssetMenu(fileName = "SetItemsContainer", menuName = "Data/Items/ItemsSet")]
public class SetItemsContainer : ScriptableObject
{
    public static SetItemsContainer Instance;

    [field: SerializeField] public ShieldSet[] ShieldSets { get; private set; }
    [field: SerializeField] public PairWeaponsSet[] PairWeaponsSets { get; private set; }
    [field: SerializeField] public TwoHandsWeaponsSet[] TwoHandsWeaponsSets{ get; private set; }

    [Inject]
    public SetItemsContainer()
    {
        Instance = this;
    }

    private void OnValidate()
    {
        foreach(ShieldSet item in ShieldSets)
        {
            AddSkillSetToItems(item);
        }

        foreach (PairWeaponsSet item in PairWeaponsSets)
        {
            AddSkillSetToItems(item);
        }

        foreach (TwoHandsWeaponsSet item in TwoHandsWeaponsSets)
        {
            AddSkillSetToItems(item);
        }
    }

    public BaseItemsSet GetSet(string playFabID)
    {
        BaseItemsSet foundedSet = FindSetByID(ShieldSets, playFabID) ??
                                  FindSetByID(PairWeaponsSets, playFabID) ??
                                  FindSetByID(TwoHandsWeaponsSets, playFabID);

        if(foundedSet != null)
        {
            return foundedSet;
        }
        else
        {
            Debug.LogError($"SetItemsContainer does not contain set with ID {playFabID}");
            return null;
        }
            
    }

    public List<ItemTemplate> GetItemsFromSet(BaseItemsSet set)
    {
        List<ItemTemplate> items = new List<ItemTemplate>
        {
            set.Helmet,
            set.Chest,
            set.Boots,
            set.Gloves
        };

        if (set is ShieldSet shieldSet)
        {
            items.Add(shieldSet.WeaponOH);
            items.Add(shieldSet.Shield);
        }
        else if (set is PairWeaponsSet pairWeaponsSet)
        {
            items.Add(pairWeaponsSet.FirstWeaponOH);
            items.Add(pairWeaponsSet.SecondWeaponOH);
        }
        else if (set is TwoHandsWeaponsSet twoHandsSet)
        {
            items.Add(twoHandsSet.WeaponTH);
        }

        else
        {
            Debug.LogError($"Can't cast set ID: {set.PlayFabID}");
        }

        return items;
    }

    public BaseItemsSet GetRandomSetByRarity(ItemsSetRarity rarity)
    {
        BaseItemsSet[] allSets = ShieldSets.Concat<BaseItemsSet>(PairWeaponsSets).Concat(TwoHandsWeaponsSets).ToArray();
        BaseItemsSet[] setsWithRarity = allSets.Where(set => set.SetRarity == rarity).ToArray();

        if (setsWithRarity.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, setsWithRarity.Length);
            return setsWithRarity[randomIndex];
        }
        else
        {
            Debug.LogWarning($"No sets found with rarity {rarity}. Returning null.");
            return null;
        }
    }

    private BaseItemsSet FindSetByID<T>(T[] setArray, string playFabID) where T : BaseItemsSet
    {
        return setArray.FirstOrDefault(item => item.PlayFabID == playFabID);
    }

    private void AddSkillSetToItems(BaseItemsSet itemsSet)
    {
        if (itemsSet.SkillSet != null)
        {
            if (!itemsSet.Helmet.ItemSkillSets.Contains(itemsSet.SkillSet))
                itemsSet.Helmet.ItemSkillSets.Add(itemsSet.SkillSet);

            if (!itemsSet.Chest.ItemSkillSets.Contains(itemsSet.SkillSet))
                itemsSet.Chest.ItemSkillSets.Add(itemsSet.SkillSet);

            if (!itemsSet.Boots.ItemSkillSets.Contains(itemsSet.SkillSet))
                itemsSet.Boots.ItemSkillSets.Add(itemsSet.SkillSet);

            if (!itemsSet.Gloves.ItemSkillSets.Contains(itemsSet.SkillSet))
                itemsSet.Gloves.ItemSkillSets.Add(itemsSet.SkillSet);
        }

        switch (itemsSet)
        {
            case ShieldSet shieldSet:
                if (!shieldSet.Shield.ItemSkillSets.Contains(shieldSet.SkillSet))
                    shieldSet.Shield.ItemSkillSets.Add(shieldSet.SkillSet);

                if (!shieldSet.WeaponOH.ItemSkillSets.Contains(shieldSet.SkillSet))
                    shieldSet.WeaponOH.ItemSkillSets.Add(shieldSet.SkillSet);

                break;

            case PairWeaponsSet pairWeaponSet:
                if (!pairWeaponSet.FirstWeaponOH.ItemSkillSets.Contains(pairWeaponSet.SkillSet))
                    pairWeaponSet.FirstWeaponOH.ItemSkillSets.Add(pairWeaponSet.SkillSet);

                if (!pairWeaponSet.SecondWeaponOH.ItemSkillSets.Contains(pairWeaponSet.SkillSet))
                    pairWeaponSet.SecondWeaponOH.ItemSkillSets.Add(pairWeaponSet.SkillSet);

                break;

            case TwoHandsWeaponsSet twoHandsSet:
                if (!twoHandsSet.WeaponTH.ItemSkillSets.Contains(twoHandsSet.SkillSet))
                    twoHandsSet.WeaponTH.ItemSkillSets.Add(twoHandsSet.SkillSet);

                break;
        }
    }

    public List<BaseItemsSet> GetAllSetsContainingItem<T>(T template) where T : ItemTemplate
    {
        List<BaseItemsSet> sets = new List<BaseItemsSet>();

        sets.AddRange(ShieldSets
            .Where(set => set.Boots == template || set.Helmet == template || set.Gloves == template || set.Chest == template || set.Shield == template || set.WeaponOH == template));
        sets.AddRange(PairWeaponsSets
            .Where(set => set.Boots == template || set.Helmet == template || set.Gloves == template || set.Chest == template || set.FirstWeaponOH == template || set.SecondWeaponOH == template));
        sets.AddRange(TwoHandsWeaponsSets
            .Where(set => set.Boots == template || set.Helmet == template || set.Gloves == template || set.Chest == template || set.WeaponTH == template));

        return sets;
    }
}

#region Single Set Classes
[System.Serializable]
public class BaseItemsSet
{
    [field: Header("Properties")]
    [field: Space(5)]
    [field: SerializeField] public string PlayFabID { get; private set; }
    [field: SerializeField] public string SetName { get; private set; }
    [field: SerializeField] public ItemsSetRarity SetRarity { get; private set; }
    [field: SerializeField] public ItemSkillSet SkillSet { get; private set; }

    [field: Space(15)]
    [field: Header("Items")]
    [field: Space(5)]
    [field: SerializeField] public ItemTemplate Helmet { get; private set; }
    [field: SerializeField] public ItemTemplate Chest { get; private set; }
    [field: SerializeField] public ItemTemplate Boots { get; private set; }
    [field: SerializeField] public ItemTemplate Gloves { get; private set; }

    [field: Space(15)]
    [field: Header("Full Set Bonuses")]
    [field: Space(5)]
    [field: SerializeField] public StatsBonus StatsBonus { get; private set; }
}

[System.Serializable]
public class ShieldSet : BaseItemsSet
{
    [field: SerializeField] public WeaponItemTemaplate WeaponOH { get; private set; }
    [field: SerializeField] public ItemTemplate Shield { get; private set; }
}

[System.Serializable]
public class PairWeaponsSet : BaseItemsSet
{
    [field: SerializeField] public WeaponItemTemaplate FirstWeaponOH { get; private set; }
    [field: SerializeField] public WeaponItemTemaplate SecondWeaponOH { get; private set; }
}

[System.Serializable]
public class TwoHandsWeaponsSet : BaseItemsSet
{
    [field: SerializeField] public WeaponItemTemaplate WeaponTH { get; private set; }
}
#endregion

[System.Serializable]
public class StatsBonus
{
    [field: SerializeField] public float PhysicalDamage { get; private set; }
    [field: SerializeField] public float MagicDamage { get; private set; }
    [field: SerializeField] public float HealthPoints { get; private set; }
    [field: SerializeField] public float PhysicalResistance { get; private set; }
    [field: SerializeField] public float MagicalResistance { get; private set; }
    [field: SerializeField] public float CritDamage { get; private set; }
    [field: SerializeField] public float CritChance { get; private set; }
    [field: SerializeField] public float DodgeChance { get; private set; }
}