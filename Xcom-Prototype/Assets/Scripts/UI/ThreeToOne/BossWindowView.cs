using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public abstract class BossWindowView : UIWindowView
{
    public UIWindowManager UIWindowManager => _uIWindowManager;
    public TemploaryInfo TemploaryInfo => _temploaryInfo;
    public PlayerData PlayerData => _playerData;
    public BossContent BossContent => _bossContent;
    protected int KeysCost => _keysCost;

    [SerializeField] private BossContent _bossContent;

    // [SerializeField] private TMP_Text _energyText;
    [SerializeField] private TMP_Text[] _playerKeysCount;
    [SerializeField] private TMP_Text _startCountText;
    [SerializeField] private Button _startBattleButton;
    [SerializeField] private Image _startBattleKey;
    [SerializeField] private Image _BG;

    [SerializeField] private Transform _rewardsContainer;
    [SerializeField] private RewardSlot _rewardSlotPrefab;

    [SerializeField] private ButtonSettings _greyButton;
    [SerializeField] private ButtonSettings _goldButton;

    private int _keysCost;

    [Inject] private PlayerData _playerData;
    [Inject] private ThreeToOneContainer _container;
    [Inject] private UIWindowManager _uIWindowManager;
    [Inject] private TemploaryInfo _temploaryInfo;

    public override void Show()
    {
        base.Show();
        _BG.gameObject.SetActive(true);

        _keysCost = _temploaryInfo.CurrentBoss.KeysCost;

        _bossContent.Initialize(_temploaryInfo.CurrentBoss, this);

        InitializeScreen(_temploaryInfo.CurrentBoss);
    }

    public void CreateRewards()
    {
        var bossData = _bossContent.BossData;
        foreach (Transform child in _rewardsContainer)
        {
            Destroy(child.gameObject);
        }
        var resources = bossData.Rewards[(int)bossData.Difficulty].ResourcesReward.Resources;
        var items = bossData.Rewards[(int)bossData.Difficulty].RewardItems;
        for (int i = 0; i < resources.Count; i++)
        {
            var slot = Instantiate(_rewardSlotPrefab, _rewardsContainer);
            slot.InitializeResource(resources[i]);
        }
        for (int i = 0; i < items.Count; i++)
        {
            var slot = Instantiate(_rewardSlotPrefab, _rewardsContainer);
            slot.InitializeItem(items[i].Item);
        }
    }

    public void ChangeButtonView(ButtonSettings settings)
    {
        _startBattleButton.image.sprite = settings.ButtonSprite;
        _startCountText.color = settings.TextColor;
        _startBattleKey.sprite = settings.KeySprite;
    }

    public void InitializeScreen(BossData data)
    {
        _startCountText.text = "Start           " + data.KeysCost.ToString();

        _startBattleButton.onClick.RemoveAllListeners();

        if (_playerData.KeysCount >= data.KeysCost)
        {
            ChangeButtonView(_goldButton);
            _startBattleButton.onClick.AddListener(() => StartBattle());
        }
        else
        {
            ChangeButtonView(_greyButton);
        }

        foreach (var text in _playerKeysCount)
        {
            text.text = _playerData.KeysCount.ToString() + " / " + _playerData.MaxKeysCount.ToString();
        }
        // _energyText.text = _playerData.Energy.ToString() + " / " + _playerData.MaxEnergy.ToString();
    }

    public override void Hide()
    {
        base.Hide();
        _BG.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public abstract void StartBattle();
}