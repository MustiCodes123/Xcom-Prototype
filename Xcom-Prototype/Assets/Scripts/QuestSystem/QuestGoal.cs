using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public abstract class QuestGoal : MonoBehaviour
{
    [JsonIgnore] public QuestGoalEnum GoalType;
    public int GoalTime;
    public int RequiredAmount;
    public int CurrentAmount;
    public bool IsReached;

    public virtual void Process(ISignal signal)
    {
        
    }

    public void ParseString(string json)
    {
        string[] data = json.Split('/');
        RequiredAmount = int.Parse(data[0]);
        CurrentAmount = int.Parse(data[1]);
        IsReached = bool.Parse(data[2]);
        GoalTime = int.Parse(data[3]);
    }

    public string GetStringValue() => $"{RequiredAmount}/{CurrentAmount}/{IsReached}/{GoalTime}";

    public virtual void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            IsReached = true;
        }
        else
        {
            IsReached = false;
        }
    }

    public void SetRequiredAmount(int amount)
    {
        RequiredAmount = amount;
    }
    
    public void ResetCurrentAmount()
    {
        CurrentAmount = 0;
        IsReached = false; // Optionally reset IsReached if necessary
    }
}
