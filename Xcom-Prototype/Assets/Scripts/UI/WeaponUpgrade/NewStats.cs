using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI chanse;
    [SerializeField] private TextMeshProUGUI currLvl;
    [SerializeField] private TextMeshProUGUI nextLvl;
    [SerializeField] private StatString statString1;
    [SerializeField] private StatString statString2;
    [SerializeField] private StatString statString3;

    private const string minDamageString = "Min Damage";
    private const string maxDamageString = "Max Damage";
    private const string attackSpeedString = "Attack Speed";
    private const string armorString = "Armor";
    private const string armorHealthString = "Health";
    private const string armorManaString = "Mana";

    public void SetupWeaponStats(BaseItem item, float dropChanse)
    {
        weaponName.text = item.itemName;
        currLvl.text = (item.CurrentLevel).ToString();
        nextLvl.text = (item.CurrentLevel + 1).ToString();
        chanse.text = ((int)dropChanse).ToString() + " %";

        if (item is WeaponItem weapon)
        {
            statString1.SetStats(minDamageString, weapon.minDamage.ToString(), (weapon.NextMinDamage).ToString());
            statString2.SetStats(maxDamageString, weapon.maxDamage.ToString(), (weapon.NextMaxDamage).ToString());
            statString3.SetStats(attackSpeedString, weapon.attackSpeed.ToString(), (weapon.NextAttackSpeed).ToString());
        }

        if (item is ArmorItem armor)
        {
            statString1.SetStats(armorString, armor.Armor.ToString(), (armor.NextArmor).ToString());
            statString2.SetStats(armorHealthString, armor.Health.ToString(), (armor.NextHealth).ToString());
            statString3.SetStats(armorManaString, armor.Mana.ToString(), (armor.NextMana).ToString());
        }
    }
}
