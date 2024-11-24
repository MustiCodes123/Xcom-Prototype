using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ViewConfig
{
    [field: SerializeField] public Sprite BackgroundSprite { get; private set; }
    [field: SerializeField] public Sprite IconSprite { get; private set; }
    [field: SerializeField] public TMP_FontAsset Font { get; private set; }
}


[RequireComponent(typeof(Button))]
public class SwitchSummonWindowButton : MonoBehaviour
{
    public ViewConfig SelectedViewConfig;
    public ViewConfig UnselectedViewConfig;

    [SerializeField] private List<SwitchSummonWindowButton> _switchButtons = new();
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _titleTMP;

    private Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void ChangeView(ViewConfig config)
    {
        _background.sprite = config.BackgroundSprite;
        _icon.sprite = config.IconSprite;
        _titleTMP.font = config.Font;
    }

    private void OnClick()
    {
        ChangeView(SelectedViewConfig);

        foreach(SwitchSummonWindowButton button in _switchButtons)
        {
            button.ChangeView(button.UnselectedViewConfig);
        }
    }
}
