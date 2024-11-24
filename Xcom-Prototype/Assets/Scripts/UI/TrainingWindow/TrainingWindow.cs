using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using System.Collections;

public class TrainingWindow : UIWindowView, ITrainingView
{
    [Inject] private TrainingDataContainer _dataContainer;
    [Inject] private CameraHolder _cameraHolder;
    [Inject] private UICharacterVIew _characterView;
    [Inject] private ActivateUIInfo _activateUI;

    [SerializeField] private TrainingPresenter _presenter;
    [SerializeField] private TrainingCharacterList _characterList;
    [SerializeField] public TMP_Text _priceTMP;
    [SerializeField] private GameObject _seletedObj;
    [SerializeField] private CharacterStatsViewer _statsViewer;
    [SerializeField] private CharacterUIXPBar _xpBar;
    [SerializeField] private GameObject _maxLevelCharacterPopup;
    [SerializeField] private AnimateUIElements _uiElements;
    [SerializeField] private Transform _upgradeButtonTransform;

    #region Initialization
    public void Initialize()
    {
        _presenter.CharacterChanged += OnCharacterChanged;
        _presenter.CheckMaxLevel += OnMaxLevelCheck;
    }
    #endregion

    #region Overrided Methods
    public override void Show()
    {
        _characterView.ResetCharacterParentRotation();
        _characterView.ResetField();
        _activateUI.SetActiveUI(false);

        _cameraHolder.ActivateCharacterCamera();
        _uiElements.AnimatePanelsIn();

        ShowCharacters();
        
        if (_characterList.PlayerData.PlayerGroup.GetCharactersFromBothGroup().Count == 1)
        {
            gameObject.SetActive(true);
            StartCoroutine(SelectFirstCharacter());
        }
        UpdateWindowState();

        base.Show();
    }

    private IEnumerator SelectFirstCharacter()
    {
        yield return new WaitForEndOfFrame();
        _characterList.Cards[0].OnHeroButtonClick();
    }

    public override void Hide()
    {
        _upgradeButtonTransform.gameObject.SetActive(false);

        _characterView.ResetField();
        _dataContainer.TrainingCharacterCard.SetEmpty();
        _uiElements.AnimatePanelsOut();

        base.Hide();
        _cameraHolder.ActivateMainCamera();
    }

    protected override void OnDestroy()
    {
        _presenter.CharacterChanged -= OnCharacterChanged;
        _presenter.CheckMaxLevel -= OnMaxLevelCheck;

        base.OnDestroy();
    }
    #endregion

    #region View Methods
    public void ShowCharacters()
    {
        _dataContainer.TrainingCharacterList.ShowCharacters(_presenter.OnCharacterSelected, _dataContainer.CurrentCharacter);
        _dataContainer.TrainingCharacterCard.SetEmpty();
    }

    public void CharacterRemoved()
    {
        _characterView.ResetField();
        _maxLevelCharacterPopup.SetActive(false);
        UpdateWindowState();
    }

    public void SetCharacterStats(BaseCharacterModel character)
    {
        _statsViewer.SetCharacterStats(character);
    }

    public void SetupXPBar(BaseCharacterModel character, BaseCharacterModel? newCharacter=null)
    {
        if (newCharacter != null)
            _xpBar.SetupBar(character, newCharacter);
    }

    public void SetActiveUIWithoutAnim(bool value)
    {
        _activateUI.SetActiveUIWithoutAnim(value);
    }

    public void SetActiveUIwithAnim(bool value)
    {
        _activateUI.SetActiveUIwithAnim(value);
    }
    #endregion
    
    #region Callbacks
    private void OnMaxLevelCheck()
    {
        foreach (GameObject singleLock in _dataContainer.Locks)
            singleLock.SetActive(false);
        _maxLevelCharacterPopup.SetActive(false);
        _dataContainer.TrainingCharacterList.ShowAllCharacters();

        if (_dataContainer.CurrentCharacter?.IsMaxLevel == true)
        {
            for (int i = _dataContainer.CurrentCharacter.Stars; i < _dataContainer.Locks.Length; i++)
                _dataContainer.Locks[i].gameObject.SetActive(true);

            _maxLevelCharacterPopup.SetActive(true);
            _dataContainer.TrainingCharacterList.ShowRankUpCharacters(_dataContainer.CurrentCharacter);
        }
    }

    private void OnCharacterChanged()
    {
        _characterView.ResetCharacterParentRotation();
        UpdateWindowState();
    }
    #endregion

    #region Update State Methods
    private void UpdateWindowState()
    {
        OnMaxLevelCheck();
    }

    public void UpdatePrice(uint price)
    {
        _priceTMP.text = "0";
        if (_dataContainer.CurrentCharacter != null)
            _priceTMP.text = price.ToString();    
    }
    #endregion
}