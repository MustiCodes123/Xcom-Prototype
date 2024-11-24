using TMPro;
using UnityEngine;

public class RewardDescriptionPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private RectTransform _popupRectTransform;
    [SerializeField] private float _verticalOffset = -25f;

    public void Display(string title, string description, Transform rewardItemTransform)
    {
        _title.text = title;
        _description.text = description;

        PositionPopup(rewardItemTransform);
        gameObject.SetActive(true);
    }

    private void PositionPopup(Transform rewardItemTransform)
    {
        Vector3[] corners = new Vector3[4];
        RectTransform rewardRectTransform = rewardItemTransform as RectTransform;

        if (rewardRectTransform != null)
        {
            rewardRectTransform.GetWorldCorners(corners);

            Vector3 centerPosition = (corners[0] + corners[2]) / 2;

            centerPosition.y += _verticalOffset;

            _popupRectTransform.position = centerPosition;
        }
    }
}
