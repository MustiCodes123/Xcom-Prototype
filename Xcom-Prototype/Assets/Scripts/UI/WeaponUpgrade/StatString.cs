using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatString : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statUpgradedNameText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private TextMeshProUGUI nextStatText;


    public void SetStats (string statName, string value, string nextValue)
    {
        statNameText.text = statName;
        statUpgradedNameText.text = statName;
        statText.text = value;
        nextStatText.text = nextValue;
    }
}
