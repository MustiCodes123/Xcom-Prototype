using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ThreeToOneContainer", menuName = "Data/ThreeToOneContainer")]
[Serializable]
public class ThreeToOneContainer : ScriptableObject
{
    public int[] BossStatsIncreases => _bossStatsIncreases;
    public BossSliderCollors[] SliderColors => _sliderColors;
    public BossData[] BossDatas => _bossDatas;

    [SerializeField] private int[] _bossStatsIncreases;
    [SerializeField] private BossSliderCollors[] _sliderColors;
    [SerializeField] private BossData[] _bossDatas = new BossData[5];
}

[Serializable]
public class BossSliderCollors
{
    public Color TopColor = Color.white;
    public Color BottomColor = Color.white;
}
