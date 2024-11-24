using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PortalWindow;

public class CristalTooggle : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGO;
    [SerializeField] private SummonCristalsEnum cristalEnum;

    private Action<SummonCristalsEnum> onCristalSelected;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        onCristalSelected?.Invoke(cristalEnum);
    }

    public void SetSelected(bool selected)
    {
        selectedGO.SetActive(selected);
    }

    public void SetData(string name, string description, string count, Sprite sprite, Action<SummonCristalsEnum> action)
    {
        onCristalSelected = action;
        this.name.text = name;
        this.description.text = description;
        this.count.text = count;
        image.sprite = sprite;
    }


}

