using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class CrystalAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _crystalShape;
    [SerializeField] private GameObject _destroyAnimation;
    [SerializeField] private GameObject _destroyDecale;
    [SerializeField] private GameObject _switchPanelsButtons;

    private Image _backGround;
    private PortalWindowController _portalWindowController;

    private const string _animatorIsUsed = "IsUsed";
    private const float _crystalShakeDyration = 1f;
    private const int _crystalPanelAnimationSpeed = 2;
    private const int _crystalPanelAnimationOffset = -2000;
    private const int _buttonlPanelAnimationOffset = -300;

    public void ActivateDestroyAnimation(Image backGround, PortalWindowController portalWindow)
    {
        _portalWindowController = portalWindow;
        _backGround = backGround;

        _crystalShape.transform.position = Vector3.zero;
        _crystalShape.transform.rotation = Quaternion.identity;
        _animator.SetBool(_animatorIsUsed, true);
        _destroyDecale.transform.localScale = Vector3.zero;
        _destroyDecale.SetActive(true);
        _destroyDecale.transform.DOScale(Vector3.one, _crystalShakeDyration).OnComplete(() => 
            {
                AnimateBang();
            });
    }
    
    public void Refresh()
    {
        ResetAnimation();

        if (_backGround != null)
        {
            DOTween.Kill(_backGround);
            _backGround.DOFade(1, 1);
        } 
    }

    public void RefreshImmidiatly()
    {
        ResetAnimation();

        if (_backGround != null)
        {
            DOTween.Kill(_backGround);
            _backGround.DOFade(1, 0);
        }
    }

    public void AnimateCrystalPanel(RectTransform panel)
    {
        panel.DOMoveX(_crystalPanelAnimationOffset, _crystalPanelAnimationSpeed);
    }
    public void ShowCrystalPanel(RectTransform panel)
    {
        panel.DOMoveX(0, 1);
    }
    public void AnimateButton(RectTransform button)
    {
        button.DOMoveY(_buttonlPanelAnimationOffset, _crystalPanelAnimationSpeed);
    }
    public void ShowButton(RectTransform button)
    {
        button.DOMoveY(0, 1);
    }

    private void ResetAnimation()
    {
        _crystalShape.transform.position = Vector3.zero;
        _crystalShape.transform.rotation = Quaternion.identity;
        _crystalShape.SetActive(true);
        _destroyAnimation.SetActive(false);
        _destroyDecale.SetActive(false);
        _animator.SetBool(_animatorIsUsed, false);
        _switchPanelsButtons.SetActive(true);
    }

    private void AnimateBang()
    {
        _crystalShape.SetActive(false);
        _destroyAnimation.SetActive(true);
        _destroyDecale.SetActive(false);
        _portalWindowController.ActivatePortal();
        DOTween.Kill(_backGround);
        _backGround.DOFade(0, 5);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
