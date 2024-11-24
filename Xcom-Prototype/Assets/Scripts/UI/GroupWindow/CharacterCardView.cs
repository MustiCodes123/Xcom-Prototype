using DG.Tweening;
using System;
using System.Collections;
using Data.Resources.AddressableManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardView : MonoBehaviour
{
    public bool IsPaused { get; set; }
    public bool IsActive = true;
    public bool IsBot = false;

    public BaseCharacterModel CharacterModel => characterInfo;

    [SerializeField] private Image characterAvatar;
    [SerializeField] private Image ActiveFrame;
    [SerializeField] private Image DisableFrame;
    [SerializeField] private GameObject selectedGO;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterLevel;
    [SerializeField] private TextMeshProUGUI disableCounter;
    [SerializeField] private Button selectButton;

    [SerializeField] private GameObject levelUpGO;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject defeatedFade;
    [SerializeField] private GameObject defeatedMark;
    [SerializeField] private GameObject _damageDealedBlock;
    [SerializeField] private GameObject _damageReceivedBlock;
    public Image HealthBarSlider;
    [SerializeField] private Image _expBarSlider;
    [SerializeField] private Image _damageBarSlider;
    [SerializeField] private Image _damageReceivedBarSlider;
    [SerializeField] private TextMeshProUGUI ExpBarText;
    [SerializeField] private TextMeshProUGUI ExpBarAddedText;
    [SerializeField] private TextMeshProUGUI _totalDamageDealed;
    [SerializeField] private TextMeshProUGUI _totalDamageReceived;
    [SerializeField] private Gradient barColorGradient;
    [SerializeField] private Image disableMask;
    [SerializeField] private float _disableTimer = 5f;

    private TemploaryInfo _temploaryInfo;
    private BaseCharacterModel characterInfo;

    private int _disableTime = 5;

    protected int CharacterMaxHP;

    private Action<BaseCharacterModel, CharacterCardView, bool> action;

    private ResourceManager _resourceManager;

    private void Awake()
    {
        if (IsBot || selectButton == null) return;
        selectButton.onClick.AddListener(OnSelectButtonClick);
    }

    private void OnSelectButtonClick()
    {
        action?.Invoke(characterInfo,this, !selectedGO.activeInHierarchy);
    }

    public void SetSelected(bool isSelected)
    {
        //if (IsBot) return;
        //selectedGO.SetActive(isSelected);
    }

    public void ShowLvlUp()
    {
        if (IsBot) return;
        levelUpGO.SetActive(true);
    }

    public void SetDefeated()
    {
        if (IsBot) return;

        ActiveFrame.gameObject.SetActive(false);
        selectButton.interactable = false;
        defeatedFade.SetActive(true);
        defeatedMark.SetActive(true);
        HealthBarSlider.enabled = false;
        IsActive = false;
    }

    public void SetAlive()
    {
        if (IsBot) return;

        ActiveFrame.gameObject.SetActive(true);
        selectButton.interactable = false;
        defeatedFade.SetActive(false);
        defeatedMark.SetActive(false);
        HealthBarSlider.enabled = true;
        IsActive = true;
    }

    public void SetCharacterInfo(BaseCharacterModel characterInfo, bool isSlecected, CharacterHandler characterHandler,  Action<BaseCharacterModel, CharacterCardView, bool> _action, TemploaryInfo temploaryInfo = null, ResourceManager resourceManager = null)
    {
        _resourceManager = resourceManager;
        gameObject.SetActive(true);
        action = _action;
        this.characterInfo = characterInfo;
        selectedGO.SetActive(isSlecected);

        characterAvatar.sprite = _resourceManager.LoadSprite(characterInfo.AvatarId);

        characterName.text = characterInfo.Name;
        characterLevel.text = (characterInfo.Level).ToString();
        CharacterMaxHP = characterInfo.MaxHealth;
        _temploaryInfo = temploaryInfo;
    }

    public void SetEnemyCharacterInfo(FakeLeader fakeLeader, BaseCharacterModel model, ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;

        IsBot = true;
        gameObject.SetActive(true);
        characterAvatar.sprite = _resourceManager.LoadSprite(model.AvatarId);
        characterLevel.text = (fakeLeader.CurrentSaveData.Level + 1).ToString();
        CharacterMaxHP = model.MaxHealth;
    }

    public void AnimateXPBar(int xpAdded)
    {
        if (IsBot) return;
        gameObject.transform.localScale = Vector3.zero;

        gameObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBounce).OnComplete(() => {

            if (_temploaryInfo != null)
            {
                _damageDealedBlock.gameObject.SetActive(true);
                _totalDamageDealed.text = characterInfo.Score.GetDealedDamage().ToString();
                float damageDealed = Interpolate(0, (float)_temploaryInfo.Score.GetTotalDealedDamage(), 0, 1, (float)characterInfo.Score.GetDealedDamage());
                _damageBarSlider.DOFillAmount(damageDealed, 0.5f).From(0).SetEase(Ease.OutBack);

                _damageReceivedBlock.gameObject.SetActive(true);
                _totalDamageReceived.text = characterInfo.Score.GetReceivedDamage().ToString();
                float damageReceived = Interpolate(0, (float)_temploaryInfo.Score.GetTotalReceivedDamage(), 0, 1, (float)characterInfo.Score.GetReceivedDamage());
                _damageReceivedBarSlider.DOFillAmount(damageReceived, 0.5f).From(0).SetEase(Ease.OutBack);
            }

            healthBar.SetActive(true);
            HealthBarSlider.gameObject.SetActive(true);
            healthBar.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.InBounce);
            float hpFillValue = Interpolate(0, (float)characterInfo.GetMaxHP(), 0, 1, (float)characterInfo.Health);
            HealthBarSlider.DOFillAmount(hpFillValue, 0.5f).From(0).SetEase(Ease.OutBack);

            float expFillValue = Interpolate(0, (float)characterInfo.XpToNextLevel, 0,1, (float)characterInfo.Xp);
            _expBarSlider.gameObject.SetActive(true);
            _expBarSlider.DOFillAmount(expFillValue, 0.5f).From(0).SetEase(Ease.OutBack).OnComplete(() => {

                ExpBarAddedText.gameObject.SetActive(true);

                if (characterInfo.IsMaxLevel)
                {
                    ExpBarAddedText.gameObject.SetActive(true);
                    ExpBarAddedText.text = "MAX LEVEL";
                }
                else
                {
                    ExpBarAddedText.text = characterInfo.Xp + "/" + characterInfo.XpToNextLevel;
                }

                ExpBarAddedText.transform.DOScale(1, 0.5f).From(Vector3.zero).SetEase(Ease.InBounce);
            });
            
        });
    }

    public virtual void UpdateCardHPBar(int currentHP)
    {
        HealthBarSlider.fillAmount = Interpolate(0f, (float)CharacterMaxHP, 0f, 1f, (float)currentHP);
        HealthBarSlider.color = barColorGradient.Evaluate(HealthBarSlider.fillAmount);

        if(HealthBarSlider.fillAmount <= 0f)
        {
            defeatedFade.SetActive(true);
            defeatedMark.SetActive(true);
        }
    }

    public void SelectCard()
    {
        ActiveFrame.gameObject.SetActive(true);
    }

    public void UnSelectCard()
    {
        ActiveFrame.gameObject.SetActive(false);
    }

    public void DisableCard()
    {
        ActiveFrame.gameObject.SetActive(false);
        selectButton.interactable = false;

        //IsActive = false;
        //_disableTimer = _disableTime;
        //disableMask.gameObject.SetActive(true);
    }
    

    public float Interpolate(float Xmin, float Xmax, float Ymin, float Ymax, float Xvalue)
    {
        return Ymin + (Xvalue - Xmin) / (Xmax - Xmin) * (Ymax - Ymin);
    }

    private void OnDestroy()
    {
        if (IsBot) return;
        selectButton.onClick.RemoveListener(OnSelectButtonClick);
        //DOTween.Kill(transform);
        // _signalBus.Unsubscribe<ChangeGameStateSignal>(OnChangeGameState);
    }
}
