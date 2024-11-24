using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BossMapController : UIWindowView
{
    [Inject] private TemploaryInfo _temploaryInfo;
    [Inject] private UIWindowManager _UIWindowManager;

    [SerializeField] private BossSelectionButton[] _bossSelectionButtons;

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        foreach(BossSelectionButton bossButton in _bossSelectionButtons)
        {
            bossButton.Click += OnBossButtonClick;
        }
    }

    private void OnDisable()
    {
        foreach (BossSelectionButton bossButton in _bossSelectionButtons)
        {
            bossButton.Click -= OnBossButtonClick;
        }
    }
    #endregion

    #region Overrided Methods

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Callbacks
    private void OnBossButtonClick(BossData bossData)
    {
        _temploaryInfo.CurrentBoss = bossData;
        _UIWindowManager.ShowWindow(_temploaryInfo.CurrentMode.Window);
    }
    #endregion
}
