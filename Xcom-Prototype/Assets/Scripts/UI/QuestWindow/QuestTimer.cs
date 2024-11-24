using System.Collections;
using TMPro;
using UnityEngine;

public class QuestTimer : MonoBehaviour
{
    [SerializeField] private GameObject dayTimerBlock;
    [SerializeField] private TextMeshProUGUI timerDays;
    [SerializeField] private TextMeshProUGUI timerHours;
    [SerializeField] private TextMeshProUGUI timerMinutes;
    [SerializeField] private int maxMounthDays;
    [SerializeField] private int maxWeekDays;
    [SerializeField] private int maxHours;
    [SerializeField] private int maxMinutes;

    private int _days;
    private int _hours;
    private int _minutes;

    private void OnEnable()
    {
        CalcilateTime();
        UpdateText();
    }

    public void ShowMonthlyTimer()
    {
        dayTimerBlock.SetActive(true);
        _days = maxMounthDays - System.DateTime.Now.Day;
        UpdateText();
    }

    public void ShowWeeklyTimer()
    {
        dayTimerBlock.SetActive(true);
        _days = maxWeekDays - (int)System.DateTime.Now.DayOfWeek;
        UpdateText();
    }

    public void ShowDaylyTimer()
    {
        dayTimerBlock.SetActive(false);
    }

    private void CalcilateTime()
    {
        _days = maxMounthDays - System.DateTime.Now.Day;
        _hours = maxHours - System.DateTime.Now.Hour;
        _minutes = maxMinutes - System.DateTime.Now.Minute;
    }

    private void UpdateText()
    {
        timerDays.text = _days.ToString();
        timerHours.text = _hours.ToString();
        timerMinutes.text = _minutes.ToString();
    }
}