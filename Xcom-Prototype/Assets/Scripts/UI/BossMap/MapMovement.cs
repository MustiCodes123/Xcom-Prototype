using UnityEngine;

public class MapMovement : MonoBehaviour
{
    [SerializeField] private RectTransform _mapRectTransform;
    [SerializeField] private Canvas _canvas;

    private Vector2 _lastTouchPosition;
    private bool _isDragging = false;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _lastTouchPosition = touch.position;
                _isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && _isDragging)
            {
                Vector2 touchDelta = touch.position - _lastTouchPosition;
                _lastTouchPosition = touch.position;

                Vector2 newPosition = _mapRectTransform.anchoredPosition + touchDelta;
                newPosition = ClampPositionToCanvas(newPosition);

                _mapRectTransform.anchoredPosition = newPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _isDragging = false;
            }
        }
    }

    private Vector2 ClampPositionToCanvas(Vector2 position)
    {
        Vector2 canvasSize = _canvas.GetComponent<RectTransform>().sizeDelta;
        Vector2 mapSize = _mapRectTransform.sizeDelta;

        float minX = canvasSize.x - mapSize.x;
        float maxX = 0;
        float minY = 0;
        float maxY = mapSize.y - canvasSize.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
