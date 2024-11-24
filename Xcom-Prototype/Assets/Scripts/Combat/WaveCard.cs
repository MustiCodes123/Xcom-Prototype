using UnityEngine.UI;
using UnityEngine;

public class WaveCard : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image backGround;
    [SerializeField] private Sprite _activeBG;

    private int maxEnemies;

    public void SetSliderMaxValue(int value)
    {
        slider.maxValue = value;
        maxEnemies = value;
    }

    public void ActiveSlider(bool value)
    {
        slider.gameObject.SetActive(value);
    }

    public void SetSliderValue(int enemyCount)
    {
        slider.value = maxEnemies - enemyCount;
    }

    public bool IsComplete()
    {
        return slider.maxValue == slider.value;
    }

    public void SetActiveFrame()
    {
        backGround.sprite = _activeBG;
    }

    public void ResetSliderValue()
    {
        slider.value = 0;
    }
}