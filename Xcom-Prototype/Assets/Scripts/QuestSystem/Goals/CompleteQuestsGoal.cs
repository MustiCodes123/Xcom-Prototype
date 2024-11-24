using System.Collections;
using UnityEngine;

public class CompleteQuestsGoal : QuestGoal
{
    public QuestTypeEnum CompleteQuestType;
    public override void Process(ISignal signal)
    {
        if (signal is QuestCompleteSignal)
        {
            QuestCompleteSignal questCompleteSignal = (QuestCompleteSignal)signal;
            if (CompleteQuestType == questCompleteSignal.QuestType)
            {
                CurrentAmount = questCompleteSignal.QuestCount;
                Evaluate();
            }

        }
    }
}