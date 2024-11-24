using System;
using UnityEngine;
using UnityEngine.UI;

public class AFCircle : MonoBehaviour
{
    public Action ItemClick;

    [SerializeField] private Image _itemIcon;
    [SerializeField] private Button _button;

    private void OnEnable() => _button.onClick.AddListener(OnItemClick);

    private void OnDisable() => _button.onClick.RemoveAllListeners();

    public void ShowItem(Sprite itemIcon)
    {
        _itemIcon.sprite = itemIcon;
        _itemIcon.gameObject.SetActive(true);
    }

    public void Unselect()
    {
        _itemIcon.sprite = null;
        _itemIcon.gameObject.SetActive(false);
    }

    public void OnItemClick() => ItemClick?.Invoke();
}