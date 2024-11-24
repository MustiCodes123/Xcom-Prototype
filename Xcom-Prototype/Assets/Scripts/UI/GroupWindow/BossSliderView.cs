using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BossSliderView : CharacterCardView
{
    [SerializeField] private Image _topSlider;
    [SerializeField] private Image _bottomSlider;
    [SerializeField] private Image _nextTopSlider;
    [SerializeField] private Image _nextBottomSlider;

    [Inject] private ThreeToOneContainer _bossContainer;
    [Inject] private TemploaryInfo _temploaryInfo;

    private BaseCharacterModel _myCharacterModel;
    private int _currentIndexColor;

    public void SetBossCharacterInfo(BaseCharacterModel model)
    {
        IsBot = true;
        gameObject.SetActive(true);

        _myCharacterModel = model;

        UpdateCardHPBar(CharacterMaxHP);
        _currentIndexColor++;

        _topSlider.color = _bossContainer.SliderColors[0].TopColor;
        _bottomSlider.color = _bossContainer.SliderColors[0].BottomColor;

        if (_temploaryInfo.CurrentMode.GameMode == GameMode.TestOfStrenght)
        {
            _nextTopSlider.color = _bossContainer.SliderColors[_currentIndexColor].TopColor;
            _nextBottomSlider.color = _bossContainer.SliderColors[_currentIndexColor].BottomColor;
            _nextTopSlider.gameObject.SetActive(true);
            _nextBottomSlider.gameObject.SetActive(true);
        }
    }

    public override void UpdateCardHPBar(int currentHP)
    {
        CharacterMaxHP = _myCharacterModel.GetMaxHP();
        HealthBarSlider.fillAmount = Interpolate(0f, (float)CharacterMaxHP, 0f, 1f, (float)currentHP);
    }

    public void ChangeSliderColors()
    {
        HealthBarSlider.fillAmount = 1;
        _topSlider.color = _nextTopSlider.color;
        _bottomSlider.color = _nextBottomSlider.color;

        if (_currentIndexColor < _bossContainer.SliderColors.Length - 1)
            _currentIndexColor++;
        else
            _currentIndexColor = 0;

        _nextTopSlider.color = _bossContainer.SliderColors[_currentIndexColor].TopColor;
        _nextBottomSlider.color = _bossContainer.SliderColors[_currentIndexColor].BottomColor;
    }

    public void FillSlider()
    {
        HealthBarSlider.fillAmount = 1;
    }

}
