using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UITopPanel : MonoBehaviour
{
    public WindowsEnum FirstWindow = WindowsEnum.None;
    
    private PlayerData playerData;
    private TemploaryInfo tempolaryInfo;
    private SignalBus signalBus;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI PlayerNameText;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private TextMeshProUGUI wnergtText;
    [SerializeField] private UIWindowManager windowManager;


    [Inject]
    public void Construct( PlayerData playerData, SignalBus signalBus, TemploaryInfo temploaryInfo)
    {
        this.playerData = playerData;
        signalBus.Subscribe<MoneyChangeSignal>(OnMoneyChange);
        this.signalBus = signalBus;
        tempolaryInfo = temploaryInfo;
    }

    private void Start()
    {
        UpdateTexts();
        StartCoroutine(MinuteCorutine());
    }

    public void UpdateTexts()
    {
        moneyText.text = "Money: " + playerData.Money;
        //PlayerNameText.text = playerData.PlayerName;
        gemsText.text = "Gem: " + playerData.Gems;
        wnergtText.text = playerData.Energy + " / " + playerData.MaxEnergy;
    }

    private IEnumerator MinuteCorutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            wnergtText.text = playerData.Energy + " / " + playerData.MaxEnergy;
        }
    }


    private void OnMoneyChange(MoneyChangeSignal moneyChangeSignal)
    {
        UpdateTexts();
    }

    private void OnDestroy()
    {
        signalBus.Unsubscribe<MoneyChangeSignal>(OnMoneyChange);
    }
}
