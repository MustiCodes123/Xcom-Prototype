using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsInfoPopup : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject _popup;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (_popup == null)
        {
            Debug.LogError("Popup is not assigned in the inspector");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ShowPopup();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HidePopup();
    }

    private void ShowPopup()
    {
        if (_popup != null)
        {
            _popup.SetActive(true);
        }
    }

    private void HidePopup()
    {
        if (_popup != null)
        {
            _popup.SetActive(false);
        }
    }
}