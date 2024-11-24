using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterStatsViewer : MonoBehaviour
{   
    [SerializeField] StatsInfoViewer statsInfoViewer;
    [SerializeField] Transform statsContainer;

    [SerializeField] private List<StatsInfoViewer> statsInfoViewers = new List<StatsInfoViewer>();

    private BaseCharacterModel _characterInfo;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        signalBus.Subscribe<CharacterSelectSignal>(SetCharacterStats);   
    }

    public void SetCharacterStats(CharacterSelectSignal characterSignal)
    {
        _characterInfo = characterSignal.CharacterInfo;

        ShowStats();
    }

    public void SetCharacterStats(BaseCharacterModel characterSignal)
    {
        _characterInfo = characterSignal;

        ShowStats();
    }
    public void SetCharacterStats(CharacterPreset characterPreset)
    {
        ShowStats(characterPreset);
    }

    private void ShowStats()
    {
        var damageRange = _characterInfo.GetDamageRange(GetDamageType());

        statsInfoViewers[0].SetInfo($"{damageRange.min}-{damageRange.max}");
        statsInfoViewers[1].SetInfo(_characterInfo.MaxArmor.ToString());
        statsInfoViewers[2].SetInfo(_characterInfo.Magic.ToString());
        statsInfoViewers[3].SetInfo(_characterInfo.MagicDefense.ToString());
        statsInfoViewers[4].SetInfo(SetPersentFormat(_characterInfo.GetAttackSpeed()));
        statsInfoViewers[5].SetInfo(_characterInfo.GetMaxMana().ToString());
        statsInfoViewers[6].SetInfo(SetPersentFormat(ParseToPrecent(_characterInfo.CriticalChance)));
        statsInfoViewers[7].SetInfo(_characterInfo.GetMaxHP().ToString());
        statsInfoViewers[8].SetInfo(SetDecsimalFormat(_characterInfo.GetAtackDistance()));
        _characterInfo.CalculateSpeed();
        statsInfoViewers[9].SetInfo((SetDecsimalFormat(_characterInfo.MoveSpeed * _characterInfo.SpeedMultiplier)));
        statsInfoViewers[10].SetInfo(SetPersentFormat(_characterInfo.PhysicalResistance));
        statsInfoViewers[11].SetInfo(SetDecsimalFormat(_characterInfo.LiftingCapacity));
        statsInfoViewers[12].SetInfo(SetPersentFormat(_characterInfo.BlockChance));
        statsInfoViewers[13].SetInfo(SetPersentFormat(ParseToPrecent(_characterInfo.DodgeAdditional)));
        statsInfoViewers[14].SetInfo(SetPersentFormat(ParseToPrecent(_characterInfo.CriticalDamage)));
        statsInfoViewers[15].SetInfo(SetPersentFormat(ParseToPrecent(_characterInfo.MagicCriticalDamage)));
    }


    private void ShowStats(CharacterPreset characterPreset)
    {
        statsInfoViewers[0].SetInfo(characterPreset.Parameters.Attack.ToString());
        statsInfoViewers[1].SetInfo(0.ToString());
        statsInfoViewers[2].SetInfo(characterPreset.Parameters.Magic.ToString());
        statsInfoViewers[3].SetInfo(characterPreset.Parameters.MagicDefense.ToString());
        statsInfoViewers[4].SetInfo(SetDecsimalFormat(characterPreset.Parameters.AdditionalAttackSpeed));
        statsInfoViewers[5].SetInfo(characterPreset.Parameters.Mana.ToString());
        statsInfoViewers[6].SetInfo(SetPersentFormat(ParseToPrecent(characterPreset.Parameters.CriticalChance)));
        statsInfoViewers[7].SetInfo(characterPreset.Parameters.Health.ToString());
        statsInfoViewers[8].SetInfo(0.ToString());
        statsInfoViewers[9].SetInfo(SetDecsimalFormat(characterPreset.Parameters.MoveSpeed));
        statsInfoViewers[10].SetInfo(SetPersentFormat(characterPreset.Parameters.PhysicalResistance));
        statsInfoViewers[11].SetInfo(SetDecsimalFormat(characterPreset.Parameters.LiftingCapacity));
        statsInfoViewers[12].SetInfo(SetPersentFormat(characterPreset.Parameters.BlockChance));
        statsInfoViewers[13].SetInfo(SetPersentFormat(ParseToPrecent(characterPreset.Parameters.DodgeAdditional)));
        statsInfoViewers[14].SetInfo(SetPersentFormat(ParseToPrecent(characterPreset.Parameters.CriticalDamage)));
        statsInfoViewers[15].SetInfo(SetPersentFormat(ParseToPrecent(characterPreset.Parameters.MagicCriticalDamage)));
    }

    private AttackType GetDamageType()
    {
        if (_characterInfo.EquipedItems != null)
        {
            foreach (var item in _characterInfo.EquipedItems)
            {
                if (item is WeaponItem weapon)
                {
                    return weapon.DamageType;
                }
            }
        }

        return AttackType.Physical;
    }

    public void ClearStats()
    {
        foreach (var statsInfoViewer in statsInfoViewers)
        {
            statsInfoViewer.SetInfo(string.Empty);
        }
    }

    private string SetPersentFormat(float param)
    {
        string result = (Math.Round(param, 1)).ToString("F1") + " %".ToString();
        return result;
    }

    private string SetDecsimalFormat(float param)
    {
        string result = (Math.Round(param, 1)).ToString("F1");
        return result;
    }

    private int ParseToPrecent(float param)
    {
        int result = (int)Math.Round(param * 100);
        return result;
    }
}
