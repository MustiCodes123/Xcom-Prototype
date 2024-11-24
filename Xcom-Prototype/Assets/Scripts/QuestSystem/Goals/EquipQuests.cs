using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipQuests : QuestGoal
{
   [JsonIgnore] public ItemTemplate NeededItem;

    public override void Process(ISignal signal)
    {
        if (signal is CharacterEquipSignal)
        {
            var equipSignal = (CharacterEquipSignal)signal;
            if (equipSignal.baseItem.itemID == NeededItem.itemID)
            {
                CurrentAmount++;
                Evaluate();
            }
        }
    }

}
