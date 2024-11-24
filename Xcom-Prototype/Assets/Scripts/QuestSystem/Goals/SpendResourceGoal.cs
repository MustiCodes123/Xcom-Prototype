
public class SpendResourceGoal : QuestGoal
{
    public ResourceType ResourceType;

    public override void Process(ISignal signal)
    {
        if (signal is UseResourceSignal)
        {
            UseResourceSignal spendResourceSignal = (UseResourceSignal)signal;
            if (ResourceType == spendResourceSignal.ResourceType && spendResourceSignal.IsSpendSignal)
            {
                CurrentAmount += spendResourceSignal.Count;
            }
        }
    }
}