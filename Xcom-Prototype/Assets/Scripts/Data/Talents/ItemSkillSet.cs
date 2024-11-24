using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSkillSet", menuName = "Data/Skills/ItemSkillSet")]
[System.Serializable]
public class ItemSkillSet : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] OneHandedSword{ get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] TwoHandedSword { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] OneHandedAxe { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] TwoHandedAxe { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] OneHandedHummer { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] TwoHandedHummer { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] OneHandedMace { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] TwoHandedMace { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Spear { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Bow { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Staff { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Head { get; set; }
    [field: SerializeField] public BaseSkillTemplate[] Chest { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Gloves { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Legs { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Shield { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Ring { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Amulet { get; private set; }
    [field: SerializeField] public BaseSkillTemplate[] Armlet { get; private set; }

    public BaseSkillModel GetRandomSkill(ItemTemplate item)
    {
        try
        {
            return item switch
            {
                ArmorItemTemplate armorTemplate => GetArmorRandomSkill(armorTemplate.Slot),
                WeaponItemTemaplate weaponTemplate => GetWeaponRandomSkill(weaponTemplate.weaponType),
                RingItemTemplate => GetRandomSkillFromArray(Ring),
                AmuletItemTemplate => GetRandomSkillFromArray(Amulet),
                ArmletItemTemplate => GetRandomSkillFromArray(Armlet),

                _ => throw new ArgumentException("Unknown item type")
            };
        }
        catch (InvalidOperationException ex)
        {
            Debug.LogWarning($"No skills available for item: {item.GetType().Name}. {ex.Message}");
            return null;
        }
    }

    private BaseSkillModel GetArmorRandomSkill(SlotEnum slot)
    {
        return slot switch
        {
            SlotEnum.Head => GetRandomSkillFromArray(Head),
            SlotEnum.Chest => GetRandomSkillFromArray(Chest),
            SlotEnum.Gloves => GetRandomSkillFromArray(Gloves),
            SlotEnum.Legs => GetRandomSkillFromArray(Legs),

            _ => throw new ArgumentException("Invalid armor slot")
        };
    }

    private BaseSkillModel GetWeaponRandomSkill(WeaponTypeEnum weaponType)
    {
        return weaponType switch
        {
            WeaponTypeEnum.Axe => GetRandomSkillFromArray(OneHandedAxe),
            WeaponTypeEnum.TwoHandedAxe => GetRandomSkillFromArray(TwoHandedAxe),
            WeaponTypeEnum.Sword => GetRandomSkillFromArray(OneHandedSword),
            WeaponTypeEnum.TwoHandedSword => GetRandomSkillFromArray(TwoHandedSword),
            WeaponTypeEnum.Hummer => GetRandomSkillFromArray(OneHandedHummer),
            WeaponTypeEnum.TwoHandedHummer => GetRandomSkillFromArray(TwoHandedHummer),
            WeaponTypeEnum.Mace => GetRandomSkillFromArray(OneHandedMace),
            WeaponTypeEnum.TwoHandedMace => GetRandomSkillFromArray(TwoHandedMace),
            WeaponTypeEnum.Spear => GetRandomSkillFromArray(Spear),
            WeaponTypeEnum.Bow => GetRandomSkillFromArray(Bow),
            WeaponTypeEnum.Staff => GetRandomSkillFromArray(Staff),
            WeaponTypeEnum.Shield => GetRandomSkillFromArray(Shield),

            _ => throw new ArgumentException("Invalid weapon type")
        };
    }

    private BaseSkillModel GetRandomSkillFromArray(BaseSkillTemplate[] skills)
    {
        if (skills == null || !skills.Any())
        {
            throw new InvalidOperationException($"No skills available");
        }

        return skills[UnityEngine.Random.Range(0, skills.Length)].GetSkill();
    }
}
