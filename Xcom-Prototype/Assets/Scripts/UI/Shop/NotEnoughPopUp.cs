using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughPopUp : MonoBehaviour
{
    [SerializeField] private Button _bankButton;
    [SerializeField] private Button _cancelButton;

    private ShopPresenter _presenter;

    public void Initialize(ShopPresenter presenter)
    {
        _presenter = presenter;

        RemoveListeners();

        _cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
        _bankButton.onClick.AddListener(OnOpenBankWindow);

        gameObject.SetActive(false);
    }

    public void Show()
    {
        transform.DOScale(1, 0.3f).From(0.8f).SetEase(Ease.OutBack);
        gameObject.SetActive(true);
    }

    private void OnOpenBankWindow()
    {
        int bankWindowID = 2;
        _presenter.OnCategorySelected(bankWindowID, WindowType.Bank);

        gameObject.SetActive(false);
    }

    private void RemoveListeners()
    {
        _bankButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
    }
}