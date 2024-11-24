
public class TemporaryReward : TemporaryQuest
{
    public void SetupTeporaryRewardGoal(int questsCount, int completedQuestsCount)
    {
        IsCompleted = false;
        IsRewardTaken = false;

        foreach (QuestGoal goal in Goals)
        {
            goal.CurrentAmount = completedQuestsCount;
            goal.IsReached = false;
            goal.GoalTime = GetQuestTimeByType(_questType);
            var newQuestAmount = questsCount;
            goal.SetRequiredAmount(newQuestAmount);

            if (IsQuestTimeFinished(goal.GoalTime))
            {
                IsCompleted = false;
                IsRewardTaken = false;
            }
        }

    }
}