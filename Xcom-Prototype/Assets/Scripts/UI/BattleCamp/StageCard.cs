using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageCard : Card
{
    public void SetStage(Stage stage ,Action<Stage> action)
    {
        Button.onClick.AddListener(() =>
        {
            action?.Invoke(stage);
        });

        Description.text = stage.Name;
        MainImage.sprite = stage.Sprite;
    }
}
