using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AFCategoryButtonViewConfig
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public bool IsSelected { get; private set; }
}

[RequireComponent(typeof(Button))]
public class AFCategoryButton : MonoBehaviour
{
    public Action<AFCategories, AFCategoryButton> Click;

    public AFCategoryButtonViewConfig SelectedView;
    public AFCategoryButtonViewConfig UnselectedView;

    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _highlight;

    [SerializeField] private AFCategories _category;

    private Button _button;

    private void OnEnable()
    {
        if (!TryGetComponent(out _button))
            Debug.LogError("AFCategoryButton is null");

        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void SetView(AFCategoryButtonViewConfig viewConfig)
    {
        _icon.sprite = viewConfig.Icon;
        _highlight.SetActive(viewConfig.IsSelected);
    }

    private void OnClick() => Click?.Invoke(_category, this);
}