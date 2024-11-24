using UnityEngine;
using UnityEngine.UI;

public class GroupProgressIcon : MonoBehaviour
{
    [SerializeField] private Image _shadow;
    [SerializeField] private Image _outline;
    [SerializeField] private Image _index;
    [SerializeField] private Image _skull;
    [SerializeField] private Image _slider;

    private int _maxCharacterCount;
    private int _deadCharacterCount;

    public void Initialize(int playerCount, bool isLastIcon = false)
    {
        gameObject.SetActive(true);
        _shadow.gameObject.SetActive(false);
        _outline.gameObject.SetActive(false);
        _skull.gameObject.SetActive(false);
        _maxCharacterCount = playerCount;
        if (_slider != null)
        {
            _slider.fillAmount = 0f;
            if (isLastIcon)
            {
                _slider.transform.parent.gameObject.SetActive(false);
            }                
        }           
    }

    public void Activate()
    {
        _outline.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _outline.gameObject.SetActive(false);
        _index.gameObject.SetActive(false);
        _outline.gameObject.SetActive(false);
        _skull.gameObject.SetActive(true);
    }

    public void ChangeSliderValue()
    {
        _deadCharacterCount++;
        if (_slider)
        {
            _slider.fillAmount = (float)_deadCharacterCount / _maxCharacterCount;
        }
            
    }
}
