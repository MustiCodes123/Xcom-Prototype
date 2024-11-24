using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PvPRewardPopUp : MonoBehaviour
{
    [Inject] private PvPBattleData _battleData;

    [SerializeField] private Image _goldImage;
    [SerializeField] private Image _gemsImage;
    [SerializeField] private Image _commonSummonCrystalImage;
    [SerializeField] private Image _rareSummonCrystalImage;
    [SerializeField] private Image _epicSummonCrystalImage;
    [SerializeField] private Image _legendarySummonCrystalImage;
    [SerializeField] private Image _keysImage;
    [SerializeField] private Image _energyImage;

    private void Start()
    {
        ShowRewardsForCurrentLeague();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    private void ShowRewardsForCurrentLeague()
    {
        Reward reward = _battleData.Reward;

        SetRewardImageActive(_goldImage, reward.Contains(ResourceType.Gold, out _));
        SetRewardImageActive(_gemsImage, reward.Contains(ResourceType.Gems, out _));
        SetRewardImageActive(_commonSummonCrystalImage, reward.Contains(ResourceType.CommonSummonCrystal, out _));
        SetRewardImageActive(_rareSummonCrystalImage, reward.Contains(ResourceType.RareSummonCrystal, out _));
        SetRewardImageActive(_epicSummonCrystalImage, reward.Contains(ResourceType.EpicSummonCrystal, out _));
        SetRewardImageActive(_legendarySummonCrystalImage, reward.Contains(ResourceType.LegendarySummonCrystal, out _));
        SetRewardImageActive(_keysImage, reward.Contains(ResourceType.Keys, out _));
        SetRewardImageActive(_energyImage, reward.Contains(ResourceType.Energy, out _));
    }

    private void SetRewardImageActive(Image image, bool active)
    {
        image.gameObject.SetActive(active);
    }
}