using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private Transform target;

    private bool _isItemDragging = false;

    private void OnEnable()
    {
        BaseDragableItem.ItemDragStateChanged += OnItemDragStateChanged;
        SkillCard.ItemDragStateChanged += OnItemDragStateChanged;
    }

    private void OnDisable()
    {
        BaseDragableItem.ItemDragStateChanged -= OnItemDragStateChanged;
        SkillCard.ItemDragStateChanged -= OnItemDragStateChanged;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !_isItemDragging)
        {
            float h = rotationSpeed * Input.GetAxis("Mouse X");
            target.Rotate(0, h, 0);
        }
    }

    private void OnItemDragStateChanged(bool isDragging)
    {
        _isItemDragging = isDragging;
    }
}
