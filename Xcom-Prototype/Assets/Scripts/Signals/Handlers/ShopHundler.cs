using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ShopHundler
{
    private SignalBus signalBus;

    [Inject] private ShopController shopController;
    [Inject] private PlayerData playerData;

    public ShopHundler(SignalBus signalBus)
    {
        this.signalBus = signalBus;
        
    }

    public bool BuyInventoryExtention()
    {
        return shopController.BuyInventorySlot();
    }

    public bool TryBoughtItem(BaseItem item)
    {
        if (shopController.TryBoughtItem(item))
        {
            signalBus.Fire(new ItemBuySignal() { item = item, Price = item.itemPrice });
            signalBus.Fire(new UseResourceSignal() { Count = item.itemPrice, ResourceType = ResourceType.Gold, IsSpendSignal = true });
            return true;
        }
   
        return false;
    }

    public bool TryBoughtSkill(BaseSkillTemplate item)
    {
        if (shopController.TryBoughtSkill(item))
        {
            signalBus.Fire(new ItemBuySignal() { Skill = item, Price = item.BuyPrice });
            signalBus.Fire(new UseResourceSignal() { Count = item.BuyPrice, ResourceType = ResourceType.Gold, IsSpendSignal = true });
            return true;
        }

        return false;
    }
}
