using System;
using DG.Tweening;
using UnityEngine;

public class AnimateUIElements : MonoBehaviour
{
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] private RectTransform _bottomPanel;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float leftPanelOffset = 500f;
    [SerializeField] private float rightPanelOffset = 500f;
    [SerializeField] private float bottomPanelOffset = 500f;

    private Vector2 _leftPanelStartPos;
    private Vector2 _rightPanelStartPos;
    private Vector2 _bottomPanelStartPos;

    private Vector2 _leftPanelOffsetPos;
    private Vector2 _rightPanelOffsetPos;
    private Vector2 _bottomPanelOffsetPos;

    private Tween _leftPanelTween;
    private Tween _rightPanelTween;
    private Tween _bottomPanelTween;
    

    private void Awake()
    {
        if (_leftPanel != null)
        {
            _leftPanelStartPos = _leftPanel.anchoredPosition;
            _leftPanelOffsetPos = new Vector2(_leftPanelStartPos.x - leftPanelOffset, _leftPanelStartPos.y);
        }

        if (_rightPanel != null)
        {
            _rightPanelStartPos = _rightPanel.anchoredPosition;
            _rightPanelOffsetPos = new Vector2(_rightPanelStartPos.x + rightPanelOffset, _rightPanelStartPos.y);
        }

        if (_bottomPanel != null)
        {
            _bottomPanelStartPos = _bottomPanel.anchoredPosition;
            _bottomPanelOffsetPos = new Vector2(_bottomPanelStartPos.x, _bottomPanelStartPos.y - bottomPanelOffset);
        }
    }

    public void AnimatePanelsIn()
    {
        if (_leftPanel != null)
        {
            _leftPanel.anchoredPosition = _leftPanelOffsetPos;
            _leftPanelTween = _leftPanel.DOAnchorPos(_leftPanelStartPos, animationDuration);
        }

        if (_rightPanel != null)
        {
            _rightPanel.anchoredPosition = _rightPanelOffsetPos;
            _rightPanelTween = _rightPanel.DOAnchorPos(_rightPanelStartPos, animationDuration);
        }

        if (_bottomPanel != null)
        {
            _bottomPanel.anchoredPosition = _bottomPanelOffsetPos;
            _bottomPanelTween = _bottomPanel.DOAnchorPos(_bottomPanelStartPos, animationDuration);
        }
    }

    public void AnimatePanelsOut()
    {
        
        if (_leftPanel != null)
        {
            _leftPanelTween = _leftPanel.DOAnchorPos(_leftPanelOffsetPos, animationDuration);
        }
        if (_rightPanel != null)
        {
            
            _rightPanelTween = _rightPanel.DOAnchorPos(_rightPanelOffsetPos, animationDuration);
     
        }
        if (_bottomPanel != null)
        {
            _bottomPanelTween = _bottomPanel.DOAnchorPos(_bottomPanelOffsetPos, animationDuration);
        }
    }
}
