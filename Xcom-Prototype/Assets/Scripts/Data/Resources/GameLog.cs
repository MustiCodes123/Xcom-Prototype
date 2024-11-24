using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Zenject;

public class GameLog : MonoBehaviour
{
    public static GameLog Instance;

    [SerializeField] private GameObject _logString;
    [SerializeField] private Transform _logContainer;
    [SerializeField] private Transform _logWindow;

    private static List<string> _logs = new List<string>();

    [Inject]
    private void Init()
    {
        Instance = this;
    }

    public void Start()
    {
        //*** UnComment in Release Build ****
        //gameObject.SetActive(false);
        // ***  ***

        LoadLog();
    }

    public void OnLogButtonClick()
    {
        _logWindow.gameObject.SetActive(!_logWindow.gameObject.activeSelf);
    }

    public void AddLog(string text)
    {
        var line = Instantiate(_logString, _logContainer);
        var logText = "[" + System.DateTime.Now.ToString() + "] " + text;
        line.GetComponentInChildren<TextMeshProUGUI>().text = logText;
        _logs.Add(logText);
    }

    private void LoadLog()
    {
        foreach(var log in _logs)
        {
            var line = Instantiate(_logString, _logContainer);
            line.GetComponentInChildren<TextMeshProUGUI>().text = log;
        }
    }
}