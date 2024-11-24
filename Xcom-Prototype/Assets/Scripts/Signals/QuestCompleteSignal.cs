using System.Collections;
using UnityEngine;

public struct QuestCompleteSignal : ISignal
{
    public int QuestCount;
    public QuestTypeEnum QuestType;
}