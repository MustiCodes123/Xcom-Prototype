using System;
using UnityEngine;

public class TemporaryQuest : Quest
{
    [SerializeField] private int _questAmountMultiplyer = 1;

    private string _maxGoalMark = "|n|";
    private string _gameModeMark = "/GameMode/";
    private string _heroCountMark = "@HeroCount@";
    private int _minCountOfHeroes = 1;
    private int _maxNumberOfHeroes = 6;
    private int _firsLevel = 1;

    public virtual void SetupTemporaryGoal()
    {
        foreach (QuestGoal goal in Goals)
        {
            if (IsQuestTimeFinished(goal.GoalTime))
            {
                ResetQuest();
            }
        }
    }

    public bool IsQuestTimeFinished(int lastDateDay)
    {
        if (lastDateDay != GetQuestTimeByType(_questType))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ResetQuest()
    {
        foreach (QuestGoal goal in Goals)
        {
            goal.CurrentAmount = 0;
            goal.IsReached = false;
            goal.GoalTime = GetQuestTimeByType(_questType);
            Name = DefaultDescription;

            int newQuestAmount = 0;

            if (_playerData == null || _playerData.GetCompanyProgres().Count == 0)
            {
                newQuestAmount = _firsLevel * _questAmountMultiplyer;
            }
            else
            {
                newQuestAmount = _playerData.GetCompanyProgres().Count * _questAmountMultiplyer;
            }

            goal.SetRequiredAmount(newQuestAmount);
            Name = ReplaseTextByMark(_maxGoalMark, Name, newQuestAmount.ToString());
            Description = Name;

            if (goal is LevelFinishGoal levelGoal && levelGoal.IsRandomGameMode)
            {
                var newLevelMode = GetRandomMode();
                levelGoal.SetNewMode(newLevelMode);

                Name = ReplaseTextByMark(_gameModeMark, Name, newLevelMode.ToString());
                Description = Name;

                if (levelGoal.HeroCount > 0)
                {
                    System.Random random = new System.Random();
                    levelGoal.HeroCount = random.Next(_minCountOfHeroes, _maxNumberOfHeroes);

                    Name = ReplaseTextByMark(_heroCountMark, Name, levelGoal.HeroCount.ToString());
                    Description = Name;
                }
            }
        }

        IsCompleted = false;
        IsRewardTaken = false;
    }

    public static string ReplaseTextByMark(string wordMark, string text, string newWord)
    {
        var result = text.Replace(wordMark, newWord);

        return result;
    }

    protected GameMode GetRandomMode()
    {
        Array values = Enum.GetValues(typeof(GameMode));
        System.Random random = new System.Random();
        GameMode randomMode = (GameMode)values.GetValue(random.Next(values.Length));
        return randomMode;
    }
}